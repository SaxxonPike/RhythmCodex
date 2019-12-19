using System;

namespace RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC
{
    internal class FlacSubFrameData
    {
        public Memory<int> DestinationBuffer;
        public Memory<int> ResidualBuffer;
        public readonly FlacPartitionedRiceContent Content = new FlacPartitionedRiceContent();
    }
}