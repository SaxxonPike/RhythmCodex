using RhythmCodex.Infrastructure;
using static SDL.SDL3;

namespace RhythmCodex.Plugin.Sdl3;

public class SdlException() : RhythmCodexException($"Sdl3: {SDL_GetError()}");