namespace CSCore.Codecs.FLAC
{
    internal unsafe class FlacSubFrameData
    {
        public int* DestinationBuffer;
        public int* ResidualBuffer;
        public readonly FlacPartitionedRiceContent Content = new FlacPartitionedRiceContent();
    }
}