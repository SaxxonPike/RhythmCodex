using System;

namespace CSCore
{
    public interface IFlacFile : IDisposable
    {
        int Read(byte[] buffer, int offset, int count);
        bool CanSeek { get; }
        WaveFormat WaveFormat { get; }
        long Position { get; set; }
        long Length { get; }
    }
}