#!/bin/bash
# Installs XLM (XIVLauncher Manager) as a Steam compatibility tool in DroidDeck, from this fork's
# test release, until the ARM64 builds are released upstream. From DroidDeck's desktop Konsole:
#   curl -fsSL https://github.com/PofMagicfingers/XIVLauncher.Core/releases/latest/download/install.sh | bash
set -e

RELEASE="${XLM_RELEASE:-https://github.com/PofMagicfingers/XIVLauncher.Core/releases/latest/download}"
COMPAT="${STEAM_COMPAT:-$HOME/.local/share/Steam/compatibilitytools.d}"

tmp=$(mktemp -d)
trap 'rm -rf "$tmp"' EXIT
curl --fail -L "$RELEASE/xlm-aarch64" -o "$tmp/xlm"
chmod +x "$tmp/xlm"

# Keep the user's XLM pre- and post-launch scripts across reinstalls.
keep=$(mktemp -d)
for hooks in prelaunch.d postlaunch.d; do
    [ -d "$COMPAT/XLM/$hooks" ] && cp -a "$COMPAT/XLM/$hooks" "$keep/"
done
rm -rf "$COMPAT/XLM"
mkdir -p "$COMPAT"
"$tmp/xlm" install-steam-tool --xlm-updater-disable \
    --extra-launch-args="--run-as-steam-compat-tool=true --use-fallback-secret-provider --xlcore-web-release-url $RELEASE/ --aria-source url:$RELEASE/aria2c-arm64.tar.gz" \
    --steam-compat-path "$COMPAT/"
cp -a "$keep"/. "$COMPAT/XLM/"
rm -rf "$keep"

echo "Done. Back in gaming mode, on FINAL FANTASY XIV: Properties > Compatibility > force \"XLCore [XLM]\"."
