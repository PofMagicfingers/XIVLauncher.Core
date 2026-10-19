using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace XIVLauncher.Common.Unix.Compatibility;

/// <summary>
/// DroidDeck (https://github.com/Droid-Deck/DroidDeck) runs Steam on Android devices. Windows games go
/// through its "droiddeck-proton-arm64" compatibility tool: Valve's ARM64 Proton, which runs x64 code with
/// FEX, plus the device's game environment.
/// </summary>
public class DroidDeck
{
    public const string TOOL_NAME = "droiddeck-proton-arm64";

    /// <summary>FINAL FANTASY XIV's Steam app id, so Proton applies its fixes for the game.</summary>
    private const string FFXIV_APP_ID = "39210";

    public string SteamRoot { get; }

    /// <summary>DroidDeck's launcher for Proton, taking a Proton verb and the command.</summary>
    public string Launcher { get; }

    /// <summary>The ARM64 Proton the launcher runs.</summary>
    public string Proton { get; }

    public string WineServer => Path.Combine(Proton, "files", "bin-arm64", "wineserver");

    private DroidDeck(string steamRoot, string launcher, string proton)
    {
        SteamRoot = steamRoot;
        Launcher = launcher;
        Proton = proton;
    }

    /// <summary>
    /// Finds DroidDeck's compatibility tool and the newest ARM64 Proton installed, or returns null outside
    /// of DroidDeck.
    /// </summary>
    public static DroidDeck Find()
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            return null;

        var steamRoot = Environment.GetEnvironmentVariable("STEAM_COMPAT_CLIENT_INSTALL_PATH");
        if (string.IsNullOrEmpty(steamRoot))
        {
            var dataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
            if (string.IsNullOrEmpty(dataHome))
                dataHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
            steamRoot = Path.Combine(dataHome, "Steam");
        }

        var launcher = Path.Combine(steamRoot, "compatibilitytools.d", TOOL_NAME, "droiddeck-proton");
        if (!File.Exists(launcher))
            return null;

        var proton = FindProton(steamRoot);
        return proton == null ? null : new DroidDeck(steamRoot, launcher, proton);
    }

    private static string FindProton(string steamRoot)
    {
        var libraries = new List<string> { steamRoot };
        var libraryFolders = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
        if (File.Exists(libraryFolders))
        {
            libraries.AddRange(Regex.Matches(File.ReadAllText(libraryFolders), "^\\s*\"path\"\\s*\"(.*)\"\\s*$", RegexOptions.Multiline)
                                    .Select(m => m.Groups[1].Value));
        }

        return libraries.Distinct()
                        .Select(library => Path.Combine(library, "steamapps", "common"))
                        .Where(Directory.Exists)
                        .SelectMany(Directory.EnumerateDirectories)
                        .Where(dir => Directory.Exists(Path.Combine(dir, "files", "bin-arm64")) && File.Exists(Path.Combine(dir, "proton")))
                        .OrderByDescending(ProtonBuild)
                        .FirstOrDefault();
    }

    private static long ProtonBuild(string proton)
    {
        // "version" starts with the build's timestamp.
        var version = Path.Combine(proton, "version");
        if (!File.Exists(version))
            return 0;

        var first = File.ReadAllText(version).Split(' ', 2)[0].Trim();
        return long.TryParse(first, out var build) ? build : 0;
    }

    /// <summary>
    /// The environment Proton expects from Steam, with XIVLauncher's Wine prefix as the prefix ("pfx") of
    /// a compatibility data folder next to it.
    /// </summary>
    public Dictionary<string, string> ProtonEnvironment(DirectoryInfo prefix)
    {
        var compatData = Path.Combine(prefix.Parent.FullName, "proton-compatdata");
        Directory.CreateDirectory(compatData);

        var pfx = new FileInfo(Path.Combine(compatData, "pfx"));
        if (pfx.LinkTarget != prefix.FullName)
        {
            if (pfx.Exists || pfx.LinkTarget != null)
                pfx.Delete();
            File.CreateSymbolicLink(pfx.FullName, prefix.FullName);
        }

        var appId = Environment.GetEnvironmentVariable("STEAM_COMPAT_APP_ID");

        return new Dictionary<string, string>
        {
            { "STEAM_COMPAT_CLIENT_INSTALL_PATH", SteamRoot },
            { "STEAM_COMPAT_DATA_PATH", compatData },
            { "STEAM_COMPAT_INSTALL_PATH", Environment.GetEnvironmentVariable("STEAM_COMPAT_INSTALL_PATH") ?? prefix.FullName },
            { "STEAM_COMPAT_TOOL_PATHS", $"{Path.GetDirectoryName(Launcher)}:{Proton}" },
            { "STEAM_COMPAT_LIBRARY_PATHS", Environment.GetEnvironmentVariable("STEAM_COMPAT_LIBRARY_PATHS") ?? Path.Combine(SteamRoot, "steamapps") },
            { "STEAM_COMPAT_APP_ID", string.IsNullOrEmpty(appId) ? FFXIV_APP_ID : appId },
        };
    }

    /// <summary>
    /// Proton's steam.exe does not pass a program's output on, so a command whose output is read runs
    /// without it. The game runs as Steam runs it ("waitforexitandrun"), which applies DroidDeck's game
    /// environment; other commands run straight in the prefix.
    /// </summary>
    public static (string Verb, bool WithoutSteam) Verb(bool gameLaunch, bool outputRead)
    {
        return gameLaunch ? ("waitforexitandrun", outputRead) : ("runinprefix", false);
    }

    /// <summary>Proton maps the Z: drive to the root of the file system.</summary>
    public static string ToWindowsPath(string unixPath) => "Z:" + unixPath.Replace('/', '\\');
}
