#!/usr/bin/env python3

import sys
import os
import zlib

DTB_MAGIC = b"\xD0\x0D\xFE\xED"
GZIP_MAGIC = b"\x1F\x8B"


def extract_first_gzip_member(raw: bytes):
    """
    Detect gzip and decompress the first gzip member,
    even if the gzip header occurs later in the file.
    Returns (processed_data, mode_string)
    """
    # gzip at start
    if raw.startswith(GZIP_MAGIC):
        try:
            d = zlib.decompressobj(16 + zlib.MAX_WBITS)
            out = d.decompress(raw)
            return out, "gunzip|split"
        except:
            return raw, "split"

    # gzip embedded
    idx = raw.find(GZIP_MAGIC)
    if idx != -1:
        try:
            d = zlib.decompressobj(16 + zlib.MAX_WBITS)
            out = d.decompress(raw[idx:])
            return out, "gunzip|split"
        except:
            return raw, "split"

    # no gzip found
    return raw, "split"


def extract_identifier(blob: bytes, start_index: int) -> str:
    """
    After the DTB magic, scan for Amlogic-style identifier.
    """
    segment = blob[start_index:start_index + 2048]
    text = segment.decode("latin-1", errors="ignore")
    parts = text.split("\x00")

    for p in parts:
        p = p.strip()
        if "_" in p and any(c.isdigit() for c in p) and len(p) >= 5:
            return p

    return None


def sanitize_name(name: str) -> str:
    """
    Sanitize filename to [A-Za-z0-9_-], drop invalid chars,
    remove leading underscores.
    """
    cleaned = "".join(c for c in name if (c.isalnum() or c in "_-"))

    while cleaned.startswith("_"):
        cleaned = cleaned[1:]

    return cleaned or "dtb"


def split_dtb_magic(input_file: str, output_folder: str, dtb_unpack_file: str = None):
    raw = open(input_file, "rb").read()
    blob, mode = extract_first_gzip_member(raw)

    size = len(blob)
    print(f"Processing {size} bytes")

    os.makedirs(output_folder, exist_ok=True)

    dtb_starts = []
    window = bytearray()
    idx = 0

    # Find magic positions
    while idx < size:
        window.append(blob[idx])
        if len(window) > 4:
            window.pop(0)

        if window == bytearray(DTB_MAGIC):
            magic_start = idx - 3  # correct offset for D0
            if magic_start >= 0:
                dtb_starts.append(magic_start)

        idx += 1

    if not dtb_starts:
        print("No DTB found")
        return

    for i, start in enumerate(dtb_starts):
        # End BEFORE next magic
        end = dtb_starts[i + 1] if i < len(dtb_starts) - 1 else size

        # Slice exactly, keep trailing nulls intact
        dtb = blob[start:end]

        name = extract_identifier(blob, start) or str(i + 1)
        name = sanitize_name(name)

        out_path = os.path.join(output_folder, f"{name}.dtb")
        with open(out_path, "wb") as f:
            f.write(dtb)

        print(f"Saved {out_path}")

    # Write unpack status
    if dtb_unpack_file:
        parent = os.path.dirname(os.path.abspath(dtb_unpack_file))
        if parent and not os.path.exists(parent):
            os.makedirs(parent, exist_ok=True)
        with open(dtb_unpack_file, "w") as f:
            f.write(mode)
    else:
        with open(os.path.join(output_folder, "dtb_unpack"), "w") as f:
            f.write(mode)

    print("Done.")


def main():
    if len(sys.argv) < 3:
        print("Usage:")
        print("  python split_dtb_magic_named.py <input.img> <output_folder> [dtb_unpack_file]")
        sys.exit(1)

    input_file = sys.argv[1]
    output_folder = sys.argv[2]
    dtb_unpack_file = sys.argv[3] if len(sys.argv) > 3 else None

    split_dtb_magic(input_file, output_folder, dtb_unpack_file)


if __name__ == "__main__":
    main()
