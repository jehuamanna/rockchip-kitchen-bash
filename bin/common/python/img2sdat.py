import sys
import struct

BLOCK_SIZE = 4096

def rangeset(src):
    """Processes transfer list data into a list of block ranges."""
    src = [int(i) for i in src]
    out = []
    index = 0
    while index < len(src):
        start = src[index]
        count = src[index + 1]
        out.extend(range(start, start + count))
        index += 2
    return out

def parse_transfer_list(transfer_list):
    """Parses the Transfer List file."""
    with open(transfer_list, 'r') as f:
        lines = f.read().splitlines()

    # Version
    version = int(lines[0])
    if version > 4 or version < 1:
        raise ValueError("Unsupported transfer list version %d" % version)

    # Number of operations
    idx = 1
    new_blocks = int(lines[idx]); idx += 1

    if version >= 2:
        # Skip stash entries count
        idx += 1

    # Extract ranges
    commands = {}
    while idx < len(lines):
        line = lines[idx]
        idx += 1
        if line.startswith("new"):
            cmds = line.split()
            # Format: new <num> <range data...>
            count = int(cmds[1])
            ranges = cmds[2:]
            commands["new"] = rangeset(ranges)
        elif line.startswith("erase"):
            cmds = line.split()
            # Format: erase <num> <range data...>
            count = int(cmds[1])
            ranges = cmds[2:]
            commands["erase"] = rangeset(ranges)
        else:
            pass

    return version, new_blocks, commands

def main():
    if len(sys.argv) < 4:
        print("Usage: {} <transfer.list> <system.new.dat> <output.img>".format(sys.argv[0]))
        sys.exit(1)

    transfer_list = sys.argv[1]
    dat_file = sys.argv[2]
    output_img = sys.argv[3]

    print("[*] Transfer list:", transfer_list)
    print("[*] New dat:", dat_file)
    print("[*] Output:", output_img)

    version, new_blocks, commands = parse_transfer_list(transfer_list)
    new_ranges = commands.get("new", [])

    print("[*] Version:", version)
    print("[*] Total new blocks:", new_blocks)

    block_count = max(new_ranges) + 1 if new_ranges else 0
    image_size = block_count * BLOCK_SIZE

    print("[*] Allocating image size:", image_size)

    with open(dat_file, 'rb') as dat, open(output_img, 'wb') as img:
        img.truncate(image_size)
        print("[*] Writing blocks...")

        for block in new_ranges:
            data = dat.read(BLOCK_SIZE)
            if not data:
                print("[!] Unexpected end of .dat file!")
                break
            img.seek(block * BLOCK_SIZE)
            img.write(data)

    print("[+] Done. Output written to: {}".format(output_img))

if __name__ == "__main__":
    main()