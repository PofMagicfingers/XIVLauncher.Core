#!/bin/bash
# Installs XLM (XIVLauncher Manager) as a Steam compatibility tool in DroidDeck, natively on ARM64.
# Run it from DroidDeck's desktop Konsole:
#   curl -fsSL https://github.com/PofMagicfingers/XIVLauncher.Core/releases/latest/download/install.sh | bash
#
# - xlm: built for aarch64 (glibc 2.31, like DroidDeck's Debian);
# - XIVLauncher.Core: built for linux-arm64 with its own glibc; it starts the game through
#   DroidDeck's ARM64 Proton (wine64/wineserver scripts in xlcore/proton-arm64);
# - aria2c: static aarch64 build.
# XLM_RELEASE points at another folder holding the same files (version, XIVLauncher.Core.tar.gz,
# xlm-aarch64, aria2c-arm64.tar.gz), for a local build served over HTTP.
set -e

RELEASE="${XLM_RELEASE:-https://github.com/PofMagicfingers/XIVLauncher.Core/releases/latest/download}"
COMPAT="${STEAM_COMPAT:-$HOME/.local/share/Steam/compatibilitytools.d}"
TOOL="$COMPAT/XLM"

echo "[1/3] Downloading XLM (aarch64) from $RELEASE"
tmp=$(mktemp -d)
curl --fail -L "$RELEASE/xlm-aarch64" -o "$tmp/xlm"
chmod +x "$tmp/xlm"

echo "[2/3] Installing into $TOOL"
rm -rf "$TOOL" "$COMPAT/xlm"
mkdir -p "$COMPAT"
"$tmp/xlm" install-steam-tool --xlm-updater-disable \
    --extra-launch-args="--run-as-steam-compat-tool=true --use-fallback-secret-provider --xlcore-web-release-url $RELEASE/ --aria-source url:$RELEASE/aria2c-arm64.tar.gz" \
    --steam-compat-path "$COMPAT/"
rm -rf "$tmp"

echo "[3/3] Checking"
grep -q "xlcore-web-release-url" "$TOOL/xlm.sh" || { echo "unexpected xlm.sh"; exit 1; }
"$TOOL/xlm" --version || true

echo
echo "Done. Go back to gaming mode (Steam restarts), then on FINAL FANTASY XIV (or the free trial):"
echo "Properties > Compatibility > force \"XLCore [XLM]\", empty launch options. On first launch XLM"
echo "downloads XIVLauncher from $RELEASE."
