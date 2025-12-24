#!/usr/bin/env bash
set -euo pipefail

# Script to unpack Rockchip firmware images
# Usage: ./unpacker.sh [firmware_file]

usage() {
    cat <<EOF
Usage:
  $(basename "$0") [options] <firmware_file>

Options:
  -o, --outdir DIR   Output directory (default: ./output)
  -c, --clean        Remove output directory before unpacking (requires --outdir/-o)
  -l1, --level1      Unpack level 1 firmware
  -h, --help         Show this help
EOF
}

RED=$'\e[0;31m'
GREEN=$'\e[0;32m'
ORANGE=$'\e[0;33m'
CYAN=$'\e[0;36m'
NC=$'\e[0m'


WORKDIR="$(pwd -P)"
SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd -P)"

OUTDIR=$SCRIPT_DIR"/output"
FIRMWARE_FILE=""
CLEAN=false
OUTDIR_EXPLICIT=false
LEVEL1=false
LEVEL2=false
LEVEL3=false
UNPACK=false
PACK=false
BIN_RK_IMAGE_MAKER="$SCRIPT_DIR/bin/rkImageMaker2"
BIN_AFPTOOL="$SCRIPT_DIR/bin/afptool2"
PYTHON_TOOLS_DIR="$SCRIPT_DIR/bin/common/python"

while (($#)); do
    case "$1" in
        -h|--help)
            usage
            exit 0
            ;;
        --outdir=*)
            OUTDIR="${1#--outdir=}"
            OUTDIR_EXPLICIT=true
            ;;
        -o|--outdir)
            shift
            if [[ -z "${1:-}" ]]; then
                OUTDIR="./output"
                OUTDIR_EXPLICIT=true
            else
                OUTDIR="$1"
                OUTDIR_EXPLICIT=true
            fi
            ;;
        -c|--clean)
            CLEAN=true
            OUTDIR_EXPLICIT=true
            ;;
        -l1|--level1)
            LEVEL1=true
            ;;
        -l2|--level2)
            LEVEL2=true
            ;;
        -l3|--level3)
            LEVEL3=true
            ;;
        -u|--unpack)
            UNPACK=true
            ;;
        -p|--pack)
            PACK=true
            ;;
        --)
            shift
            if [[ -z "$FIRMWARE_FILE" && $# -gt 0 ]]; then
                FIRMWARE_FILE="$1"
                shift
            fi
            if [[ $# -gt 0 ]]; then
                printf 'Unexpected argument: %s\n' "$1" >&2
                usage >&2
                exit 2
            fi
            break
            ;;
        -*)
            printf 'Unknown option: %s\n' "$1" >&2
            usage >&2
            exit 2
            ;;
        *)
            if [[ -z "$FIRMWARE_FILE" ]]; then
                FIRMWARE_FILE="$1"
            else
                printf 'Unexpected argument: %s\n' "$1" >&2
                usage >&2
                exit 2
            fi
            ;;
    esac
    shift
done

if [[ -z "$FIRMWARE_FILE" && "$CLEAN" != true ]]; then
    usage
    exit 1
fi

if [[ "$CLEAN" == true && "$OUTDIR_EXPLICIT" != true ]]; then
    printf 'Error: --clean requires --outdir/-o to be set explicitly\n' >&2
    usage >&2
    exit 2
fi

if [[ -z "$OUTDIR" ]]; then
    printf 'Missing value for --outdir\n' >&2
    usage >&2
    exit 2
fi

# Validate dependencies between flags
if [[ "$LEVEL1" == true || "$LEVEL2" == true || "$LEVEL3" == true ]]; then
    if [[ "$UNPACK" != true && "$PACK" != true ]]; then
        printf '%sError: Level flags (-l1, -l2, -l3) require either --unpack/-u or --pack/-p%s\n' "$RED" "$NC" >&2
        usage >&2
        exit 2
    fi
fi

if [[ "$UNPACK" == true && "$PACK" == true ]]; then
    printf '%sError: Cannot use --unpack and --pack together%s\n' "$RED" "$NC" >&2
    usage >&2
    exit 2
fi

if [[ "$UNPACK" == true || "$PACK" == true ]]; then
    if [[ "$LEVEL1" != true && "$LEVEL2" != true && "$LEVEL3" != true ]]; then
        printf '%sError: --unpack/-u or --pack/-p require a level flag (-l1, -l2, or -l3)%s\n' "$RED" "$NC" >&2
        usage >&2
        exit 2
    fi
fi




mkd() {
    # Create one or more directories if they don't exist
    mkdir -pv -- "$@"
}

clean_outdir() {
    if [[ -z "$OUTDIR" || "$OUTDIR" == "/" ]]; then
        printf '%sError: refusing to clean invalid OUTDIR: %s%s\n' "$RED" "$OUTDIR" "$NC" >&2
        exit 1
    fi

    if [[ -e "$OUTDIR" ]]; then
        printf '%sCleaning output directory: %s%s\n' "$ORANGE" "$OUTDIR" "$NC"
        rm -rf -- "$OUTDIR"
    fi
}

if [[ "$CLEAN" == true && -z "$FIRMWARE_FILE" ]]; then
    clean_outdir
    exit 0
fi


# preprocessing steps
# Remove existing output directory if it exists
# Check for rkImage in ./bin directory
# Check for afptools in ./bin directory
# write a function to validate prerequisites

validate_prerequisites() {
    # print verification status for required binaries
    # checking for rkImageMaker
    printf '%sValidating prerequisites...%s\n' "$ORANGE" "$NC"

    local bin_dir="./bin"
    if [[ ! -d "$bin_dir" ]]; then
        printf '%sError: %s directory not found%s\n' "$RED" "$bin_dir" "$NC" >&2
        exit 1
    fi
    printf '%sValidating binaries directory...%sOK%s\n' "$ORANGE" "$GREEN" "$NC"


    if [[ ! -f "$BIN_RK_IMAGE_MAKER" ]]; then
        printf '%sError: rkImageMaker not found at %s%s\n' "$RED" "$BIN_RK_IMAGE_MAKER" "$NC" >&2
        exit 1
    fi
    printf '%sValidating rkImageMaker...%sOK%s\n' "$ORANGE" "$GREEN" "$NC"


    if [[ ! -f "$BIN_AFPTOOL" ]]; then
        printf '%sError: afptool not found at %s%s\n' "$RED" "$BIN_AFPTOOL" "$NC" >&2
        exit 1
    fi

    printf '%sValidating afptool...%sOK%s\n' "$ORANGE" "$GREEN" "$NC"

    printf '%sAll required binaries validated.%s\n' "$GREEN" "$NC"
}

# Unpacking firmware level one:

# extract firmware contents using rkImageMaker
extract_firmware_rkimagemaker() {
    local firmware_file="$1"
    local output_dir="$2"

    printf '%sExtracting firmware contents using rkImageMaker...%s\n' "$ORANGE" "$NC"

    # Use rkImageMaker to extract the firmware
    if "$BIN_RK_IMAGE_MAKER" -unpack "$firmware_file" "$output_dir"; then
        printf '%sFirmware extraction completed successfully.%s\n' "$GREEN" "$NC"
    else
        printf '%sError: Failed to extract firmware contents.%s\n' "$RED" "$NC" >&2
        exit 1
    fi
}

# Extract firmware contents using afptool
extract_firmware_afptool() {
    local firmware_file="$1"
    local output_dir="$2"

    printf '%sExtracting firmware contents using afptool...%s\n' "$ORANGE" "$NC"

    # Use afptool to extract the firmware
    if "$BIN_AFPTOOL" -unpack "$firmware_file" "$output_dir"; then
        printf '%sFirmware extraction completed successfully.%s\n' "$GREEN" "$NC"
    else
        printf '%sError: Failed to extract firmware contents.%s\n' "$RED" "$NC" >&2
        exit 1
    fi
}

run_level_one_unpack() {
    local firmware_file="$1"
    local output_dir="$2"

    local level1_dir="$output_dir/level1"
    local level1_firmware_img="$level1_dir/firmware.img"
    local level1_boot_bin="$level1_dir/boot.bin"

    mkd "$level1_dir"

    extract_firmware_rkimagemaker "$firmware_file" "$level1_dir"

    local missing=()
    if [[ ! -f "$level1_boot_bin" ]]; then
        missing+=("boot.bin")
    fi
    if [[ ! -f "$level1_firmware_img" ]]; then
        missing+=("firmware.img")
    fi
    if (( ${#missing[@]} )); then
        printf '%sError: rkImageMaker output missing in %s: %s%s\n' "$RED" "$level1_dir" "${missing[*]}" "$NC" >&2
        exit 1
    fi

    extract_firmware_afptool "$level1_firmware_img" "$level1_dir"

    # delete boot.in and firmware.img
    rm -f "$level1_dir/boot.bin"
    rm -f "$level1_dir/firmware.img"
    # move  contents of Image directory to level1_dir
    mv "$level1_dir/Image"/* "$level1_dir"
    # remove Image directory
    rm -rf "$level1_dir/Image"

    missing=()
    if [[ ! -f "$level1_dir/package-file" ]]; then
        missing+=("package-file")
    fi
    if [[ ! -f "$level1_dir/MiniLoaderAll.bin" ]]; then
        missing+=("MiniLoaderAll.bin")
    fi
    if [[ ! -f "$level1_dir/parameter.txt" ]]; then
        missing+=("parameter.txt")
    fi

    local required_images=(
        baseparameter.img
        boot.img
        dtbo.img
        misc.img
        recovery.img
        super.img
        uboot.img
        vbmeta.img
    )
    local img
    for img in "${required_images[@]}"; do
        if [[ ! -f "$level1_dir/$img" ]]; then
            missing+=("$img")
        fi
    done
    if (( ${#missing[@]} )); then
        printf '%sError: afptool output missing in %s: %s%s\n' "$RED" "$level1_dir" "${missing[*]}" "$NC" >&2
        exit 1
    fi


}


extract_level_one_firmware() {
    clean_outdir
    mkd "$OUTDIR"

    # Banner "Unpacking to $OUTDIR"
    printf '\n%s' "$CYAN"
    cat <<EOF
========================================
   Unpacking Level 1. Into $OUTDIR
========================================
EOF
    printf '%s' "$NC"

    run_level_one_unpack "$FIRMWARE_FILE" "$OUTDIR"

    printf '%s\n' "$GREEN"
    cat <<EOF
========================================
   Level 1 unpack complete
========================================
EOF
    printf '%s' "$NC"
}

unpack_super() {
    local level1_dir=$1
    local level2_dir=$2
    local level2_config=$3
    local partition_name=$4

    #if level1/super.img exists,
    if [[ -f "$level1_dir/$partition_name" ]]; then
        # image check using $PYTHON_TOOLS_DIR/imagcheck.py
        printf '%sChecking %s integrity...%s\n' "$ORANGE" "$partition_name" "$NC"
        if output=$(python3 "$PYTHON_TOOLS_DIR/imgcheck.py" "$level1_dir/$partition_name"); then
            if [[ "$output" == *"sparse"* ]]; then
                printf '%sSparse image detected.%s\n' "$GREEN" "$NC"
                printf '%sUnsparsing the image.%s\n' "$GREEN" "$NC"
                python3 "$PYTHON_TOOLS_DIR/simg2img.py" "$level1_dir/$partition_name" "$level2_dir/$partition_name.raw"
                python3 "$PYTHON_TOOLS_DIR/du.py" -b "$level2_dir/$partition_name.raw" | awk '{print $1}' > "$level2_config/${partition_name%.img}_size"
                python3 "$PYTHON_TOOLS_DIR/lpunpack.py" "$level2_dir/$partition_name.raw" "$level2_dir/super"
            else
                printf '%sRaw image detected.%s\n' "$GREEN" "$NC"
                # For raw images, copy and use the original
                cp "$level2_dir/$partition_name" "$level2_dir/$partition_name.raw"
                python3 "$PYTHON_TOOLS_DIR/du.py" -b "$level2_dir/$partition_name.raw" | awk '{print $1}' > "$level2_config/${partition_name%.img}_size"
                python3 "$PYTHON_TOOLS_DIR/lpunpack.py" "$level2_dir/$partition_name.raw" "$level2_dir/${partition_name}.raw"
            fi
        else
            printf '%sError: %s image check failed.%s\n' "$RED" "$partition_name" "$NC" >&2
            exit 1
        fi
        rm -f "$level2_dir/$partition_name.raw"
    else
        printf '%sError: %s not found in %s%s\n' "$RED" "$partition_name" "$level1_dir" "$NC" >&2
        exit 1
    fi


}

unpack_linux_partition() {
    local level2_dir=$1
    local level2_config=$2
    local partition_name=$3
    local partition_img="$partition_name.img"
    local partition_path="$level2_dir/super/$partition_img"

    #if level1/partition_name exists,
    if [[ -f "$partition_path" ]]; then
        # image check using $PYTHON_TOOLS_DIR/imagcheck.py
        printf '%sChecking %s integrity...%s\n' "$ORANGE" "$partition_img" "$NC"
        if output=$(python3 "$PYTHON_TOOLS_DIR/imgcheck.py" "$partition_path"); then
            if [[ "$output" == *"sparse"* ]]; then
                printf '%sTo Be Implemented.%s\n' "$RED" "$NC"
            else
                printf '%sRaw image detected.%s\n' "$GREEN" "$NC"
                # For raw images, copy and use the original
                sudo python3 "$PYTHON_TOOLS_DIR/imgextractor.py" "$partition_path" "$level2_dir"
                python3 "$PYTHON_TOOLS_DIR/du.py" -b "$partition_path" | awk '{print $1}' > "$level2_config/${partition_img%.img}_size"
            fi
        else
            printf '%sError: %s image check failed.%s\n' "$RED" "$partition_img" "$NC" >&2
            exit 1
        fi
    else
        printf '%sError: %s not found in %s%s\n' "$RED" "$partition_img" "$partition_path" "$NC" >&2
        exit 1
    fi
}


extract_level_two_firmware() {
    local level1_dir="$OUTDIR/level1"
    local level2_dir="$OUTDIR/level2"
    local work_dir_aik="$SCRIPT_DIR/bin/aik"
    local level2_config="$level2_dir/.config"

    printf '\n%s' "$CYAN"
    cat <<EOF
========================================
    Unpacking Level 2. Into $level2_dir
========================================
EOF
    printf '%s' "$NC"

    mkd "$level2_dir"
    mkd "$level2_config"
    mkd "$work_dir_aik"


    unpack_super "$level1_dir" "$level2_dir" "$level2_config" "super.img"

    # unpack_linux_partition "$level1_dir" "$level2_dir" "$level2_config" "system.img"
    # unpack_linux_partition "$level1_dir" "$level2_dir" "$level2_config" "vendor.img"
    # unpack_linux_partition "$level1_dir" "$level2_dir" "$level2_config" "product.img"
    # unpack_linux_partition "$level1_dir" "$level2_dir" "$level2_config" "odm.img"
    # unpack_linux_partition "$level1_dir" "$level2_dir" "$level2_config" "oem.img"
    # unpack_linux_partition "$level1_dir" "$level2_dir" "$level2_config" "system_ext.img"
    # unpack_linux_partition "$level1_dir" "$level2_dir" "$level2_config" "odm_ext.img"

    for partition_name in system vendor product odm system_ext; do
        unpack_linux_partition "$level2_dir" "$level2_config" "$partition_name"
    done

    # unpacking boot and recovery using AIK
    for partition_name in boot recovery; do
        local partition_img="$partition_name.img"
        local partition_path="$level1_dir/$partition_img"
        local partition_aik_img="$work_dir_aik/$partition_name"
        cp "$partition_path" "$partition_aik_img"
        bash "$work_dir_aik/unpackimg.sh" "$partition_aik_img"
        shopt -s nullglob
        for f in "$work_dir_aik/split_img/${partition_name}"-*; do
            mv "$f" "$work_dir_aik/split_img/${f##*-}"
        done
        shopt -u nullglob

        mkd "$level2_dir/$partition_name/"
        for f in split_img ramdisk; do
            mv "$work_dir_aik/$f" "$level2_dir/$partition_name/$f"
        done
        printf '%sSuccess unpacking %s partition.%s\n' "$GREEN" "$partition_name" "$NC"

    done

    # unpack dtb
    local dtb_img="$level1_dir/dtbo.img"
    local dtb_dir="$level2_dir/boot/split_img"
    mkd "$dtb_dir"
    # Try gzip first
    # if gzip -t "$dtb_img" 2>/dev/null; then
    #     gunzip -d "$dtb_img" > "$dtb_dir/dtb.raw"
    # else
    #     cp "$dtb_img" "$dtb_dir/dtb.raw"
    # fi
    # python3 "$PYTHON_TOOLS_DIR/splitdtb.py" "$dtb_dir/dtb.raw" "$dtb_dir"
    # for f in "$dtb_dir"/*.dtb; do
    #     dtc -I dtb -O dts "$f" > "${f%.dtb}.dts"
    #     rm -f "$f"
    # done

    python3 "$PYTHON_TOOLS_DIR/splitdtb.py" "$dtb_dir/dtb" "$dtb_dir"  $level2_config/dtb_unpack

        for f in "$dtb_dir"/*.dtb; do
        dtc -I dtb -O dts "$f" > "${f%.dtb}.dts"
        mkdir -p "$level2_dir/dtb"
        mv "${f%.dtb}.dts" "$level2_dir/dtb/"
        rm -f "$f"
    done


    printf '%sSuccess unpacking dtbo partition.%s\n' "$GREEN" "dtbo" "$NC"


    printf '%s\n' "$GREEN"
    cat <<EOF
========================================
   Level 2 unpack complete
========================================
EOF
    printf '%s' "$NC"
}

pack_level_one_firmware() {
    local level1_dir="$OUTDIR/level1"
    local level2_dir="$OUTDIR/level2"


}


# extracting level one firmware contents
# 1. Extract firmware contents using rkImageMaker
# 2. Extract firmware contents using afptool

cat <<EOF
========================================
   🚀 Rockchip Firmware Pack-Unpack Utility
   Author : Jehu
   Device : RK3576
========================================
EOF



if [[ "$CLEAN" == true ]]; then
    clean_outdir
    mkd "$OUTDIR"
fi

if [[ "$LEVEL1" == true && "$UNPACK" == true ]]; then
    # Validate prerequisites
    validate_prerequisites
    extract_level_one_firmware
fi

if [[ "$LEVEL2" == true && "$UNPACK" == true ]]; then
    # Validate prerequisites
    validate_prerequisites
    extract_level_one_firmware

    extract_level_two_firmware
    
fi

if [[ "$LEVEL1" == true && "$PACK" == true ]]; then
    echo "Unpacking Level 1 firmware..."
    pack_level_one_firmware
fi

if [[ "$LEVEL2" == true && "$PACK" == true ]]; then
    echo "Unpacking Level 2 firmware..."
fi

exit 0