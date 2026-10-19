using System.Runtime.InteropServices;

using XIVLauncher.Common.PlatformAbstractions;

namespace XIVLauncher.Common.Unix;

public class UnixDalamudCompatibilityCheck : IDalamudCompatibilityCheck
{
    public void EnsureCompatibility()
    {
        //Dalamud will work with wines built-in vcrun, so no need to check that
        EnsureArchitecture();
    }

    private static void EnsureArchitecture()
    {
        var arch = RuntimeInformation.ProcessArchitecture;

        switch (arch)
        {
            case Architecture.X86:
                throw new IDalamudCompatibilityCheck.ArchitectureNotSupportedException("Dalamud is not supported on x86 architecture.");

            case Architecture.X64:
                break;

            case Architecture.Arm:
                throw new IDalamudCompatibilityCheck.ArchitectureNotSupportedException("Dalamud is not supported on ARM32.");

            case Architecture.Arm64:
                // Under an ARM64 Wine (ARM64EC), DXVK runs as native ARM64 code, and Dalamud's hook on
                // IDXGISwapChain::Present writes x64 code into it: the game dies on an illegal instruction.
                throw new IDalamudCompatibilityCheck.ArchitectureNotSupportedException("Dalamud is not supported on ARM64 yet.");
        }
    }
}
