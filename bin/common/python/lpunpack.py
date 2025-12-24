#!/usr/bin/env python3
import argparse, io, os, struct, tempfile, configparser
from pathlib import Path

SPARSE_MAGIC = 0xED26FF3A
CHUNK_RAW      = 0xCAC1
CHUNK_FILL     = 0xCAC2
CHUNK_DONTCARE = 0xCAC3
CHUNK_CRC32    = 0xCAC4

LP_GEOM_MAGIC   = 0x616c4467
LP_HEADER_MAGIC = 0x414C5030
LP_SECTOR_SIZE  = 512
LP_TARGET_TYPE_LINEAR = 0
LP_TARGET_TYPE_ZERO   = 1

# ---------------------------------------------------------------------
# Sparse image reader
# ---------------------------------------------------------------------
def _is_sparse(fp):
    here = fp.tell()
    fp.seek(0)
    h = fp.read(4)
    fp.seek(here)
    return len(h) == 4 and struct.unpack("<I", h)[0] == SPARSE_MAGIC

def sparse_to_raw(infile: Path) -> Path:
    with open(infile, "rb") as f:
        hdr = f.read(28)
        magic, maj, minv, fh_sz, ch_sz, blk_sz, total_blks, total_chunks, _ = struct.unpack("<IHHHHIIII", hdr)
        if magic != SPARSE_MAGIC:
            raise ValueError("Not a sparse image")

        raw_path = Path(tempfile.mkstemp(prefix="super_raw_", suffix=".img")[1])
        with open(raw_path, "wb") as out:
            out.truncate(blk_sz * total_blks)
            write_off = 0
            for _ in range(total_chunks):
                chdr = f.read(ch_sz)
                ctype, _, cblocks, _ = struct.unpack("<HHII", chdr)
                if ctype == CHUNK_RAW:
                    to_copy = cblocks * blk_sz
                    payload = f.read(to_copy)
                    out.seek(write_off)
                    out.write(payload)
                    write_off += to_copy
                elif ctype == CHUNK_FILL:
                    pat = struct.unpack("<I", f.read(4))[0]
                    buf = pat.to_bytes(4, "little") * (blk_sz // 4)
                    out.seek(write_off)
                    for _ in range(cblocks):
                        out.write(buf)
                    write_off += cblocks * blk_sz
                elif ctype == CHUNK_DONTCARE:
                    write_off += cblocks * blk_sz
                elif ctype == CHUNK_CRC32:
                    f.read(4)
        return raw_path

# ---------------------------------------------------------------------
# Struct definitions
# ---------------------------------------------------------------------
PARTITION_STRUCT_V10 = struct.Struct("<36sIIII")   # 52 bytes
EXTENT_STRUCT_V10    = struct.Struct("<QIQI")      # 24 bytes
PARTITION_STRUCT_V9  = struct.Struct("<IIII")      # 16 bytes
EXTENT_STRUCT_V9     = struct.Struct("<QIQI")      # 24 bytes
EXTENT_STRUCT_V7     = struct.Struct("<QI")        # 12 bytes (older)
GROUP_STRUCT_V9      = struct.Struct("<36sIQ")

HEADER_PREFIX_STRUCT = struct.Struct("<IHHIIII")
TD_STRUCT = struct.Struct("<III")

# ---------------------------------------------------------------------
# Read geometry
# ---------------------------------------------------------------------
def _read_geometry(f):
    f.seek(0)
    data = f.read(4096)
    struct_size, = struct.unpack_from("<I", data, 4)
    metadata_max_size, = struct.unpack_from("<I", data, 12)
    slot_count, = struct.unpack_from("<I", data, 16)
    if struct_size == 0:
        struct_size = 4096
    return struct_size, metadata_max_size, slot_count

# ---------------------------------------------------------------------
# Locate metadata header
# ---------------------------------------------------------------------
def _find_header(f, geom_sz):
    # Try common offsets
    for base in (0x3000, 0x10000, 0x13000, 0x20000, 0x23000, 0x33000, 0x43000, 0x53000):
        f.seek(base)
        data = f.read(4)
        if len(data)==4 and struct.unpack("<I", data)[0] == LP_HEADER_MAGIC:
            print(f"Found metadata header at 0x{base:X}")
            return base
    raise ValueError("LpMetadataHeader not found at known offsets")

# ---------------------------------------------------------------------
# Parse metadata tables
# ---------------------------------------------------------------------
def _read_header_and_tables(f, header_off):
    f.seek(header_off)
    hdr_data = f.read(0x200)
    magic, maj, minv, hdr_sz = struct.unpack_from("<IHHI", hdr_data, 0)
    if magic != LP_HEADER_MAGIC:
        raise ValueError(f"Bad magic at 0x{header_off:X}")

    # read table descriptors
    f.seek(header_off + 0x50)
    td_blob = f.read(48)
    (
        part_off, part_cnt, part_es,
        ext_off, ext_cnt, ext_es,
        grp_off, grp_cnt, grp_es,
        bdev_off, bdev_cnt, bdev_es,
    ) = struct.unpack("<IIIIIIIIIIII", td_blob)

    # vendor detection
    vendor = "Unknown"
    if bdev_es == 108: vendor = "AOSP/Generic"
    elif bdev_es == 64: vendor = "Amlogic"
    elif bdev_es == 72: vendor = "Allwinner"
    elif bdev_es == 80: vendor = "Rockchip"
    else: vendor = f"Unknown({bdev_es})"

    print(f"Vendor = {vendor}")

    # read tables blob
    tables_start = header_off + hdr_sz
    f.seek(tables_start)
    tables = f.read(0x10000)

    # select structs
    if part_es == 52:
        PS = PARTITION_STRUCT_V10
    elif part_es == 48:
        PS = PARTITION_STRUCT_V9
    else:
        PS = PARTITION_STRUCT_V10

    if ext_es == 24:
        ES = EXTENT_STRUCT_V10
    elif ext_es in (12, 8):
        ES = EXTENT_STRUCT_V7
    else:
        ES = EXTENT_STRUCT_V10

    GS = GROUP_STRUCT_V9

    def parse_table(td_off, td_cnt, td_es, struct_obj):
        out = []
        for i in range(td_cnt):
            base = td_off + i * td_es
            if base + td_es > len(tables):
                break
            try:
                out.append(struct_obj.unpack_from(tables, base))
            except struct.error:
                break
        return out

    parts = parse_table(part_off, part_cnt, part_es, PS)
    exts  = parse_table(ext_off, ext_cnt, ext_es, ES)
    grps  = parse_table(grp_off, grp_cnt, grp_es, GS)

    print(f"Parsed {len(parts)} partitions, {len(exts)} extents.")

    return {
        "maj": maj,
        "min": minv,
        "vendor": vendor,
        "parts": parts,
        "exts": exts,
        "grps": grps,
        "bdev_es": bdev_es,
    }

def _cstr(b): return b.split(b"\x00",1)[0].decode("ascii","ignore")

# ---------------------------------------------------------------------
# Write config
# ---------------------------------------------------------------------
def write_cfg(meta, out_dir, is_sparse=False):
    out_dir = Path(out_dir)
    cfg_dir = out_dir.parent / ".config"
    cfg_dir.mkdir(exist_ok=True, parents=True)

    cfg = configparser.ConfigParser()
    cfg.optionxform = str

    cfg["super_metadata"] = {
        "major_version": str(meta["maj"]),
        "minor_version": str(meta["min"]),
        "vendor": meta["vendor"],
        "partition_count": str(len(meta["parts"])),
        "extent_count": str(len(meta["exts"])),
        "is_sparse": "true" if is_sparse else "false",
    }

    for i, p in enumerate(meta["parts"]):
        sec = f"partition.{i}"
        if isinstance(p[0], (bytes, bytearray)):
            name = _cstr(p[0])
        else:
            name = f"partition_{i}"

        cfg[sec] = {
            "name": name,
            "first_extent": str(int(p[2]) if len(p) > 2 else 0),
            "num_extents": str(int(p[3]) if len(p) > 3 else 0),
            "group_index": str(int(p[4]) if len(p) > 4 else 0),
        }

    cfg_path = cfg_dir/"super_config"
    with open(cfg_path,"w",encoding="utf-8") as fp:
        cfg.write(fp)

    print(f"Config saved to {cfg_path}")
    return cfg_path

# ---------------------------------------------------------------------
# Extraction logic
# ---------------------------------------------------------------------
def lpunpack(super_img: Path, out_dir: Path, only=None):
    only = set(only or [])
    raw_path = None
    success = False

    try:
        with open(super_img, "rb") as sf:
            if _is_sparse(sf):
                print("Sparse image detected. Converting...")
                raw_path = sparse_to_raw(super_img)
                in_path = raw_path
                is_sparse = True
            else:
                in_path = super_img
                is_sparse = False

        with open(in_path, "rb") as f:
            geom_sz, _, _ = _read_geometry(f)
            header_off = _find_header(f, geom_sz)
            meta = _read_header_and_tables(f, header_off)

            write_cfg(meta, out_dir, is_sparse=is_sparse)

            parts = meta["parts"]
            exts  = meta["exts"]

            out_dir.mkdir(parents=True, exist_ok=True)

            # extraction
            for idx, part in enumerate(parts):
                if len(part)==5:
                    nm_raw, attrs, first_ext, num_ext, grp_idx = part
                    name = _cstr(nm_raw)
                else:
                    name = f"partition_{idx}"
                    attrs, first_ext, num_ext, grp_idx = part

                if only and name not in only:
                    continue

                outp = out_dir / f"{name}.img"
                print(f"Writing {outp}")

                with open(outp,"wb") as out:
                    for e_i in range(first_ext, first_ext + num_ext):
                        e = exts[e_i]

                        if len(e)==2:
                            num_sec, target_data = e
                            src_off = target_data * LP_SECTOR_SIZE
                        else:
                            num_sec, t_type, t_data, t_src = e
                            if t_type == LP_TARGET_TYPE_ZERO:
                                out.seek(out.tell() + num_sec*LP_SECTOR_SIZE)
                                continue
                            if t_type != LP_TARGET_TYPE_LINEAR:
                                raise ValueError(f"Unsupported extent type {t_type}")
                            src_off = t_data * LP_SECTOR_SIZE

                        f.seek(src_off)
                        to_copy = num_sec * LP_SECTOR_SIZE

                        while to_copy > 0:
                            buf = f.read(min(1024*1024, to_copy))
                            if not buf:
                                raise EOFError("Unexpected EOF")
                            out.write(buf)
                            to_copy -= len(buf)

        success = True

    except Exception as e:
        print(f"Extraction failed: {e}")

    finally:
        if raw_path and raw_path.exists():
            try: os.remove(raw_path)
            except: pass

        print("Extraction completed successfully." if success else "Extraction failed.")

# ---------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------
def main():
    ap = argparse.ArgumentParser(description="Extract partitions from an Android super.img")
    ap.add_argument("SUPER_IMG", type=Path)
    ap.add_argument("OUT_DIR", type=Path)
    ap.add_argument("-p","--partition", action="append", default=[], help="Only extract this partition")
    args = ap.parse_args()
    lpunpack(args.SUPER_IMG, args.OUT_DIR, args.partition)

if __name__=="__main__":
    main()
