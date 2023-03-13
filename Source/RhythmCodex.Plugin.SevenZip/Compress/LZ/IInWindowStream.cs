using System;

namespace RhythmCodex.Plugin.SevenZip.Compress.LZ;

interface IInWindowStream
{
    void SetStream(System.IO.Stream inStream);
    void Init();
    void ReleaseStream();
    Byte GetIndexByte(Int32 index);
    UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit);
    UInt32 GetNumAvailableBytes();
}