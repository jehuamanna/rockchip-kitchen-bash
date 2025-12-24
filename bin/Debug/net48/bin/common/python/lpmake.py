#!/usr/bin/env python3
import argparse
import io
import os
import struct
import zlib
import configparser
from pathlib import Path

# -------------------- constants --------------------
LP_SECTOR_SIZE = 512
LBA_ALIGN_BYTES = 4096

GEOMETRY_AREA = 4096          # geometry region at start (0x0000..0x0FFF)
MAX_METADATA_SIZE = 65536     # size reserved for one metadata copy
BACKUP_METADATA_SIZE = 65536  # backup metadata copy (at end)

LP_GEOM_MAGIC = 0x616c4467    # "gDla"
LP_HEADER_MAGIC = 0x414C5030  # "0PLA"

# sparse image constants
SPARSE_MAGIC = 0xED26FF3A
CHUNK_RAW = 0xCAC1

# target types
LP_TARGET_TYPE_LINEAR = 0
LP_TARGET_TYPE_ZERO   = 1

# AOSP table entry structs (fixed)
PARTITION_STRUCT_V10 = struct.Struct("<36sIIII")  # name[36], attrs, first_extent, num_extents, group_index
EXTENT_STRUCT_V10    = struct.Struct("<QIQI")     # num_sectors, target_type, target_data, target_source
GROUP_STRUCT_V10     = struct.Struct("<36sIQ")    # name[36], flags, maximum_size

# Default AOSP blockdev entry (108 bytes)
BLOCKDEV_AOSP = struct.Struct("<36sQQQQI36s")

# Vendor-specific blockdev entries
BLOCKDEV_AMLOGIC  = struct.Struct("<36sIQQQ")     # 64
BLOCKDEV_ALLWIN   = struct.Struct("<32sIQQQ")     # 72
BLOCKDEV_ROCKCHIP = struct.Struct("<32sIIQQQ")    # 80

# header prefix and table descriptor
HDR_PREFIX = struct.Struct("<IHHIIII")  # magic, major, minor, header_size, header_crc32, tables_size, tables_crc32
TD        = struct.Struct("<III")       # offset, num_entries, entry_size

# -------------------- helpers --------------------
def crc32_le(data: bytes) -> int:
    return zlib.crc32(data) & 0xFFFFFFFF

def align_up(x: int, a: int) -> int:
    return (x + (a - 1)) // a * a

def pad_to(fp, align: int):
    off = fp.tell()
    pad = (-off) & (align - 1)
    if pad:
        fp.write(b"\x00" * pad)
    return fp.tell()

def normalize_vendor(v: str) -> str:
    v = (v or "").strip().lower()
    if "amlogic" in v:
        return "amlogic"
    if "allwinner" in v:
        return "allwinner"
    if "rockchip" in v:
        return "rockchip"
    if "aosp" in v or "generic" in v or v == "":
        return "aosp"
    return "aosp"  # safe default

def vendor_blockdev_layout(vendor: str):
    """Return (struct, name_field_len)."""
    if vendor == "amlogic":
        return BLOCKDEV_AMLOGIC, 36
    if vendor == "allwinner":
        return BLOCKDEV_ALLWIN, 32
    if vendor == "rockchip":
        return BLOCKDEV_ROCKCHIP, 32
    return BLOCKDEV_AOSP, 36  # aosp

def vendor_profile(vendor: str):
    """
    Returns (primary_metadata_offset, header_size) per vendor family, so we can match originals.
    """
    vendor = normalize_vendor(vendor)
    if vendor == "amlogic":
        # Many Amlogic firmwares put header right after geometry at 0x1000; header is 72 bytes
        return 0x1000, 72
    if vendor == "allwinner":
        # Commonly sees 0x3000 with short header
        return 0x3000, 72
    if vendor == "rockchip":
        # Often 0x3000 with short header as well
        return 0x3000, 72
    # AOSP/Generic observed with header at 0x3000 and hdr_sz=256 in your sample
    return 0x3000, 256

def is_sparse_required(cfg_path: Path) -> bool:
    cp = configparser.ConfigParser()
    cp.read(cfg_path, encoding="utf-8")
    return cp["super_metadata"].getboolean("is_sparse", fallback=False)

def read_cfg(cfg_path: Path):
    cp = configparser.ConfigParser()
    cp.read(cfg_path, encoding="utf-8")

    md = cp["super_metadata"]
    major = int(md.get("major_version", 10))
    minor = int(md.get("minor_version", 2))
    vendor = normalize_vendor(md.get("vendor", "aosp"))
    part_count = int(md.get("partition_count", 0))

    parts = []
    for i in range(part_count):
        sec = f"partition.{i}"
        if sec not in cp:
            continue
        name = cp[sec].get("name", f"partition_{i}")
        parts.append(name)

    return major, minor, vendor, parts

# -------------------- sparse writer --------------------
def write_sparse(raw_path: Path, final_out: Path):
    blk_sz = 4096
    total_sz = raw_path.stat().st_size
    total_blks = total_sz // blk_sz
    with open(raw_path, "rb") as inf, open(final_out, "wb") as outf:
        outf.write(struct.pack("<IHHHHIIII", SPARSE_MAGIC, 1, 0, 28, 12, blk_sz, total_blks, 1, 0))
        outf.write(struct.pack("<HHII", CHUNK_RAW, 0, total_blks, 0))
        while True:
            buf = inf.read(1024 * 1024)
            if not buf:
                break
            outf.write(buf)

# -------------------- metadata build --------------------
def build_tables(input_dir: Path, vendor: str, part_names, primary_meta_off: int):
    """
    Allocate one linear extent per partition, consecutive after primary metadata region.
    Build tables blob in order: partitions | extents | groups | blockdevs
    Return (tables_blob, layout_info)
    """
    # place payloads after the entire reserved metadata area
    data_start = align_up(primary_meta_off + MAX_METADATA_SIZE, LBA_ALIGN_BYTES)
    cursor = data_start

    partition_entries = []
    extent_entries = []
    group_entries = []
    bdev_entries = []

    # single group "main"
    gname = b"main\x00" + b"\x00" * (36 - len("main") - 1)
    group_entries.append(GROUP_STRUCT_V10.pack(gname, 0, 0x7FFF_FFFF_FFFF_FFFF))

    first_extent_index = 0
    for pname in part_names:
        img = input_dir / f"{pname}.img"
        if not img.exists():
            raise FileNotFoundError(f"Missing partition image: {img}")
        size_bytes = img.stat().st_size

        cursor = align_up(cursor, LBA_ALIGN_BYTES)
        phys_sector = cursor // LP_SECTOR_SIZE
        num_sectors = align_up(size_bytes, LP_SECTOR_SIZE) // LP_SECTOR_SIZE

        extent_entries.append(EXTENT_STRUCT_V10.pack(
            num_sectors, LP_TARGET_TYPE_LINEAR, phys_sector, 0
        ))

        name_bytes = pname.encode("ascii", "ignore")[:35]
        name_pad = name_bytes + b"\x00" * (36 - len(name_bytes))
        partition_entries.append(PARTITION_STRUCT_V10.pack(
            name_pad, 0, first_extent_index, 1, 0
        ))

        first_extent_index += 1
        cursor += size_bytes

    end_of_data = cursor

    # block device entry
    bstruct, name_field_len = vendor_blockdev_layout(vendor)
    bname_u = b"super"
    bname = bname_u[:name_field_len - 1] + b"\x00" * (name_field_len - len(bname_u[:name_field_len - 1]))

    first_logical_sector = data_start // LP_SECTOR_SIZE
    alignment = LBA_ALIGN_BYTES // LP_SECTOR_SIZE
    alignment_offset = 0
    size = align_up(end_of_data, LBA_ALIGN_BYTES)

    if vendor == "aosp":
        # AOSP: <36sQQQQI36s>
        entry = bstruct.pack(
            bname, first_logical_sector, alignment, alignment_offset, size, 0, b"\x00" * 36
        )
    elif vendor == "amlogic":
        # Amlogic: <36sIQQQ>  name, flags, first_sector, size, alignment
        entry = bstruct.pack(
            bname, 0, first_logical_sector, size, alignment
        )
    elif vendor == "allwinner":
        # Allwinner: <32sIQQQ>
        entry = bstruct.pack(
            bname, 0, first_logical_sector, size, alignment
        )
    elif vendor == "rockchip":
        # Rockchip: <32sIIQQQ>  name, type(int), flags(int), first_sector, size, alignment
        entry = bstruct.pack(
            bname, 0, 0, first_logical_sector, size, alignment
        )
    else:
        entry = BLOCKDEV_AOSP.pack(
            bname, first_logical_sector, alignment, alignment_offset, size, 0, b"\x00" * 36
        )

    bdev_entries.append(entry)

    # Build tables blob
    part_tbl = b"".join(partition_entries)
    ext_tbl  = b"".join(extent_entries)
    grp_tbl  = b"".join(group_entries)
    bdev_tbl = b"".join(bdev_entries)
    tables = part_tbl + ext_tbl + grp_tbl + bdev_tbl

    layout = {
        "num_parts": len(partition_entries),
        "num_exts": len(extent_entries),
        "num_grps": len(group_entries),
        "num_bdev": len(bdev_entries),

        "part_es": PARTITION_STRUCT_V10.size,
        "ext_es":  EXTENT_STRUCT_V10.size,
        "grp_es":  GROUP_STRUCT_V10.size,
        "bdev_es": len(entry),

        "primary_meta_off": primary_meta_off,
        "data_start": data_start,
        "end_of_data": end_of_data,
        "blockdev_size": size
    }
    return tables, layout

def write_geometry(fp, logical_block_size: int):
    """
    Minimal geometry block (liblp-compatible):
    magic, struct_size, checksum, metadata_max_size, metadata_slot_count, logical_block_size
    """
    GEOM = struct.Struct("<IIIIII")
    geom_struct_size = GEOM.size
    metadata_max_size = MAX_METADATA_SIZE
    slot_count = 2  # A/B
    logical_block = logical_block_size

    tmp = struct.pack("<IIIIII", LP_GEOM_MAGIC, geom_struct_size, 0, metadata_max_size, slot_count, logical_block)
    checksum = crc32_le(tmp)
    geom = struct.pack("<IIIIII", LP_GEOM_MAGIC, geom_struct_size, checksum, metadata_max_size, slot_count, logical_block)
    fp.write(geom.ljust(GEOMETRY_AREA, b"\x00"))

def write_metadata(fp, major, minor, tables: bytes, layout, vendor: str, header_size_override: int):
    """
    Write header + 4 table descriptors + optional header padding + tables blob.
    Offsets in descriptors are relative to the start of tables blob.
    Tables MUST start at (header_off + hdr_sz).
    """
    num_parts = layout["num_parts"]
    num_exts  = layout["num_exts"]
    num_grps  = layout["num_grps"]
    num_bdev  = layout["num_bdev"]

    part_es = layout["part_es"]
    ext_es  = layout["ext_es"]
    grp_es  = layout["grp_es"]
    bdev_es = layout["bdev_es"]

    # compute offsets inside tables blob
    part_off = 0
    ext_off  = part_off + num_parts * part_es
    grp_off  = ext_off  + num_exts  * ext_es
    bdev_off = grp_off  + num_grps  * grp_es

    tables_size = len(tables)

    # base header size = prefix + 4 descriptors (this is the *actual* bytes we write before padding)
    base_hdr_sz = HDR_PREFIX.size + TD.size * 4
    # if vendor requires larger hdr_sz (e.g., AOSP 256), we'll pad between descriptors and tables
    hdr_sz = max(base_hdr_sz, int(header_size_override) if header_size_override else base_hdr_sz)

    # header CRC is over header with header_crc32 field set to 0 (size = base_hdr_sz, NOT including padding)
    header_for_crc  = struct.pack("<IHHI", LP_HEADER_MAGIC, major, minor, hdr_sz)
    header_for_crc += struct.pack("<I", 0)
    header_for_crc += struct.pack("<I", tables_size)
    header_for_crc += struct.pack("<I", crc32_le(tables))
    header_crc = crc32_le(header_for_crc)

    # finalize header (prefix)
    header  = struct.pack("<IHHI", LP_HEADER_MAGIC, major, minor, hdr_sz)
    header += struct.pack("<I", header_crc)
    header += struct.pack("<I", tables_size)
    header += struct.pack("<I", crc32_le(tables))

    # descriptors
    part_td = TD.pack(part_off, num_parts, part_es)
    ext_td  = TD.pack(ext_off,  num_exts, ext_es)
    grp_td  = TD.pack(grp_off,  num_grps, grp_es)
    bdv_td  = TD.pack(bdev_off, num_bdev, bdev_es)

    # write header + descriptors
    fp.write(header)
    fp.write(part_td)
    fp.write(ext_td)
    fp.write(grp_td)
    fp.write(bdv_td)

    # pad up to hdr_sz (so tables start at header_off + hdr_sz)
    cur = fp.tell()
    need = hdr_sz - cur + layout["primary_meta_off"]  # since we will seek to primary_meta_off before calling this
    if need > 0:
        fp.write(b"\x00" * need)

    # finally write tables
    fp.write(tables)

    # total metadata bytes written (prefix+descriptors+padding+tables), not including outer padding to MAX_METADATA_SIZE
    return hdr_sz + tables_size

# -------------------- repack core --------------------
def repack_bootable(input_dir: Path, cfg_path: Path, out_img: Path):
    """
    Build a bootable super.img with vendor-specific metadata:
      - aosp, amlogic, allwinner, rockchip
    Uses one linear extent per partition.
    """
    success = False
    tmp_raw = None
    try:
        major, minor, vendor, part_names = read_cfg(cfg_path)
        primary_meta_off, header_size = vendor_profile(vendor)

        tables, layout = build_tables(input_dir, vendor, part_names, primary_meta_off)

        tmp_raw = out_img.with_suffix(".tmp_raw.img")
        with open(tmp_raw, "wb") as fp:
            # 1) geometry at 0x0000..0x0FFF
            write_geometry(fp, LBA_ALIGN_BYTES)

            # 2) seek to vendor-specific primary metadata offset and write metadata
            if fp.tell() < primary_meta_off:
                fp.write(b"\x00" * (primary_meta_off - fp.tell()))
            elif fp.tell() > primary_meta_off:
                raise RuntimeError(f"File cursor ({fp.tell():#x}) past desired header offset {primary_meta_off:#x}")

            meta_buf = io.BytesIO()
            # Use a temporary buffer to compute exact size, then write to file
            write_metadata(meta_buf, major, minor, tables, layout, vendor, header_size)
            meta_blob = meta_buf.getvalue()
            fp.write(meta_blob)

            # 2b) pad primary metadata region up to MAX_METADATA_SIZE from primary_meta_off
            cur = fp.tell()
            meta_end = primary_meta_off + MAX_METADATA_SIZE
            if cur < meta_end:
                fp.write(b"\x00" * (meta_end - cur))

            # 3) data payloads at computed data_start
            cur = fp.tell()
            if cur < layout["data_start"]:
                fp.write(b"\x00" * (layout["data_start"] - cur))

            cursor = layout["data_start"]
            for pname in part_names:
                img = input_dir / f"{pname}.img"
                if not img.exists():
                    raise FileNotFoundError(f"Missing partition image: {img}")
                if fp.tell() < cursor:
                    fp.write(b"\x00" * (cursor - fp.tell()))
                with open(img, "rb") as pf:
                    while True:
                        b = pf.read(1024 * 1024)
                        if not b:
                            break
                        fp.write(b)
                cursor = align_up(fp.tell(), LBA_ALIGN_BYTES)

            # 4) backup metadata at end (same content as primary), padded to BACKUP_METADATA_SIZE
            pad_to(fp, LBA_ALIGN_BYTES)
            fp.write(meta_blob)
            if len(meta_blob) < BACKUP_METADATA_SIZE:
                fp.write(b"\x00" * (BACKUP_METADATA_SIZE - len(meta_blob)))

        # honor sparse/raw preference, finalize to requested name
        if is_sparse_required(cfg_path):
            write_sparse(tmp_raw, out_img)
            os.remove(tmp_raw)
        else:
            if out_img.exists():
                os.remove(out_img)
            os.rename(tmp_raw, out_img)

        print("Repack completed successfully.")
        print(f"Vendor: {vendor}")
        print(f"Header offset: 0x{primary_meta_off:X}")
        print(f"Header size: {header_size} bytes")
        print(f"Output image: {out_img}")
        success = True
    except Exception as e:
        print(f"Repack failed: {e}")
    finally:
        if tmp_raw and tmp_raw.exists():
            try:
                os.remove(tmp_raw)
            except OSError:
                pass
        if not success:
            print("Output image was not created.")

# -------------------- CLI --------------------
def main():
    p = argparse.ArgumentParser(
        description="Build a bootable vendor-aware super.img (AOSP/Amlogic/Allwinner/Rockchip) "
                    "from .config/super_config and partition images."
    )
    p.add_argument("INPUT_FOLDER", type=Path, help="Folder containing partition images (e.g. 'out')")
    p.add_argument("CONFIG_FILE", type=Path, help="Path to .config/super_config")
    p.add_argument("-o", "--output", type=Path, default=Path("super_new.img"))
    args = p.parse_args()

    if not args.INPUT_FOLDER.exists():
        raise FileNotFoundError(f"Input folder not found: {args.INPUT_FOLDER}")
    if not args.CONFIG_FILE.exists():
        raise FileNotFoundError(f"Config file not found: {args.CONFIG_FILE}")

    repack_bootable(args.INPUT_FOLDER, args.CONFIG_FILE, args.output)

if __name__ == "__main__":
    main()
