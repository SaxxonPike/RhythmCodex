using RhythmCodex.Graphics.Models;
using RhythmCodex.Graphics.Streamers;
using RhythmCodex.IoC;
using RhythmCodex.Plugin.Sdl3.Infrastructure;
using SDL;
using static SDL.SDL3_image;
using static SDL.SDL3;

namespace RhythmCodex.Plugin.Sdl3.Graphics;

[Service]
public class BitmapStreamWriter : IBitmapStreamWriter
{
    public void Write(Stream stream, Bitmap bitmap)
    {
        unsafe
        {
            using var io = new IoStreamWrapper(stream);

            SDL_Surface* surface = null;

            try
            {
                //
                // Create the surface.
                //

                fixed (void* ptr = bitmap.Data)
                {
                    surface = SDL_CreateSurfaceFrom(
                        bitmap.Width,
                        bitmap.Height,
                        SDL_PIXELFORMAT_RGBA32,
                        (IntPtr)ptr,
                        bitmap.Width * 4
                    );

                    if (surface == null)
                        throw new SdlException();
                }
                
                //
                // Save the surface to the stream.
                //

                if (!IMG_SavePNG_IO(surface, (SDL_IOStream*)io.SdlIoStream, false))
                    throw new SdlException();
            }
            finally
            {
                //
                // Clean up allocated surfaces.
                //

                if (surface != null)
                    SDL_free(surface);
            }
        }
    }
}