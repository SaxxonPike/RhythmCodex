using JetBrains.Annotations;
using RhythmCodex.Infrastructure;
using static SDL.SDL3_mixer;
using static SDL.SDL3;

namespace RhythmCodex.Plugin.Sdl3;

[PublicAPI]
internal static class Init
{
    // For the time being, plugins do not have any form of lifetime management.
    // Thus, we need to track whether we have initialized SDL subsystems.

    private static readonly bool Loaded = Load();

    private static bool Load()
    {
        SDL_Init(0);
        MIX_Init();
        AppDomain.CurrentDomain.UnhandledException += HandleQuit;
        AppDomain.CurrentDomain.ProcessExit += HandleQuit!;
        return true;
    }

    private static void HandleQuit(object o, EventArgs unhandledExceptionEventArgs)
    {
        MIX_Quit();
        SDL_Quit();
    }

    public static void EnsureLoaded()
    {
        if (!Loaded)
            throw new RhythmCodexException("Failed to initialize SDL3");
    }
}