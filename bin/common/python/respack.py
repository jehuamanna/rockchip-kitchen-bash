#!/usr/bin/env python3
import sys
import os
import struct
import argparse
import binascii
import gzip
import zlib
from typing import Optional, Tuple

# =========================================
# CONSTANTS
# =========================================
AML_RES_IMG_ITEM_ALIGN_SZ = 16
AML_RES_IMG_V1_MAGIC = b"AML_RES!"
AML_RES_IMG_V1_MAGIC_LEN = 8
AML_RES_IMG_HEAD_SZ = AML_RES_IMG_ITEM_ALIGN_SZ * 4
AML_RES_IMG_VERSION_V2 = 0x02

IH_MAGIC = 0x27051956
IH_NMLEN = 32
ARCH_ARM = 8

# =========================================
# FORMAT DETECTION
# =========================================
def is_bmp(data: bytes) -> bool:
    return data.startswith(b"BM")

def is_gzip(data: bytes) -> bool:
    return data.startswith(b"\x1f\x8b")

def is_cpio_newc(data: bytes) -> bool:
    return data.startswith(b"070701")

def detect_format(data: bytes) -> str:
    """
    Enhanced deep format detection:
    bmp
    gzip_bmp
    gzip_cpio_bmp
    gzip_cpio_unknown
    gzip_unknown
    cpio_bmp
    cpio_unknown
    unknown
    """
    # Direct BMP
    if is_bmp(data):
        return "bmp"

    # Direct GZIP
    if is_gzip(data):
        try:
            unz = gzip.decompress(data)

            # inner BMP?
            if is_bmp(unz):
                return "gzip_bmp"

            # inner CPIO?
            if is_cpio_newc(unz):
                first = parse_cpio_newc_first_file(unz)
                if first:
                    _, payload = first
                    if is_bmp(payload):
                        return "gzip_cpio_bmp"
                    else:
                        return "gzip_cpio_unknown"
                return "gzip_cpio_unknown"

            # fallback
            return "gzip_unknown"

        except:
            return "gzip_unknown"

    # Direct CPIO
    if is_cpio_newc(data):
        first = parse_cpio_newc_first_file(data)
        if first:
            _, payload = first
            if is_bmp(payload):
                return "cpio_bmp"
            else:
                return "cpio_unknown"
        else:
            return "cpio_unknown"

    return "unknown"

# =========================================
# CPIO newc helpers
# =========================================
def _round_up_4(n):
    return (4 - (n % 4)) % 4

def parse_cpio_newc_first_file(blob: bytes) -> Optional[Tuple[str, bytes]]:
    off = 0
    while True:
        if off + 110 > len(blob):
            return None
        if blob[off:off+6] != b"070701":
            return None

        def rh(i):
            return int(blob[off+6+8*i:off+6+8*(i+1)], 16)

        try:
            namesize = rh(10)
            filesize = rh(5)
        except:
            return None

        off_hdr_end = off + 110
        off_hdr_end += _round_up_4(off_hdr_end)

        if off_hdr_end + namesize > len(blob):
            return None

        name = blob[off_hdr_end:off_hdr_end + namesize]
        name = name[:-1] if name.endswith(b"\x00") else name
        name_str = name.decode("ascii", "ignore")

        if name_str == "TRAILER!!!":
            return None

        off_name_end = off_hdr_end + namesize
        off_name_end += _round_up_4(off_name_end)

        if off_name_end + filesize > len(blob):
            return None

        filedata = blob[off_name_end:off_name_end + filesize]
        return name_str, filedata

def build_cpio_newc_single_file(name: str, data: bytes) -> bytes:
    import time
    name_bytes = name.encode("ascii", "ignore")
    namesize = len(name_bytes) + 1
    mtime = int(time.time())

    def hx(v): return f"{v:08X}".encode()

    header = (
        b"070701"
        + b"00000000"
        + hx(0o100644)
        + b"00000000"*2
        + hx(mtime)
        + hx(len(data))
        + b"00000000"*4
        + hx(namesize)
        + b"00000000"
    )

    out = bytearray()
    out += header
    out += b"\x00" * _round_up_4(len(out))
    out += name_bytes + b"\x00"
    out += b"\x00" * _round_up_4(len(out))
    out += data
    out += b"\x00" * _round_up_4(len(out))

    trailer = b"TRAILER!!!"
    tsize = len(trailer) + 1

    thdr = (
        b"070701"
        + b"00000000"*11
        + f"{tsize:08X}".encode()
        + b"00000000"
    )

    out += thdr
    out += b"\x00" * _round_up_4(len(out))
    out += trailer + b"\x00"
    out += b"\x00" * _round_up_4(len(out))

    return bytes(out)

# =========================================
# Deterministic GZIP builder
# =========================================
def build_gzip_exact(uncompressed: bytes) -> bytes:
    header = bytes([
        0x1F,0x8B,0x08,0x00,
        0x00,0x00,0x00,0x00,
        0x02,0xFF
    ])
    co = zlib.compressobj(9, zlib.DEFLATED, -15)
    comp = co.compress(uncompressed) + co.flush()
    crc  = binascii.crc32(uncompressed) & 0xFFFFFFFF
    size = len(uncompressed) & 0xFFFFFFFF
    return header + comp + struct.pack("<II", crc, size)

# =========================================
# Resource Item
# =========================================
class AmlResItem:
    _format = f"IIIIIIIBBBB{IH_NMLEN}s"
    _size   = struct.calcsize(_format)

    def __init__(self):
        self.magic = IH_MAGIC
        self.hcrc  = 0
        self.size  = 0
        self.start = 0
        self.end   = 0
        self.next  = 0
        self.dcrc  = 0
        self.index = 0
        self.nums  = ARCH_ARM
        self.type  = 0
        self.comp  = 0
        self.name  = b""
        self.data  = b""

    def clean_name(self):
        return self.name.decode("ascii","ignore")

    @classmethod
    def from_file(cls,path):
        obj = cls()
        with open(path,"rb") as f:
            obj.data=f.read()
        nm=os.path.splitext(os.path.basename(path))[0]
        obj.name=nm.encode("ascii","ignore")[:IH_NMLEN].ljust(IH_NMLEN,b"\x00")
        obj.size=len(obj.data)
        obj.dcrc=binascii.crc32(obj.data)&0xFFFFFFFF
        return obj

    @classmethod
    def unpack_from(cls,fp):
        buf = fp.read(cls._size)
        if len(buf)!=cls._size:
            raise Exception("EOF in item")

        obj=cls()
        (obj.magic,obj.hcrc,obj.size,obj.start,obj.end,obj.next,obj.dcrc,
         obj.index,obj.nums,obj.type,obj.comp,obj.name)=struct.unpack(cls._format,buf)
        obj.name=obj.name.rstrip(b"\x00")

        if obj.magic != IH_MAGIC:
            raise Exception("Bad magic")

        cur=fp.tell()
        fp.seek(obj.start)
        obj.data=fp.read(obj.size)
        fp.seek(cur)

        return obj

    def pack(self):
        return struct.pack(
            self._format,
            self.magic,self.hcrc,self.size,self.start,self.end,
            self.next,self.dcrc,self.index,self.nums,self.type,self.comp,
            self.name.ljust(IH_NMLEN,b"\x00")
        )

# =========================================
# Header
# =========================================
class AmlResImgHead:
    _format = f"Ii{AML_RES_IMG_V1_MAGIC_LEN}sIII{AML_RES_IMG_HEAD_SZ-8*3-4}s"
    _size   = struct.calcsize(_format)

    def __init__(self):
        self.crc=0
        self.version=AML_RES_IMG_VERSION_V2
        self.magic=AML_RES_IMG_V1_MAGIC
        self.imgSz=0
        self.imgItemNum=0
        self.alignSz=AML_RES_IMG_ITEM_ALIGN_SZ
        self.reserv=b"\x00"*(AML_RES_IMG_HEAD_SZ-8*3-4)

    @classmethod
    def unpack_from(cls,fp):
        raw=fp.read(cls._size)
        if len(raw)!=cls._size:
            raise Exception("EOF header")
        obj=cls()
        (obj.crc,obj.version,obj.magic,obj.imgSz,obj.imgItemNum,obj.alignSz,obj.reserv)=struct.unpack(cls._format,raw)

        if obj.magic!=AML_RES_IMG_V1_MAGIC:
            raise Exception("Bad header magic")
        return obj

    def pack(self):
        return struct.pack(
            self._format,
            self.crc,self.version,self.magic,
            self.imgSz,self.imgItemNum,self.alignSz,self.reserv
        )

# =========================================
# Image
# =========================================
class AmlResourcesImage:
    def __init__(self):
        self.header=AmlResImgHead()
        self.items=[]

    @classmethod
    def unpack_from(cls,fp):
        img=cls()
        img.header=AmlResImgHead.unpack_from(fp)
        while True:
            it=AmlResItem.unpack_from(fp)
            img.items.append(it)
            if it.next==0:
                break
            fp.seek(it.next)
        return img

    def pack(self):
        data=b""
        for item in self.items:
            item.start=len(data) + AmlResImgHead._size + len(self.items)*AmlResItem._size
            item.size=len(item.data)
            item.dcrc=binascii.crc32(item.data)&0xFFFFFFFF
            data+=item.data
            pad=(self.header.alignSz-(len(data)%self.header.alignSz))%self.header.alignSz
            if pad:
                data+=b"\x00"*pad

        headers=b""
        for i,item in enumerate(self.items):
            item.index=i
            item.next=0 if i==len(self.items)-1 else (AmlResImgHead._size + (i+1)*AmlResItem._size)
            headers+=item.pack()

        self.header.imgItemNum=len(self.items)
        self.header.imgSz=AmlResImgHead._size + len(headers) + len(data)

        return self.header.pack() + headers + data

# =========================================
# CONFIG
# =========================================
def load_config(cfgfile):
    rows=[]
    with open(cfgfile,"r",encoding="utf-8") as f:
        for line in f:
            p=line.strip().split("|")
            if len(p)==2:
                rows.append((p[0],p[1]))
    return rows

def config_map(rows):
    return {name:fmt for name,fmt in rows}

# =========================================
# UNPACK
# =========================================
def unpack_image(img,outdir,config_outdir=None):
    try:
        with open(img,"rb") as f:
            res=AmlResourcesImage.unpack_from(f)

        os.makedirs(outdir,exist_ok=True)

        rows=[]

        for i,item in enumerate(res.items):
            fmt=detect_format(item.data)
            print(f"index={i} name={item.clean_name()} start=0x{item.start:X} size={item.size} format={fmt}")

            name=item.clean_name()
            dst=os.path.join(outdir,f"{name}.bmp")

            with open(dst,"wb") as w:
                w.write(item.data)

            if fmt=="bmp":
                pass

            elif fmt=="gzip_bmp":
                unz=gzip.decompress(item.data)
                os.remove(dst)
                with open(dst,"wb") as w: w.write(unz)

            elif fmt=="gzip_cpio_bmp":
                unz=gzip.decompress(item.data)
                first=parse_cpio_newc_first_file(unz)
                if first:
                    _,payload = first
                    os.remove(dst)
                    with open(dst,"wb") as w: w.write(payload)

            elif fmt=="cpio_bmp":
                first=parse_cpio_newc_first_file(item.data)
                if first:
                    _,payload=first
                    os.remove(dst)
                    with open(dst,"wb") as w: w.write(payload)

            rows.append((name,fmt))

        config_name=os.path.splitext(os.path.basename(img))[0] + "_config"
        if config_outdir:
            os.makedirs(config_outdir,exist_ok=True)
            cfg=os.path.join(config_outdir,config_name)
        else:
            cfg=os.path.join(outdir,config_name)

        with open(cfg,"w",encoding="utf-8") as f:
            for name,fmt in rows:
                f.write(f"{name}|{fmt}\n")

        print("Successfully unpacked")

    except:
        print("Failed to unpack")
        raise

# =========================================
# PACK
# =========================================
def pack_image(output,indir,cfgfile):
    try:
        rows=load_config(cfgfile)
        fmtmap=config_map(rows)

        files=[f for f in os.listdir(indir) if f.lower().endswith(".bmp")]
        files.sort()

        img=AmlResourcesImage()

        for i,fname in enumerate(files):
            name=os.path.splitext(fname)[0]
            path=os.path.join(indir,fname)
            fmt=fmtmap.get(name,"bmp")

            raw_size=os.path.getsize(path)
            print(f"index={i} name={name} size={raw_size} format={fmt}")

            item=AmlResItem.from_file(path)
            item.name=name.encode("ascii","ignore")[:IH_NMLEN].ljust(IH_NMLEN,b"\x00")

            payload=item.data

            if fmt=="bmp":
                final=payload

            elif fmt=="gzip_bmp":
                final=build_gzip_exact(payload)

            elif fmt=="gzip_cpio_bmp":
                cpio=build_cpio_newc_single_file(name,payload)
                final=build_gzip_exact(cpio)

            elif fmt=="gzip_cpio_unknown":
                cpio=build_cpio_newc_single_file(name,payload)
                final=build_gzip_exact(cpio)

            elif fmt=="gzip_unknown":
                final=build_gzip_exact(payload)

            elif fmt=="cpio_bmp":
                final=build_cpio_newc_single_file(name,payload)

            elif fmt=="cpio_unknown":
                final=build_cpio_newc_single_file(name,payload)

            else:
                final=payload

            item.data=final
            item.size=len(final)
            item.dcrc=binascii.crc32(final)&0xFFFFFFFF

            img.items.append(item)

        blob=img.pack()

        with open(output,"wb") as f:
            f.write(blob)

        print("Successfully packed")

    except:
        print("Failed to repack")
        raise

# =========================================
# LIST
# =========================================
def list_items(img):
    with open(img,"rb") as f:
        res=AmlResourcesImage.unpack_from(f)

    for i,item in enumerate(res.items):
        fmt=detect_format(item.data)
        print(f"index={i} name={item.clean_name()} start=0x{item.start:X} size={item.size} format={fmt}")

# =========================================
# CLI
# =========================================
def main():
    parser=argparse.ArgumentParser(description="Amlogic resource tool")

    group=parser.add_mutually_exclusive_group(required=True)
    group.add_argument("-d",action="store_true",help="unpack image")
    group.add_argument("-l",action="store_true",help="list items")
    group.add_argument("-r",metavar="OUTIMG",help="repack into OUTIMG")

    parser.add_argument("-o",help="output directory for unpack")
    parser.add_argument("-C","--config-outdir",help="directory for generated config file")
    parser.add_argument("-i","--indir",help="input directory for repacking")
    parser.add_argument("-c","--config",help="config file path for repack")
    parser.add_argument("files",nargs="*",help="input image for -d/-l")

    args=parser.parse_args()

    if args.d:
        if len(args.files)!=1:
            print("ERROR: -d requires exactly 1 input image")
            sys.exit(1)
        if not args.o:
            print("ERROR: -o required for unpack")
            sys.exit(1)
        unpack_image(args.files[0],args.o,args.config_outdir)
        return

    if args.l:
        if len(args.files)!=1:
            print("ERROR: -l requires exactly 1 input image")
            sys.exit(1)
        list_items(args.files[0])
        return

    if args.r:
        if not args.indir:
            print("ERROR: -i <indir> required for repack")
            sys.exit(1)
        if not args.config:
            print("ERROR: -c <config> required for repack")
            sys.exit(1)
        pack_image(args.r,args.indir,args.config)
        return

if __name__=="__main__":
    main()
