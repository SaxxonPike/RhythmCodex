using System.Runtime.InteropServices;
using RhythmCodex.Graphics.Gdi.Streamers;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;
using RhythmCodex.Plugin.Sdl3.Infrastructure;
using SDL;
using static SDL.SDL3_image;
using static SDL.SDL3;

namespace RhythmCodex.Plugin.Sdl3.Graphics;

[Service]
public class BitmapStreamReader : IBitmapStreamReader
{
    public Bitmap Read(Stream stream)
    {
        unsafe
        {
            using var io = new IoStreamWrapper(stream);

            SDL_Surface* inSurface = null;
            SDL_Surface* convertedSurface = null;

            try
            {
                //
                // Load in the bitmap.
                //

                var surface = IMG_Load_IO((SDL_IOStream*)io.SdlIoStream, false);

                if (surface == null)
                    throw new SdlException();

                SDL_Surface* sourceSurface = null;

                //
                // If the pixel format isn't RGBA32, convert it now.
                //

                if (surface->format != SDL_PIXELFORMAT_RGBA32)
                {
                    convertedSurface = SDL_ConvertSurface(surface, SDL_PIXELFORMAT_RGBA32);

                    if (convertedSurface == null)
                        throw new SdlException();

                    sourceSurface = convertedSurface;
                }
                else
                {
                    sourceSurface = surface;
                }

                //
                // Copy the pixels to managed memory, one scanline at a time.
                //

                var scanWidth = sourceSurface->w * 4;

                var src = (byte*)sourceSurface->pixels;
                var srcPitch = sourceSurface->pitch;
                var srcSpan = new Span<byte>(src, sourceSurface->h * sourceSurface->pitch);

                var dst = new Bitmap(sourceSurface->w, sourceSurface->h);
                var dstPitch = dst.Width * 4;
                var dstSpan = MemoryMarshal.Cast<int, byte>(dst.Data.AsSpan());

                for (var y = 0; y < dst.Height; y++)
                {
                    srcSpan[..scanWidth].CopyTo(dstSpan);
                    srcSpan = srcSpan[srcPitch..];
                    dstSpan = dstSpan[dstPitch..];
                }

                return dst;
            }
            finally
            {
                //
                // Clean up allocated surfaces.
                //

                if (inSurface != null)
                    SDL_free(inSurface);

                if (convertedSurface != null)
                    SDL_free(convertedSurface);
            }
        }
    }
}