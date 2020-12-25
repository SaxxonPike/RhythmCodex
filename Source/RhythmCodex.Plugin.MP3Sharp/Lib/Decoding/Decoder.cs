// /***************************************************************************
//  * Decoder.cs
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
using RhythmCodex.Plugin.MP3Sharp.Lib.Decoding.Decoders;

namespace RhythmCodex.Plugin.MP3Sharp.Lib.Decoding
{
    /// <summary>
    ///     Encapsulates the details of decoding an MPEG audio frame.
    /// </summary>
    internal sealed class Decoder
    {
        private static readonly Params DEFAULT_PARAMS = new Params();
        private readonly Params params_Renamed;
        //private Equalizer m_Equalizer;

        private SynthesisFilter m_LeftChannelFilter;
        private SynthesisFilter m_RightChannelFilter;

        private bool m_IsInitialized;
        private LayerIDecoder m_L1Decoder;
        private LayerIIDecoder m_L2Decoder;
        private LayerIIIDecoder m_L3Decoder;

        private ABuffer m_Output;

        /// <summary>
        ///     Creates a new Decoder instance with default parameters.
        /// </summary>
        public Decoder() : this(null)
        {
            InitBlock();
        }

        /// <summary>
        ///     Creates a new Decoder instance with custom parameters.
        /// </summary>
        public Decoder(Params params0)
        {
            InitBlock();
            if (params0 == null)
                params0 = DEFAULT_PARAMS;

            params_Renamed = params0;

//            var eq = params_Renamed.InitialEqualizerSettings;
//            if (eq != null)
//            {
//                m_Equalizer.FromEqualizer = eq;
//            }
        }

        public static Params DefaultParams => (Params) DEFAULT_PARAMS.Clone();

//        public Equalizer Equalizer
//        {
//            set
//            {
//                if (value == null)
//                    value = Equalizer.PASS_THRU_EQ;
//
//                //m_Equalizer.FromEqualizer = value;
//
//                //var factors = m_Equalizer.BandFactors;
////                if (m_LeftChannelFilter != null)
////                    m_LeftChannelFilter.EQ = factors;
////
////                if (m_RightChannelFilter != null)
////                    m_RightChannelFilter.EQ = factors;
//            }
//        }

        /// <summary>
        ///     Changes the output buffer. This will take effect the next time
        ///     decodeFrame() is called.
        /// </summary>
        public ABuffer OutputBuffer
        {
            set => m_Output = value;
        }

        /// <summary>
        ///     Retrieves the sample frequency of the PCM samples output
        ///     by this decoder. This typically corresponds to the sample
        ///     rate encoded in the MPEG audio stream.
        /// </summary>
        public int OutputFrequency { get; private set; }

        /// <summary>
        ///     Retrieves the number of channels of PCM samples output by
        ///     this decoder. This usually corresponds to the number of
        ///     channels in the MPEG audio stream.
        /// </summary>
        public int OutputChannels { get; private set; }

        /// <summary>
        ///     Retrieves the maximum number of samples that will be written to
        ///     the output buffer when one frame is decoded. This can be used to
        ///     help calculate the size of other buffers whose size is based upon
        ///     the number of samples written to the output buffer. NB: this is
        ///     an upper bound and fewer samples may actually be written, depending
        ///     upon the sample rate and number of channels.
        /// </summary>
        public int OutputBlockSize => ABuffer.OBUFFERSIZE;

        private void InitBlock()
        {
            //m_Equalizer = new Equalizer();
        }

        /// <summary>
        ///     Decodes one frame from an MPEG audio bitstream.
        /// </summary>
        /// <param name="header">
        ///     Header describing the frame to decode.
        /// </param>
        /// <param name="stream">
        ///     Bistream that provides the bits for the body of the frame.
        /// </param>
        /// <returns>
        ///     A SampleBuffer containing the decoded samples.
        /// </returns>
        public ABuffer DecodeFrame(Header header, Bitstream stream)
        {
            if (!m_IsInitialized)
            {
                Initialize(header);
            }

            var layer = header.layer();

            m_Output.ClearBuffer();

            var decoder = RetrieveDecoder(header, stream, layer);

            decoder.DecodeFrame();

            m_Output.WriteBuffer(1);

            return m_Output;
        }

        private DecoderException NewDecoderException(int errorcode)
        {
            return new DecoderException(errorcode, null);
        }

        private DecoderException NewDecoderException(int errorcode, Exception throwable)
        {
            return new DecoderException(errorcode, throwable);
        }

        private IFrameDecoder RetrieveDecoder(Header header, Bitstream stream, int layer)
        {
            IFrameDecoder decoder = null;

            // REVIEW: allow channel output selection type
            // (LEFT, RIGHT, BOTH, DOWNMIX)
            switch (layer)
            {
                case 3:
                    if (m_L3Decoder == null)
                    {
                        m_L3Decoder = new LayerIIIDecoder(stream, header, m_LeftChannelFilter, m_RightChannelFilter, m_Output,
                            (int) OutputChannelsEnum.BOTH_CHANNELS);
                    }

                    decoder = m_L3Decoder;
                    break;

                case 2:
                    if (m_L2Decoder == null)
                    {
                        m_L2Decoder = new LayerIIDecoder();
                        m_L2Decoder.Create(stream, header, m_LeftChannelFilter, m_RightChannelFilter, m_Output,
                            (int) OutputChannelsEnum.BOTH_CHANNELS);
                    }
                    decoder = m_L2Decoder;
                    break;

                case 1:
                    if (m_L1Decoder == null)
                    {
                        m_L1Decoder = new LayerIDecoder();
                        m_L1Decoder.Create(stream, header, m_LeftChannelFilter, m_RightChannelFilter, m_Output,
                            (int) OutputChannelsEnum.BOTH_CHANNELS);
                    }
                    decoder = m_L1Decoder;
                    break;
            }

            if (decoder == null)
            {
                throw NewDecoderException(DecoderErrors.UNSUPPORTED_LAYER, null);
            }

            return decoder;
        }

        private void Initialize(Header header)
        {
            // REVIEW: allow customizable scale factor
            var scalefactor = 32700.0f;

            var mode = header.mode();
            var layer = header.layer();
            var channels = mode == Header.SINGLE_CHANNEL ? 1 : 2;

            // set up output buffer if not set up by client.
            if (m_Output == null)
                m_Output = new SampleBuffer(header.frequency(), channels);

            //var factors = m_Equalizer.BandFactors;
            //Console.WriteLine("NOT CREATING SYNTHESIS FILTERS");
            m_LeftChannelFilter = new SynthesisFilter(0, scalefactor);

            // REVIEW: allow mono output for stereo
            if (channels == 2)
                m_RightChannelFilter = new SynthesisFilter(1, scalefactor);

            OutputChannels = channels;
            OutputFrequency = header.frequency();

            m_IsInitialized = true;
        }

        /// <summary>
        ///     The Params class presents the customizable
        ///     aspects of the decoder. Instances of this class are not thread safe.
        /// </summary>
        internal class Params : ICloneable
        {
//            private Equalizer m_Equalizer;
            private OutputChannels m_OutputChannels;

            public OutputChannels OutputChannels
            {
                get => m_OutputChannels;

                set => m_OutputChannels = value ?? throw new NullReferenceException("out");
            }

//            /// <summary>
//            ///     Retrieves the equalizer settings that the decoder's equalizer
//            ///     will be initialized from.
//            ///     The Equalizer instance returned
//            ///     cannot be changed in real time to affect the
//            ///     decoder output as it is used only to initialize the decoders
//            ///     EQ settings. To affect the decoder's output in realtime,
//            ///     use the Equalizer returned from the getEqualizer() method on
//            ///     the decoder.
//            /// </summary>
//            /// <returns>
//            ///     The Equalizer used to initialize the
//            ///     EQ settings of the decoder.
//            /// </returns>
//            public Equalizer InitialEqualizerSettings
//            {
//                get { return m_Equalizer; }
//            }

            public object Clone()
            {
                try
                {
                    return MemberwiseClone();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(this + ": " + ex);
                }
            }
        }
    }
}