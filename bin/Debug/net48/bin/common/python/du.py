import os
import sys

def get_path_size_bytes(path):
    """Return size in bytes for a file or directory."""
    if os.path.isfile(path):
        try:
            return os.path.getsize(path)
        except (FileNotFoundError, PermissionError):
            return 0
    elif os.path.isdir(path):
        total_bytes = 0
        for root, dirs, files in os.walk(path, topdown=True):
            for f in files:
                fp = os.path.join(root, f)
                try:
                    total_bytes += os.path.getsize(fp)
                except (FileNotFoundError, PermissionError):
                    pass
        return total_bytes
    else:
        return 0

def main():
    if len(sys.argv) != 3:
        print("Usage: du_emulator.py -sk|-sb|-b <file_or_directory>")
        sys.exit(1)

    flag = sys.argv[1]
    path = sys.argv[2]

    if not os.path.exists(path):
        print(f"Error: Path does not exist: {path}")
        sys.exit(1)

    total_bytes = get_path_size_bytes(path)

    if flag == "-sk":
        # du -sk : kilobytes
        print(total_bytes // 1024)
    elif flag == "-sb" or flag == "-b":
        # du -sb / du -b : total bytes
        print(total_bytes)
    else:
        print("Error: Unknown option. Use -sk, -sb, or -b.")
        sys.exit(1)

if __name__ == "__main__":
    main()
