# XIVLauncher.Core on DroidDeck (ARM64)

Packaging that runs XIVLauncher.Core natively on ARM64 inside [DroidDeck](https://github.com/Droid-Deck/DroidDeck)
(ARM64 Steam in a proot on Android, ARM64 Proton with FEX for x64 code), through XLM
([fork with aarch64 support](https://github.com/PofMagicfingers/xlm/tree/droiddeck-aarch64)).

## Install

From DroidDeck's desktop Konsole:

```sh
curl -fsSL https://github.com/PofMagicfingers/XIVLauncher.Core/releases/latest/download/install.sh | bash
```

Then back in gaming mode, on FINAL FANTASY XIV (or the free trial): Properties > Compatibility >
force "XLCore [XLM]", empty launch options.

Status: the game runs in gaming mode. Not working yet: XIVLauncher in DroidDeck's desktop session
(`SDL_CreateGPUDevice(): No supported SDL_GPU backend found`). Not tested yet: Dalamud with XIVLauncher's DXVK.

## Files

| File | What |
| --- | --- |
| `XIVLauncher.Core` | Launcher: runs the apphost with a bundled glibc (DroidDeck's Debian has 2.31, the Hexa.NET natives need 2.38). The loader sits next to the apphost so .NET finds its app directory through `/proc/self/exe`. |
| `proton-arm64/` | XIVLauncher's `wine64`/`wineserver` (Wine install "Custom"): route everything through DroidDeck's ARM64 Proton. |
| `aria2c-wrapper` | Wraps a static aarch64 aria2c with `--async-dns=false` (c-ares finds no DNS server in DroidDeck). |
| `install.sh` | Installs XLM as a Steam compatibility tool pointing at this fork's latest release. |

## Why the Proton wrapper does what it does

- Proton's `steam.exe` does not pass the program's stdout on, and XIVLauncher reads it: `winepath`
  is emulated (Proton maps `Z:` to `/`), Dalamud's injector starts without `steam.exe`
  (`DROIDDECK_NO_STEAM=1`) and other helpers use `runinprefix`.
- The game starts with `waitforexitandrun`, like a Steam launch, so DroidDeck applies its game
  environment and components.
- XIVLauncher overwrites `SteamAppId`/`SteamGameId` with each id it tries to initialise Steam with
  (312060, the free trial, last). Gamescope only shows windows tagged with the id Steam launched,
  so the game stayed behind a black screen: both are reset to `STEAM_COMPAT_APP_ID`.
- The game uses XIVLauncher's own DXVK (x64), as on x64: Dalamud hooks `IDXGISwapChain::Present`
  by patching x86 code, which crashes in Proton's ARM64EC DXVK.

## Release contents

`version`, `XIVLauncher.Core.tar.gz` (the `linux-arm64` publish, non-single-file, with the
launcher, `ld-linux-aarch64.so.1`, `glibc/` from Debian trixie and `proton-arm64/`),
`xlm-aarch64`, `aria2c-arm64.tar.gz` ([abcfy2/aria2-static-build](https://github.com/abcfy2/aria2-static-build)
1.37.0) and `install.sh`.
