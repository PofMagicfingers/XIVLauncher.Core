# Shared by wine64 and wineserver: finds DroidDeck's ARM64 Proton and the Steam environment it
# expects, with XIVLauncher's Wine prefix as Proton's "pfx".
steam_root=${STEAM_COMPAT_CLIENT_INSTALL_PATH:-${XDG_DATA_HOME:-$HOME/.local/share}/Steam}
tool="$steam_root/compatibilitytools.d/droiddeck-proton-arm64"

depot=
best=0
libraries=$(printf '%s\n' "$steam_root" /mnt/droiddeck /mnt/droiddeck-sd; sed -n 's/^[[:space:]]*"path"[[:space:]]*"\(.*\)"[[:space:]]*$/\1/p' "$steam_root/steamapps/libraryfolders.vdf" 2>/dev/null)
while IFS= read -r library; do
    for candidate in "$library"/steamapps/common/*; do
        [ -d "$candidate/files/bin-arm64" ] && [ -x "$candidate/proton" ] || continue
        build=0
        read -r build _ 2>/dev/null < "$candidate/version"
        case $build in "" | *[!0-9]*) build=0 ;; esac
        if [ -z "$depot" ] || [ "$build" -gt "$best" ]; then
            depot=$candidate
            best=$build
        fi
    done
done <<< "$libraries"

if [ -z "$depot" ] || [ ! -x "$tool/droiddeck-proton" ]; then
    echo "xlcore-proton: DroidDeck's ARM64 Proton is not installed yet (open Steam once)" >&2
    exit 1
fi

prefix=${WINEPREFIX:-$HOME/.xlcore/wineprefix}
mkdir -p "$prefix"
compat="$(dirname "$prefix")/proton-compatdata"
mkdir -p "$compat"
[ "$(readlink "$compat/pfx")" = "$prefix" ] || ln -sfn "$prefix" "$compat/pfx"

export WINEPREFIX="$prefix"
export STEAM_COMPAT_CLIENT_INSTALL_PATH="$steam_root"
export STEAM_COMPAT_DATA_PATH="$compat"
export STEAM_COMPAT_INSTALL_PATH="${STEAM_COMPAT_INSTALL_PATH:-$prefix}"
export STEAM_COMPAT_TOOL_PATHS="$tool:$depot"
export STEAM_COMPAT_LIBRARY_PATHS="${STEAM_COMPAT_LIBRARY_PATHS:-$steam_root/steamapps}"
# FINAL FANTASY XIV, so Proton applies its fixes for the game.
export STEAM_COMPAT_APP_ID="${STEAM_COMPAT_APP_ID:-39210}"
# The id Steam launched, which Gamescope uses to pick the window it shows. XIVLauncher overwrites
# SteamAppId/SteamGameId with each id it tries to initialise Steam with (312060, the free trial,
# last), and a game window tagged with another id than the running one is never shown.
export SteamAppId="$STEAM_COMPAT_APP_ID" SteamGameId="$STEAM_COMPAT_APP_ID"
