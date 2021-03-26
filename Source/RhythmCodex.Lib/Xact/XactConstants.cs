using System;

namespace RhythmCodex.Xact
{
    // source: https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Audio/Xact/XactHelpers.cs
    // license: ms-pl
    
    public static class XactConstants
    {
        public static float ParseDecibels(byte decibels)
        {
            //lazy 4-param fitting:
            //0xff 6.0
            //0xca 2.0
            //0xbf 1.0
            //0xb4 0.0
            //0x8f -4.0
            //0x5a -12.0
            //0x14 -38.0
            //0x00 -96.0
            const double a = -96.0;
            const double b = 0.432254984608615;
            const double c = 80.1748600297963;
            const double d = 67.7385212334047;
            var dB = (float) ((a - d) / (1 + Math.Pow(decibels / c, b)) + d);

            return dB;
        }

        public static float ParseVolumeFromDecibels(byte decibels)
        {
            //lazy 4-param fitting:
            //0xff 6.0
            //0xca 2.0
            //0xbf 1.0
            //0xb4 0.0
            //0x8f -4.0
            //0x5a -12.0
            //0x14 -38.0
            //0x00 -96.0
            const double a = -96.0;
            const double b = 0.432254984608615;
            const double c = 80.1748600297963;
            const double d = 67.7385212334047;
            var dB = (float) ((a - d) / (1 + Math.Pow(decibels / c, b)) + d);

            return ParseVolumeFromDecibels(dB);
        }

        public static float ParseVolumeFromDecibels(float decibels)
        {
            // Convert from decibels to linear volume.
            return (float) Math.Pow(10.0, decibels / 20.0);
        }
    }
}