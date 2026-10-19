using System;
using System.Threading.Tasks;

using Steamworks;

using XIVLauncher.Common.PlatformAbstractions;

namespace XIVLauncher.Common.Unix
{
    public class UnixSteam : ISteam
    {
        public UnixSteam()
        {
            SteamUtils.OnGamepadTextInputDismissed += b => OnGamepadTextInputDismissed?.Invoke(b);
        }

        public void Initialize(uint appId)
        {
            // workaround because SetEnvironmentVariable doesn't actually touch the process environment on unix
            [System.Runtime.InteropServices.DllImport("c")]
            static extern int setenv(string name, string value, int overwrite);

            [System.Runtime.InteropServices.DllImport("c")]
            static extern int unsetenv(string name);

            [System.Runtime.InteropServices.DllImport("c")]
            static extern IntPtr getenv(string name);

            static void Restore(string name, string value)
            {
                if (value == null)
                    unsetenv(name);
                else
                    setenv(name, value, 1);
            }

            // Steam (and Gamescope) tell the game apart by these ids. If initialising fails, keep the
            // ones the launcher was started with, so the game is not tagged with an id that never ran.
            var previousAppId = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(getenv("SteamAppId"));
            var previousGameId = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(getenv("SteamGameId"));

            setenv("SteamAppId", appId.ToString(), 1);
            setenv("SteamGameId", appId.ToString(), 1);

            try
            {
                SteamClient.Init(appId);
            }
            catch
            {
                Restore("SteamAppId", previousAppId);
                Restore("SteamGameId", previousGameId);
                throw;
            }
        }

        public bool IsValid => SteamClient.IsValid;

        public bool BLoggedOn => SteamClient.IsLoggedOn;

        public bool BOverlayNeedsPresent => SteamUtils.DoesOverlayNeedPresent;

        public void Shutdown()
        {
            SteamClient.Shutdown();
        }

        public async Task<byte[]?> GetAuthSessionTicketAsync()
        {
            var ticket = await SteamUser.GetAuthSessionTicketAsync().ConfigureAwait(true);
            return ticket?.Data;
        }

        public bool IsAppInstalled(uint appId)
        {
            return SteamApps.IsAppInstalled(appId);
        }

        public string GetAppInstallDir(uint appId)
        {
            return SteamApps.AppInstallDir(appId);
        }

        public bool ShowGamepadTextInput(bool password, bool multiline, string description, int maxChars, string existingText = "")
        {
            return SteamUtils.ShowGamepadTextInput(password ? GamepadTextInputMode.Password : GamepadTextInputMode.Normal, multiline ? GamepadTextInputLineMode.MultipleLines : GamepadTextInputLineMode.SingleLine, description, maxChars, existingText);
        }

        public string GetEnteredGamepadText()
        {
            return SteamUtils.GetEnteredGamepadText();
        }

        public bool ShowFloatingGamepadTextInput(ISteam.EFloatingGamepadTextInputMode mode, int x, int y, int width, int height)
        {
            SteamUtils.ShowFloatingGamepadTextInput((TextInputMode)mode, x, y, width, height);
            return true;
        }

        public bool IsRunningOnSteamDeck() => SteamUtils.IsRunningOnSteamDeck;

        public uint GetServerRealTime() => (uint)((DateTimeOffset)SteamUtils.SteamServerTime).ToUnixTimeSeconds();

        public void ActivateGameOverlayToWebPage(string url, bool modal = false)
        {
            SteamFriends.OpenWebOverlay(url, modal);
        }

        public event Action<bool> OnGamepadTextInputDismissed;
    }
}
