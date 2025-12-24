#!/usr/bin/env python3
"""
awunpack.py

Inspect, extract and repack Allwinner IMAGEWTY firmware images (Phoenix/LiveSuit style).

Usage:
  python awunpack.py info <image.img>
  python awunpack.py list <image.img>
  python awunpack.py extract <image.img> [output_dir]
  python awunpack.py repack <extracted_dir> <new_image.img>

This script:
  - Validates IMAGEWTY header
  - Parses per-file headers
  - Lists files with offsets and sizes
  - Extracts all files (flattened into a directory) and generates a basic image.cfg
  - Rebuilds a new IMAGEWTY image using only image.cfg + files in a directory
"""

import argparse
import os
import re
import struct
import zlib
from dataclasses import dataclass
from typing import BinaryIO, List, Optional


IMAGEWTY_SIGNATURE = b"IMAGEWTY"
HEADER_SIZE_EXPECTED = 96
HEADER_V3 = 0x00000300
HEADER_V4 = 0x00000403  # supported but treated like v3 for now
FORMAT_VERSION_EXPECTED = 0x00100234  # 0x100234

# Data alignment for file payloads and offsets (observed: 0x400)
DATA_ALIGN = 1024
DEFAULT_FILE_HEADER_SIZE = 1024
DEFAULT_NAME_FIELD_LEN = 256


@dataclass
class ImageWTYHeader:
    signature: str
    header_version: int
    header_size: int
    format_version: int
    image_size: int
    file_count: int

    def __str__(self) -> str:
        return (
            f"IMAGEWTY Header:\n"
            f"  Signature      : {self.signature}\n"
            f"  Header version : 0x{self.header_version:08X}\n"
            f"  Header size    : {self.header_size} bytes\n"
            f"  Format version : 0x{self.format_version:08X}\n"
            f"  Image size     : {self.image_size} bytes\n"
            f"  File count     : {self.file_count}"
        )


@dataclass
class ImageWTYFileEntry:
    index: int
    main_type: str
    sub_type: str
    name: str
    name_field_length: int
    header_size: int
    stored_length: int  # size including padding in the image
    original_length: int  # real file size (without padding)
    offset: int
    unknown: int
    pad1: int
    pad2: int

    def __str__(self) -> str:
        return (
            f"[{self.index:02d}] {self.name}\n"
            f"  Main type      : {self.main_type}\n"
            f"  Sub type       : {self.sub_type}\n"
            f"  Stored length  : {self.stored_length} bytes\n"
            f"  Original length: {self.original_length} bytes\n"
            f"  Offset         : 0x{self.offset:X} ({self.offset} bytes)\n"
        )


class ImageWTYImage:
    """
    Parser and writer for Allwinner IMAGEWTY images.

    Layout (simplified):
      - Global header (96 bytes, version 0x300 / 0x403)
      - Padding up to offset 0x400 (1024 bytes from start)
      - N file headers (each usually 1024 bytes)
      - File data blobs (uncompressed, aligned to 1024 bytes)
    """

    def __init__(self) -> None:
        self.header: Optional[ImageWTYHeader] = None
        self.files: List[ImageWTYFileEntry] = []
        self._fp: Optional[BinaryIO] = None
        self._hdr2_raw: Optional[bytes] = None  # second half of main header (64 bytes)

    # ---------- low-level helpers ----------

    def _open(self, path: str) -> BinaryIO:
        if self._fp:
            self._fp.close()
        self._fp = open(path, "rb")
        return self._fp

    def close(self) -> None:
        if self._fp:
            self._fp.close()
            self._fp = None

    @staticmethod
    def _align(value: int, align: int = DATA_ALIGN) -> int:
        if align <= 1:
            return value
        return (value + align - 1) // align * align

    # ---------- parsing from a .img ----------

    def parse(self, path: str) -> None:
        """
        Parse an existing IMAGEWTY .img file (for info/list/extract).
        """
        fp = self._open(path)

        # Validate file size
        fp.seek(0, os.SEEK_END)
        size = fp.tell()
        fp.seek(0, os.SEEK_SET)
        if size < HEADER_SIZE_EXPECTED:
            raise ValueError("File too short to be a valid IMAGEWTY image")

        # First 32 bytes of header
        # <8sLL4xLL4x ==
        #   signature[8], header_version, header_size, skip 4,
        #   format_version, image_size, skip 4
        hdr1 = fp.read(32)
        if len(hdr1) != 32:
            raise ValueError("Failed to read global header (part 1)")

        signature, header_ver, header_size, fmt_ver, img_size = struct.unpack(
            "<8sLL4xLL4x", hdr1
        )

        if signature != IMAGEWTY_SIGNATURE:
            raise ValueError("Not an IMAGEWTY image (wrong magic)")

        if header_ver not in (HEADER_V3, HEADER_V4):
            raise ValueError(f"Unsupported header version: 0x{header_ver:X}")

        if header_size != HEADER_SIZE_EXPECTED:
            raise ValueError(
            f"Unexpected header size: {header_size}, expected {HEADER_SIZE_EXPECTED}"
            )

        if fmt_ver != FORMAT_VERSION_EXPECTED:
            raise ValueError(f"Unexpected format version: 0x{fmt_ver:X}")

        if size < img_size:
            # Some images have img_size == file size, but we allow slightly larger file size.
            raise ValueError(
                f"Image file too short: actual {size}, header says {img_size}"
            )

        # Remaining 64 bytes of header; we only decode file count, but keep raw bytes
        hdr2 = fp.read(64)
        if len(hdr2) != 64:
            raise ValueError("Failed to read global header (part 2)")

        self._hdr2_raw = hdr2

        # v3/v4 format: file count is a uint32 at offset 28 of this 64-byte block
        (file_count,) = struct.unpack("<28xL32x", hdr2)

        self.header = ImageWTYHeader(
            signature=signature.decode("ascii", "ignore"),
            header_version=header_ver,
            header_size=header_size,
            format_version=fmt_ver,
            image_size=img_size,
            file_count=file_count,
        )

        # File headers start at offset 1024 (0x400)
        fp.seek(1024, os.SEEK_SET)

        self.files = []
        for i in range(file_count):
            entry = self._parse_file_header(fp, i)
            self.files.append(entry)

    def _parse_file_header(self, fp: BinaryIO, index: int) -> ImageWTYFileEntry:
        # First 32 bytes of file header:
        #   - name length (fn_len)
        #   - total header size (hdr_size, usually 1024)
        #   - main type (8 chars)
        #   - sub type (16 chars)
        hdr1 = fp.read(32)
        if len(hdr1) != 32:
            raise ValueError(f"Failed to read file header {index} (part 1)")

        fn_len, hdr_size, main_raw, sub_raw = struct.unpack("<LL8s16s", hdr1)
        main = main_raw.rstrip(b"\0").decode("ascii", "ignore")
        sub = sub_raw.rstrip(b"\0").decode("ascii", "ignore")

        # Next (24 + fn_len) bytes:
        #   unknown (4)
        #   filename (fn_len bytes, padded with zeros)
        #   pad1 (4)
        #   original_length (4)
        #   stored_length (4)
        #   pad2 (4)
        #   offset (4)
        # Total: 4 + fn_len + 4 + 4 + 4 + 4 + 4 = 24 + fn_len
        hdr2_size = 24 + fn_len
        hdr2 = fp.read(hdr2_size)
        if len(hdr2) != hdr2_size:
            raise ValueError(f"Failed to read file header {index} (part 2)")

        unknown = struct.unpack("<L", hdr2[0:4])[0]
        raw_name = hdr2[4 : 4 + fn_len]
        name = raw_name.split(b"\0", 1)[0].decode("ascii", "ignore")

        after_name_off = 4 + fn_len
        pad1, original_length = struct.unpack(
            "<LL", hdr2[after_name_off : after_name_off + 8]
        )
        stored_length, pad2, offset = struct.unpack(
            "<LLL", hdr2[after_name_off + 8 : after_name_off + 8 + 12]
        )

        # Skip the rest of this file header (up to hdr_size)
        bytes_read = 32 + hdr2_size
        remaining = hdr_size - bytes_read
        if remaining < 0:
            raise ValueError(
                f"Invalid header size for file {index}: hdr_size={hdr_size}, "
                f"already read {bytes_read} bytes"
            )
        if remaining > 0:
            fp.seek(remaining, os.SEEK_CUR)

        return ImageWTYFileEntry(
            index=index,
            main_type=main,
            sub_type=sub,
            name=name,
            name_field_length=fn_len,
            header_size=hdr_size,
            stored_length=stored_length,
            original_length=original_length,
            offset=offset,
            unknown=unknown,
            pad1=pad1,
            pad2=pad2,
        )

    # ---------- parsing from image.cfg for repack ----------

    @classmethod
    def from_cfg(cls, cfg_dir: str) -> "ImageWTYImage":
        """
        Build an in-memory IMAGEWTY image definition from image.cfg
        (no original .img needed).
        """
        cfg_path = os.path.join(cfg_dir, "image.cfg")
        if not os.path.isfile(cfg_path):
            raise FileNotFoundError(f"image.cfg not found in {cfg_dir}")

        filename_re = re.compile(r'filename\s*=\s*"([^"]+)"')
        maintype_re = re.compile(r'maintype\s*=\s*"([^"]*)"')
        subtype_re = re.compile(r'subtype\s*=\s*"([^"]*)"')

        entries: List[ImageWTYFileEntry] = []
        with open(cfg_path, "r", encoding="utf-8", errors="ignore") as f:
            for line in f:
                if "filename" not in line:
                    continue
                m_fn = filename_re.search(line)
                if not m_fn:
                    continue
                filename = m_fn.group(1)

                m_mt = maintype_re.search(line)
                m_st = subtype_re.search(line)
                main_type = m_mt.group(1) if m_mt else ""
                sub_type = m_st.group(1) if m_st else ""

                idx = len(entries)
                e = ImageWTYFileEntry(
                    index=idx,
                    main_type=main_type,
                    sub_type=sub_type,
                    name=filename,
                    name_field_length=DEFAULT_NAME_FIELD_LEN,
                    header_size=DEFAULT_FILE_HEADER_SIZE,
                    stored_length=0,
                    original_length=0,
                    offset=0,
                    unknown=0,
                    pad1=0,
                    pad2=0,
                )
                entries.append(e)

        if not entries:
            raise ValueError("No file entries found in image.cfg")

        file_count = len(entries)
        # Build a synthetic second header block: 64 bytes, with file_count at offset 28.
        hdr2 = bytearray(64)
        struct.pack_into("<L", hdr2, 28, file_count)

        img = cls()
        img.header = ImageWTYHeader(
            signature=IMAGEWTY_SIGNATURE.decode("ascii"),
            header_version=HEADER_V3,
            header_size=HEADER_SIZE_EXPECTED,
            format_version=FORMAT_VERSION_EXPECTED,
            image_size=0,
            file_count=file_count,
        )
        img.files = entries
        img._hdr2_raw = bytes(hdr2)
        return img

    # ---------- data access ----------

    def read_file_data(self, entry: ImageWTYFileEntry) -> bytes:
        if not self._fp:
            raise RuntimeError("Image not opened")
        self._fp.seek(entry.offset, os.SEEK_SET)
        data = self._fp.read(entry.stored_length)
        if entry.original_length and entry.original_length <= len(data):
            data = data[: entry.original_length]
        return data

    # ---------- high-level operations for parsed .img ----------

    def print_info(self) -> None:
        if not self.header:
            raise RuntimeError("Image not parsed")

        print(self.header)
        print()
        print("Files:")
        for entry in self.files:
            print(entry)

    def print_list(self) -> None:
        if not self.header:
            raise RuntimeError("Image not parsed")

        print(f"IMAGEWTY: {self.header.signature}, {self.header.file_count} files")
        print(
            f"{'Idx':>3} {'Offset':>12} {'Size':>12}  {'Main':<12} {'Sub':<20} Name"
        )
        print("-" * 80)
        for e in self.files:
            print(
                f"{e.index:3d}  0x{e.offset:08X} {e.stored_length:12d}  "
                f"{e.main_type:<12} {e.sub_type:<20} {e.name}"
            )

    def extract_all(self, out_dir: str) -> None:
        """
        Extract all files from a parsed .img into out_dir, flattened:
          - Any path components in the archive (e.g. 'imgRePacker/sys_config.fex')
            are stripped; only the basename is used ('sys_config.fex').
        Also generates a simple image.cfg listing filename/maintype/subtype.
        """
        if not self.header:
            raise RuntimeError("Image not parsed")

        os.makedirs(out_dir, exist_ok=True)

        for e in self.files:
            data = self.read_file_data(e)

            # Flatten name: only keep the final component
            name_in_img = e.name or f"file_{e.index:02d}.bin"
            name_in_img = name_in_img.replace("\\", "/")
            safe_name = os.path.basename(name_in_img) or f"file_{e.index:02d}.bin"

            out_path = os.path.join(out_dir, safe_name)

            # Avoid overwriting if multiple entries share the same basename
            base, ext = os.path.splitext(out_path)
            suffix = 1
            while os.path.exists(out_path):
                out_path = f"{base}_{suffix}{ext}"
                suffix += 1

            with open(out_path, "wb") as of:
                of.write(data)

        # Generate a simple image.cfg-like file that lists all entries.
        cfg_path = os.path.join(out_dir, "image.cfg")
        with open(cfg_path, "w", encoding="utf-8") as cf:
            cf.write("// Auto-generated by awunpack.py\n")
            cf.write("// Basic mapping of maintype/subtype to filenames\n\n")
            for e in self.files:
                cf.write(
                    '{filename = "%s", maintype = "%s", subtype = "%s",},\n'
                    % (e.name, e.main_type, e.sub_type)
                )

    # ---------- helpers for repack-from-cfg ----------

    def _collect_file_data_from_dir(self, extracted_dir: str) -> List[bytes]:
        """
        For each file entry defined in image.cfg, load the corresponding file
        from extracted_dir by matching its basename.
        """
        datas: List[bytes] = []

        for e in self.files:
            name_in_cfg = e.name or f"file_{e.index:02d}.bin"
            name_in_cfg = name_in_cfg.replace("\\", "/")
            base_name = os.path.basename(name_in_cfg) or f"file_{e.index:02d}.bin"
            path = os.path.join(extracted_dir, base_name)
            if not os.path.isfile(path):
                raise FileNotFoundError(
                    f"Required file not found for repack: {path}"
                )
            with open(path, "rb") as f:
                datas.append(f.read())

        return datas

    def _update_v_files_crc(self, datas: List[bytes]) -> None:
        """
        For each V*.fex entry, replace its data with a 4-byte CRC32 of the
        matching non-V file (e.g., Vboot.fex -> CRC32(boot.fex)).
        This mutates the datas list in place.
        """
        name_to_index = {e.name: i for i, e in enumerate(self.files)}

        for i, e in enumerate(self.files):
            if not e.name:
                continue
            if not (e.name.startswith("V") and e.name.endswith(".fex")):
                continue

            base_name = e.name[1:]  # drop leading 'V'
            j = name_to_index.get(base_name)
            if j is None:
                # No matching non-V file; leave as-is
                continue

            base_data = datas[j]
            crc = zlib.crc32(base_data) & 0xFFFFFFFF
            datas[i] = struct.pack("<I", crc)  # 4-byte little-endian

    def repack_from_cfg(self, extracted_dir: str, out_image_path: str) -> None:
        """
        Rebuild a new IMAGEWTY image using:
          - Header + file list constructed from image.cfg
          - File data from extracted_dir (all must be present)
          - Automatically updated V-files (4-byte CRC32)
        """
        if not self.header or self._hdr2_raw is None:
            raise RuntimeError("Image header not initialised from cfg")

        # Step 1: collect file data from directory
        datas = self._collect_file_data_from_dir(extracted_dir)

        # Step 2: recalc V*.fex contents
        self._update_v_files_crc(datas)

        # Step 3: compute new layout
        headers_start = 1024
        headers_total_size = sum(e.header_size for e in self.files)
        data_start = headers_start + headers_total_size

        # Assign new offsets + lengths, aligned to DATA_ALIGN
        current_offset = data_start
        for e, data in zip(self.files, datas):
            current_offset = self._align(current_offset, DATA_ALIGN)
            e.offset = current_offset
            e.original_length = len(data)
            e.stored_length = self._align(len(data), DATA_ALIGN)
            current_offset += e.stored_length

        total_image_size = current_offset

        # Step 4: write new image
        with open(out_image_path, "wb") as out:
            # 4a: main header
            hdr1 = struct.pack(
                "<8sLL4xLL4x",
                IMAGEWTY_SIGNATURE,
                self.header.header_version,
                self.header.header_size,
                self.header.format_version,
                total_image_size,
            )
            out.write(hdr1)
            out.write(self._hdr2_raw)

            # Pad up to 1024 bytes (start of file headers)
            pad_len = headers_start - out.tell()
            if pad_len < 0:
                raise RuntimeError("Header overlap: headers_start is before current position")
            if pad_len:
                out.write(b"\0" * pad_len)

            # 4b: file headers
            for e in self.files:
                # Build hdr1 for file
                fn_len = e.name_field_length
                hdr_size = e.header_size

                main_raw = e.main_type.encode("ascii", "ignore")[:8].ljust(8, b"\0")
                sub_raw = e.sub_type.encode("ascii", "ignore")[:16].ljust(16, b"\0")

                fhdr1 = struct.pack("<LL8s16s", fn_len, hdr_size, main_raw, sub_raw)

                # Filename field (fn_len bytes)
                filename = (e.name or f"file_{e.index:02d}.bin").encode(
                    "ascii", "ignore"
                )
                if len(filename) >= fn_len:
                    filename = filename[: fn_len - 1]
                filename_field = filename + b"\0" * (fn_len - len(filename))

                # Build hdr2
                fhdr2 = struct.pack("<L", e.unknown)
                fhdr2 += filename_field
                fhdr2 += struct.pack(
                    "<LLLLL",
                    e.pad1,
                    e.original_length,
                    e.stored_length,
                    e.pad2,
                    e.offset,
                )

                # Pad header to hdr_size
                header_bytes = fhdr1 + fhdr2
                if len(header_bytes) > hdr_size:
                    raise RuntimeError(
                        f"File header {e.index} exceeds hdr_size "
                        f"({len(header_bytes)} > {hdr_size})"
                    )
                if len(header_bytes) < hdr_size:
                    header_bytes += b"\0" * (hdr_size - len(header_bytes))

                out.write(header_bytes)

            # 4c: file data
            for e, data in zip(self.files, datas):
                # Ensure we're at correct offset (or pad if needed)
                pos = out.tell()
                if pos < e.offset:
                    out.write(b"\0" * (e.offset - pos))
                elif pos > e.offset:
                    raise RuntimeError(
                        f"Data offset mismatch for {e.name}: pos={pos}, expected={e.offset}"
                    )

                out.write(data)
                pad = e.stored_length - len(data)
                if pad > 0:
                    out.write(b"\0" * pad)

        # Not strictly necessary, but nice sanity check
        final_size = os.path.getsize(out_image_path)
        if final_size != total_image_size:
            print(
                f"Warning: final image size {final_size} "
                f"differs from computed {total_image_size}"
            )


# ---------- CLI command handlers ----------


def cmd_info(args: argparse.Namespace) -> None:
    img = ImageWTYImage()
    try:
        img.parse(args.image)
        img.print_info()
    finally:
        img.close()


def cmd_list(args: argparse.Namespace) -> None:
    img = ImageWTYImage()
    try:
        img.parse(args.image)
        img.print_list()
    finally:
        img.close()


def cmd_extract(args: argparse.Namespace) -> None:
    img = ImageWTYImage()
    try:
        img.parse(args.image)

        if args.output:
            out_dir = args.output
        else:
            # Default: <image_basename>_unpacked
            base = os.path.basename(args.image)
            root, _ = os.path.splitext(base)
            out_dir = root + "_unpacked"

        print(f"Extracting {img.header.file_count} files to '{out_dir}' ...")
        img.extract_all(out_dir)
        print("Done.")
        print(f"Files and image.cfg written to: {os.path.abspath(out_dir)}")
    finally:
        img.close()


def cmd_repack(args: argparse.Namespace) -> None:
    """
    Usage: repack <extracted_dir> <new_image.img>

    Requires:
      - extracted_dir/image.cfg  (created by this tool or imgrepacker)
      - all files referenced in image.cfg present in extracted_dir
    """
    if not os.path.isdir(args.extracted_dir):
        raise SystemExit(f"Extracted directory not found: {args.extracted_dir}")

    img = ImageWTYImage.from_cfg(args.extracted_dir)
    print(
        f"Repacking from '{args.extracted_dir}' using image.cfg "
        f"into '{args.output_image}' ..."
    )
    img.repack_from_cfg(args.extracted_dir, args.output_image)
    print("Done.")
    print(f"New image written to: {os.path.abspath(args.output_image)}")


# ---------- argparse setup ----------


def build_argparser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description=(
            "IMAGEWTY Tool (Python): inspect, extract, and repack "
            "Allwinner IMAGEWTY images."
        )
    )

    sub = parser.add_subparsers(dest="command", required=True)

    p_info = sub.add_parser("info", help="Show detailed header and file information")
    p_info.add_argument("image", help="Path to IMAGEWTY .img file")
    p_info.set_defaults(func=cmd_info)

    p_list = sub.add_parser("list", help="List files in the image")
    p_list.add_argument("image", help="Path to IMAGEWTY .img file")
    p_list.set_defaults(func=cmd_list)

    p_extract = sub.add_parser(
        "extract",
        help="Extract all files and generate a basic image.cfg into the specified directory",
    )
    p_extract.add_argument("image", help="Path to IMAGEWTY .img file")
    p_extract.add_argument(
        "output",
        nargs="?",
        help="Output directory (default: <image_basename>_unpacked)",
    )
    p_extract.set_defaults(func=cmd_extract)

    p_repack = sub.add_parser(
        "repack",
        help=(
            "Repack files into a new IMAGEWTY image using only image.cfg "
            "and files in the directory.\n"
            "Usage: repack <extracted_dir> <new_image.img>"
        ),
    )
    p_repack.add_argument("extracted_dir", help="Directory with image.cfg and .fex files")
    p_repack.add_argument("output_image", help="Path for the new .img file")
    p_repack.set_defaults(func=cmd_repack)

    return parser


def main(argv: Optional[list] = None) -> None:
    parser = build_argparser()
    args = parser.parse_args(argv)
    args.func(args)


if __name__ == "__main__":
    main()
