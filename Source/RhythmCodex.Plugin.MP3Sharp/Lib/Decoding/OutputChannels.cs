// /***************************************************************************
//  * OutputChannels.cs
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

using System;

namespace RhythmCodex.Plugin.MP3Sharp.Lib.Decoding
{
    /// <summary>
    ///     A Type-safe representation of the the supported output channel
    ///     constants. This class is immutable and, hence, is thread safe.
    /// </summary>
    /// <author>
    ///     Mat McGowan
    /// </author>
    internal sealed class OutputChannels
    {
        /// <summary>
        ///     Flag to indicate output should include both channels.
        /// </summary>
        public static int BOTH_CHANNELS = 0;

        /// <summary>
        ///     Flag to indicate output should include the left channel only.
        /// </summary>
        public static int LEFT_CHANNEL = 1;

        /// <summary>
        ///     Flag to indicate output should include the right channel only.
        /// </summary>
        public static int RIGHT_CHANNEL = 2;

        /// <summary>
        ///     Flag to indicate output is mono.
        /// </summary>
        public static int DOWNMIX_CHANNELS = 3;

        public static readonly OutputChannels LEFT = new(LEFT_CHANNEL);
        public static readonly OutputChannels RIGHT = new(RIGHT_CHANNEL);
        public static readonly OutputChannels BOTH = new(BOTH_CHANNELS);
        public static readonly OutputChannels DOWNMIX = new(DOWNMIX_CHANNELS);

        private OutputChannels(int channels)
        {
            ChannelsOutputCode = channels;

            if (channels < 0 || channels > 3)
                throw new ArgumentException("channels");
        }

        /// <summary>
        ///     Retrieves the code representing the desired output channels.
        ///     Will be one of LEFT_CHANNEL, RIGHT_CHANNEL, BOTH_CHANNELS
        ///     or DOWNMIX_CHANNELS.
        /// </summary>
        /// <returns>
        ///     the channel code represented by this instance.
        /// </returns>
        public int ChannelsOutputCode { get; }

        /// <summary>
        ///     Retrieves the number of output channels represented
        ///     by this channel output type.
        /// </summary>
        /// <returns>
        ///     The number of output channels for this channel output
        ///     type. This will be 2 for BOTH_CHANNELS only, and 1
        ///     for all other types.
        /// </returns>
        public int ChannelCount
        {
            get
            {
                var count = (ChannelsOutputCode == BOTH_CHANNELS) ? 2 : 1;
                return count;
            }
        }

        /// <summary>
        ///     Creates an OutputChannels instance
        ///     corresponding to the given channel code.
        /// </summary>
        /// <param name="code">
        ///     one of the OutputChannels channel code constants.
        ///     @throws	IllegalArgumentException if code is not a valid
        ///     channel code.
        /// </param>
        public static OutputChannels fromInt(int code)
        {
            switch (code)
            {
                case (int) OutputChannelsEnum.LEFT_CHANNEL:
                    return LEFT;

                case (int) OutputChannelsEnum.RIGHT_CHANNEL:
                    return RIGHT;

                case (int) OutputChannelsEnum.BOTH_CHANNELS:
                    return BOTH;

                case (int) OutputChannelsEnum.DOWNMIX_CHANNELS:
                    return DOWNMIX;

                default:
                    throw new ArgumentException("Invalid channel code: " + code);
            }
        }

        public override bool Equals(object o)
        {
            var equals = false;

            if (o is OutputChannels)
            {
                var oc = (OutputChannels) o;
                equals = (oc.ChannelsOutputCode == ChannelsOutputCode);
            }

            return equals;
        }

        public override int GetHashCode()
        {
            return ChannelsOutputCode;
        }
    }
}