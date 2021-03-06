﻿// /***************************************************************************
//  * SubbandLayer2Stereo.cs
//  * Copyright (c) 2015 the authors.
//  * 
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the GNU Lesser General Public License
//  * (LGPL) version 3 which accompanies this distribution, and is available at
//  * https://www.gnu.org/licenses/lgpl-3.0.en.html
//  *
//  * This library is distributed in the hope that it will be useful,
//  * but WITHOUT ANY WARRANTY; without even the implied warranty of
//  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  * Lesser General Public License for more details.
//  *
//  ***************************************************************************/

namespace RhythmCodex.Plugin.MP3Sharp.Lib.Decoding.Decoders.LayerII
{
    /// <summary>
    ///     Class for layer II subbands in stereo mode.
    /// </summary>
    internal sealed class SubbandLayer2Stereo : SubbandLayer2
    {
        private int channel2_allocation;

        private float[] channel2_c = { 0 };
        //protected boolean	 	channel2_grouping;  ???? Never used!
        private int[] channel2_codelength = { 0 };

        private float[] channel2_d = { 0 };
        //protected float[][] 	channel2_groupingtable = {{0},{0}};
        private float[] channel2_factor = { 0 };
        private float[] channel2_samples;
        private float channel2_scalefactor1;
        private float channel2_scalefactor2;
        private float channel2_scalefactor3;
        private int channel2_scfsi;

        /// <summary>
        ///     Constructor
        /// </summary>
        public SubbandLayer2Stereo(int subbandnumber)
            : base(subbandnumber)
        {
            channel2_samples = new float[3];
        }

        /// <summary>
        ///     *
        /// </summary>
        public override void ReadBitAllocation(Bitstream stream, Header header, Crc16 crc)
        {
            var length = get_allocationlength(header);
            allocation = stream.GetBitsFromBuffer(length);
            channel2_allocation = stream.GetBitsFromBuffer(length);
            if (crc != null)
            {
                crc.add_bits(allocation, length);
                crc.add_bits(channel2_allocation, length);
            }
        }

        /// <summary>
        ///     *
        /// </summary>
        public override void read_scalefactor_selection(Bitstream stream, Crc16 crc)
        {
            if (allocation != 0)
            {
                scfsi = stream.GetBitsFromBuffer(2);
                if (crc != null)
                    crc.add_bits(scfsi, 2);
            }
            if (channel2_allocation != 0)
            {
                channel2_scfsi = stream.GetBitsFromBuffer(2);
                if (crc != null)
                    crc.add_bits(channel2_scfsi, 2);
            }
        }

        /// <summary>
        ///     *
        /// </summary>
        public override void ReadScaleFactor(Bitstream stream, Header header)
        {
            base.ReadScaleFactor(stream, header);
            if (channel2_allocation != 0)
            {
                switch (channel2_scfsi)
                {
                    case 0:
                        channel2_scalefactor1 = ScaleFactors[stream.GetBitsFromBuffer(6)];
                        channel2_scalefactor2 = ScaleFactors[stream.GetBitsFromBuffer(6)];
                        channel2_scalefactor3 = ScaleFactors[stream.GetBitsFromBuffer(6)];
                        break;

                    case 1:
                        channel2_scalefactor1 = channel2_scalefactor2 = ScaleFactors[stream.GetBitsFromBuffer(6)];
                        channel2_scalefactor3 = ScaleFactors[stream.GetBitsFromBuffer(6)];
                        break;

                    case 2:
                        channel2_scalefactor1 =
                            channel2_scalefactor2 = channel2_scalefactor3 = ScaleFactors[stream.GetBitsFromBuffer(6)];
                        break;

                    case 3:
                        channel2_scalefactor1 = ScaleFactors[stream.GetBitsFromBuffer(6)];
                        channel2_scalefactor2 = channel2_scalefactor3 = ScaleFactors[stream.GetBitsFromBuffer(6)];
                        break;
                }
                prepare_sample_reading(header, channel2_allocation, 1, channel2_factor, channel2_codelength,
                    channel2_c, channel2_d);
            }
        }

        /// <summary>
        ///     *
        /// </summary>
        public override bool ReadSampleData(Bitstream stream)
        {
            var returnvalue = base.ReadSampleData(stream);

            if (channel2_allocation != 0)
                if (groupingtable[1] != null)
                {
                    var samplecode = stream.GetBitsFromBuffer(channel2_codelength[0]);
                    // create requantized samples:
                    samplecode += samplecode << 1;
                    /*
                    float[] target = channel2_samples;
                    float[] source = channel2_groupingtable[0];
                    int tmp = 0;
                    int temp = 0;
                    target[tmp++] = source[samplecode + temp];
                    temp++;
                    target[tmp++] = source[samplecode + temp];
                    temp++;
                    target[tmp] = source[samplecode + temp];
                    // memcpy (channel2_samples, channel2_groupingtable + samplecode, 3 * sizeof (real));
                    */
                    var target = channel2_samples;
                    var source = groupingtable[1];
                    var tmp = 0;
                    var temp = samplecode;
                    target[tmp] = source[temp];
                    temp++;
                    tmp++;
                    target[tmp] = source[temp];
                    temp++;
                    tmp++;
                    target[tmp] = source[temp];
                }
                else
                {
                    channel2_samples[0] =
                        (float)((stream.GetBitsFromBuffer(channel2_codelength[0])) * channel2_factor[0] - 1.0);
                    channel2_samples[1] =
                        (float)((stream.GetBitsFromBuffer(channel2_codelength[0])) * channel2_factor[0] - 1.0);
                    channel2_samples[2] =
                        (float)((stream.GetBitsFromBuffer(channel2_codelength[0])) * channel2_factor[0] - 1.0);
                }
            return returnvalue;
        }

        /// <summary>
        ///     *
        /// </summary>
        public override bool PutNextSample(int channels, SynthesisFilter filter1, SynthesisFilter filter2)
        {
            var returnvalue = base.PutNextSample(channels, filter1, filter2);
            if ((channel2_allocation != 0) && (channels != OutputChannels.LEFT_CHANNEL))
            {
                var sample = channel2_samples[samplenumber - 1];

                if (groupingtable[1] == null)
                    sample = (sample + channel2_d[0]) * channel2_c[0];

                if (groupnumber <= 4)
                    sample *= channel2_scalefactor1;
                else if (groupnumber <= 8)
                    sample *= channel2_scalefactor2;
                else
                    sample *= channel2_scalefactor3;
                if (channels == OutputChannels.BOTH_CHANNELS)
                    filter2.WriteSample(sample, subbandnumber);
                else
                    filter1.WriteSample(sample, subbandnumber);
            }
            return returnvalue;
        }
    }
}
