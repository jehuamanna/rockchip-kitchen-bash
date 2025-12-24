#!/usr/bin/env python3
"""
Convert an Android sparse image (ED26FF3A) to raw form.

Usage:
    python sparse_to_raw.py input.img output.img
"""

import sys, struct, os

SPARSE_MAGIC = 0xED26FF3A
CHUNK_RAW      = 0xCAC1
CHUNK_FILL     = 0xCAC2
CHUNK_DONTCARE = 0xCAC3
CHUNK_CRC32    = 0xCAC4

def convert_sparse_to_raw(infile, outfile):
    with open(infile, "rb") as f:
        hdr = f.read(28)
        magic, maj, minv, fh_sz, ch_sz, blk_sz, total_blks, total_chunks, _ = struct.unpack("<IHHHHIIII", hdr)
        if magic != SPARSE_MAGIC:
            raise SystemExit("Not a sparse image (bad magic)")

        with open(outfile, "wb") as out:
            out.truncate(blk_sz * total_blks)
            write_off = 0
            for _ in range(total_chunks):
                chdr = f.read(ch_sz)
                if len(chdr) < ch_sz:
                    raise SystemExit("Corrupt sparse chunk header")
                ctype, _, cblocks, _ = struct.unpack("<HHII", chdr)

                if ctype == CHUNK_RAW:
                    data = f.read(cblocks * blk_sz)
                    out.seek(write_off)
                    out.write(data)
                    write_off += cblocks * blk_sz
                elif ctype == CHUNK_FILL:
                    pat = struct.unpack("<I", f.read(4))[0]
                    buf = pat.to_bytes(4, "little") * (blk_sz // 4)
                    out.seek(write_off)
                    for _b in range(cblocks):
                        out.write(buf)
                    write_off += cblocks * blk_sz
                elif ctype == CHUNK_DONTCARE:
                    write_off += cblocks * blk_sz
                elif ctype == CHUNK_CRC32:
                    f.read(4)
                else:
                    raise SystemExit(f"Unknown chunk type {hex(ctype)}")
    print(f"Wrote raw image: {outfile} (block size {blk_sz}, {total_blks} blocks)")

def main():
    if len(sys.argv) < 3:
        print("Usage: python sparse_to_raw.py input.img output.img")
        sys.exit(1)
    infile = sys.argv[1]
    outfile = sys.argv[2]

    if not os.path.exists(infile):
        print("File not found:", infile)
        sys.exit(1)

    convert_sparse_to_raw(infile, outfile)

if __name__ == "__main__":
    main()
