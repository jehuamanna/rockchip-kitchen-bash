#!/usr/bin/env python3
import os, sys, struct

SPARSE_MAGIC = 0xED26FF3A          # Android sparse image
EROFS_MAGIC  = 0xE0F5E1E2          # EROFS superblock magic (at offset 1024)
EROFS_SB_OFF = 1024

def read_u32_le(f):
    b = f.read(4)
    if len(b) != 4:
        return None
    return struct.unpack("<I", b)[0]

def is_sparse(f):
    f.seek(0)
    magic = read_u32_le(f)
    return magic == SPARSE_MAGIC

def is_erofs(f):
    # EROFS superblock begins at absolute offset 1024 bytes
    try:
        f.seek(EROFS_SB_OFF)
        magic = read_u32_le(f)
        return magic == EROFS_MAGIC
    except Exception:
        return False

def detect_image_type(path):
    if not os.path.isfile(path):
        return "file-not-found"
    size = os.path.getsize(path)
    if size < 4:
        return "unknown"

    with open(path, "rb") as f:
        if is_sparse(f):
            return "sparse"
        if size >= EROFS_SB_OFF + 4 and is_erofs(f):
            return "erofs"
    return "raw"

def main():
    if len(sys.argv) < 2:
        print("Usage: python detect_img_type.py <imagefile.img>")
        sys.exit(1)
    print(f"Detected type:{detect_image_type(sys.argv[1])}")

if __name__ == "__main__":
    main()