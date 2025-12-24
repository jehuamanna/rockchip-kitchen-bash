#!/usr/bin/env python3
"""
fw_detect.py

Detect whether a firmware image is:
  - Amlogic (ampack-style burning image)
  - Allwinner IMAGEWTY (Phoenix/LiveSuit)
  - Rockchip (RKAF / RKFW / RKFP)
  - or Unknown

Usage:
  python fw_detect.py firmware.img
"""

import os
import struct
import sys
from typing import Literal, Optional

FirmwareType = Literal["Amlogic", "Allwinner", "Rockchip", "Unknown"]

# --- Constants copied from your tools ---

# Amlogic
AMLOGIC_MAGIC = 0x27B51956
AMLOGIC_HEADER_SIZE = 64
AMLOGIC_VALID_VERSIONS = (1, 2)

# Allwinner IMAGEWTY
IMAGEWTY_SIGNATURE = b"IMAGEWTY"
IMAGEWTY_HEADER_SIZE_EXPECTED = 96
IMAGEWTY_HEADER_V3 = 0x00000300
IMAGEWTY_HEADER_V4 = 0x00000403
IMAGEWTY_FORMAT_VERSION_EXPECTED = 0x00100234  # 0x100234

# Rockchip
ROCKCHIP_SIGNATURES = (b"RKAF", b"RKFW", b"RKFP")


# --- detection helpers ---

def read_exact(buf: bytes, offset: int, length: int) -> Optional[bytes]:
    if offset + length > len(buf):
        return None
    return buf[offset:offset + length]


def detect_rockchip(buf: bytes) -> bool:
    """
    Rockchip formats:
      - RKAF, RKFW, RKFP
      Signature is first 4 bytes.
    """
    sig = read_exact(buf, 0, 4)
    if not sig:
        return False
    return sig in ROCKCHIP_SIGNATURES


def detect_allwinner(buf: bytes, file_size: int) -> bool:
    """
    Allwinner IMAGEWTY format:
      - First 32 bytes structured as: "<8sLL4xLL4x"
      - Signature == b"IMAGEWTY"
      - header_version in {0x00000300, 0x00000403}
      - header_size == 96
      - format_version == 0x00100234
      - image_size <= file_size
    """
    hdr = read_exact(buf, 0, 32 + 64)  # full 96-byte header
    if hdr is None or len(hdr) < 32:
        return False

    hdr1 = hdr[:32]
    try:
        signature, header_ver, header_size, fmt_ver, img_size = struct.unpack(
            "<8sLL4xLL4x", hdr1
        )
    except struct.error:
        return False

    if signature != IMAGEWTY_SIGNATURE:
        return False

    if header_ver not in (IMAGEWTY_HEADER_V3, IMAGEWTY_HEADER_V4):
        return False

    if header_size != IMAGEWTY_HEADER_SIZE_EXPECTED:
        return False

    if fmt_ver != IMAGEWTY_FORMAT_VERSION_EXPECTED:
        return False

    if img_size == 0 or img_size > file_size:
        return False

    return True


def detect_amlogic(buf: bytes, file_size: int) -> bool:
    """
    Amlogic burning image (ampack style):
      - First 64 bytes are RawImageHead:
        "<IIIQII36s"
        crc (u32), version (u32), magic (u32), image_size (u64),
        item_align_size (u32), item_count (u32), reserve[36]
      - magic == 0x27B51956
      - version in {1,2}
      - image_size <= file_size
      - item_align_size is a reasonable small positive integer (e.g., <= 0x1000)
    """
    hdr_bytes = read_exact(buf, 0, AMLOGIC_HEADER_SIZE)
    if hdr_bytes is None or len(hdr_bytes) != AMLOGIC_HEADER_SIZE:
        return False

    try:
        crc, version, magic, image_size, item_align_size, item_count, reserve = struct.unpack(
            "<IIIQII36s", hdr_bytes
        )
    except struct.error:
        return False

    if magic != AMLOGIC_MAGIC:
        return False

    if version not in AMLOGIC_VALID_VERSIONS:
        return False

    if image_size == 0 or image_size > file_size:
        return False

    # Simple sanity checks to avoid random matches
    if item_align_size == 0 or item_align_size > 0x10000:
        return False

    if item_count == 0 or item_count > 0x10000:
        return False

    return True


def detect_firmware_type(path: str) -> FirmwareType:
    if not os.path.isfile(path):
        raise FileNotFoundError(f"Input file does not exist: {path}")

    file_size = os.path.getsize(path)
    # Read enough bytes for all header checks (4 KB is plenty)
    with open(path, "rb") as f:
        buf = f.read(4096)

    if len(buf) < 4:
        return "Unknown"

    # Order: Rockchip (simple magic), Allwinner (IMAGEWTY), Amlogic (binary struct)
    if detect_rockchip(buf):
        return "Rockchip"

    if detect_allwinner(buf, file_size):
        return "Allwinner"

    if detect_amlogic(buf, file_size):
        return "Amlogic"

    return "Unknown"


def main(argv=None):
    if argv is None:
        argv = sys.argv[1:]

    if not argv or len(argv) != 1:
        print(f"Usage: {os.path.basename(sys.argv[0])} <firmware.img>", file=sys.stderr)
        sys.exit(1)

    path = argv[0]
    try:
        fw_type = detect_firmware_type(path)
    except Exception as e:
        print(f"Error while detecting firmware type: {e}", file=sys.stderr)
        sys.exit(1)

    print(fw_type)


if __name__ == "__main__":
    main()
