﻿namespace RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC;

internal class FlacPartitionedRiceContent
{
    public int[] Parameters;
    public int[] RawBits;

    private int _capByOrder = -1;

    public void UpdateSize(int partitionOrder)
    {
        if (_capByOrder < partitionOrder)
        {
            var size = 1 << partitionOrder;
            Parameters = new int[size];
            RawBits = new int[size];

            _capByOrder = partitionOrder;
        }
    }
}