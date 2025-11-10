using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SDL;
using static SDL.SDL3;

namespace RhythmCodex.Plugin.Sdl3.Infrastructure;

/// <summary>
/// Wraps an <see cref="SDL_IOStream"/> around a managed <see cref="Stream"/>.
/// </summary>
[PublicAPI]
[MustDisposeResource]
internal sealed unsafe class IoStreamWrapper : IDisposable
{
    /// <summary>
    /// SDL user data value.
    /// </summary>
    public IntPtr UserData { get; }
    
    /// <summary>
    /// <see cref="SDL_IOStream"/> reference.
    /// </summary>
    public IntPtr SdlIoStream { get; }
    
    /// <summary>
    /// Managed <see cref="Stream"/> reference.
    /// </summary>
    public Stream BaseStream { get; }

    /// <summary>
    /// Creates an <see cref="SDL_IOStream"/> wrapper around a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">
    /// Managed <see cref="Stream"/> to wrap.
    /// </param>
    public IoStreamWrapper(
        Stream stream
    )
    {
        SDL_INIT_INTERFACE<SDL_IOStreamInterface>(out var io);

        io.close = &IngressClose;
        io.flush = &IngressFlush;
        io.read = &IngressRead;
        io.seek = &IngressSeek;
        io.size = &IngressSize;
        io.write = &IngressWrite;

        BaseStream = stream;
        UserData = UserDataStore.Add(stream);
        SdlIoStream = (IntPtr)SDL_OpenIO(&io, UserData);
    }

    /// <summary>
    /// Size function exposed to SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static long IngressSize(
        IntPtr userdata)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            return stream.Length;
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Seek function exposed to SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static long IngressSeek(
        IntPtr userdata,
        long offset,
        SDL_IOWhence whence)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            return stream.Seek(offset, whence switch
            {
                SDL_IOWhence.SDL_IO_SEEK_SET => SeekOrigin.Begin,
                SDL_IOWhence.SDL_IO_SEEK_CUR => SeekOrigin.Current,
                SDL_IOWhence.SDL_IO_SEEK_END => SeekOrigin.End,
                _ => throw new ArgumentOutOfRangeException(nameof(whence), whence, null)
            });
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Read function exposed to SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static UIntPtr IngressRead(
        IntPtr userdata,
        IntPtr ptr,
        UIntPtr size,
        SDL_IOStatus* status)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            if (!stream.CanRead)
            {
                *status = SDL_IOStatus.SDL_IO_STATUS_WRITEONLY;
                return 0;
            }

            if (stream.CanSeek && stream.Position >= stream.Length)
            {
                *status = SDL_IOStatus.SDL_IO_STATUS_EOF;
                return 0;
            }

            var amount = (UIntPtr)stream.Read(
                new Span<byte>((void*)ptr, (int)size)
            );

            return amount;
        }
        catch
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            return 0;
        }
    }

    /// <summary>
    /// Write function exposed to SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static UIntPtr IngressWrite(
        IntPtr userdata,
        IntPtr ptr,
        UIntPtr size,
        SDL_IOStatus* status)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            if (!stream.CanWrite)
            {
                *status = SDL_IOStatus.SDL_IO_STATUS_READONLY;
                return 0;
            }

            if (stream.CanSeek && stream.Position >= stream.Length)
            {
                *status = SDL_IOStatus.SDL_IO_STATUS_EOF;
                return 0;
            }

            stream.Write(
                new Span<byte>((void*)ptr, (int)size)
            );

            return size;
        }
        catch
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            return 0;
        }
    }

    /// <summary>
    /// Flush function exposed to SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool IngressFlush(
        IntPtr userdata,
        SDL_IOStatus* status)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            stream.Flush();
            return true;
        }
        catch
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            return false;
        }
    }

    /// <summary>
    /// Close function exposed to SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool IngressClose(
        IntPtr userdata)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            stream.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}