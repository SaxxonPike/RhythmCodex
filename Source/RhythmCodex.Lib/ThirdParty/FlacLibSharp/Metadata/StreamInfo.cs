using System;
using System.IO;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A metadata block that contains information on the actual stream.
    /// </summary>
    public class StreamInfo : MetadataBlock {

        #region Private Fields

        private ushort minimumBlockSize;
        private ushort maximumBlockSize;
        // Is actually an Int24 but we won't bother creating that type now... maybe later
        private uint minimumFrameSize;
        private uint maximumFrameSize;
        // Is actually 20 bit: "Sample rate in Hz. Though 20 bits are available, the maximum sample rate is limited by the structure of frame headers to 655350Hz. Also, a value of 0 is invalid."
        private uint sampleRateHz;
        // 3-bit value: maximum 8 channels
        private short channels;
        // 5-bit value: 4 to 32 bits per sample
        private short bitsPerSample;
        // 36 byte (!) value: 
        private long samples;
        // Should contain 16 bytes...
        private byte[] md5Signature;

        #endregion

        /// <summary>
        /// Creates a new StreamInfo.
        /// </summary>
        public StreamInfo()
        {
            Header.Type = MetadataBlockHeader.MetadataBlockType.StreamInfo;
        }

        /// <summary>
        /// Loads a new stream info block from the provided data.
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data) {
            // "All numbers are big-endian coded and unsigned".

            // 1: Minimum Block Size (first 16-bit)
            minimumBlockSize = BinaryDataHelper.GetUInt16(data, 0);
            maximumBlockSize = BinaryDataHelper.GetUInt16(data, 2);
            minimumFrameSize = BinaryDataHelper.GetUInt24(data, 4);
            maximumFrameSize = BinaryDataHelper.GetUInt24(data, 7);
            // Interpret 20 bits starting from byte 10 as a UInt
            sampleRateHz = (uint)BinaryDataHelper.GetUInt64(data, 10, 20);
            channels = (short)(BinaryDataHelper.GetUInt64(data, 12, 3, 4) + 1);
            bitsPerSample = (short)(BinaryDataHelper.GetUInt64(data, 12, 5, 7) + 1);
            samples = (long)BinaryDataHelper.GetUInt64(data, 13, 36, 4);
            md5Signature = new byte[16];
            Array.Copy(data, 18, md5Signature, 0, 16);
        }

        /// <summary>
        /// Writes the data describing this metadata to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            Header.WriteHeaderData(targetStream);

            targetStream.Write(BinaryDataHelper.GetBytesUInt16(minimumBlockSize), 0, 2);
            targetStream.Write(BinaryDataHelper.GetBytesUInt16(maximumBlockSize), 0, 2);
            targetStream.Write(BinaryDataHelper.GetBytes(minimumFrameSize, 3), 0, 3);
            targetStream.Write(BinaryDataHelper.GetBytes(maximumFrameSize, 3), 0, 3);

            // next are 64 bits containing:
            // * sample rate in Hz (20-bits)
            // * number of channels (3 bits)
            // * bits per sample (5 bits)
            // * total samples per stream (36 bits)
            var combinedData = (ulong)SampleRateHz << 44;
            combinedData = combinedData + ((ulong)(channels-1) << 41);
            combinedData = combinedData + ((ulong)(bitsPerSample-1) << 36);
            combinedData = combinedData + (ulong)samples;

            targetStream.Write(BinaryDataHelper.GetBytes(combinedData, 8), 0, 8);
            targetStream.Write(md5Signature, 0, 16);
        }

        #region Public Properties

        /// <summary>
        /// The minimum block size, in samples, used in the stream.
        /// </summary>
        public ushort MinimumBlockSize {
            get { return minimumBlockSize; }
        }

        /// <summary>
        /// The maximum block size, in samples, used in the stream. 
        /// </summary>
        /// <remarks>Minimum blocksize == maximum blocksize implies a fixed-blocksize stream.</remarks>
        public ushort MaximumBlockSize {
            get { return maximumBlockSize; }
        }

        /// <summary>
        /// The minimum frame size, in bytes, used in the stream.
        /// </summary>
        /// <remarks>May be 0 to imply the value is not known.</remarks>
        public uint MinimumFrameSize {
            get { return minimumFrameSize; }
        }

        /// <summary>
        /// The maximum frame size, in bytes, used in the stream. 
        /// </summary>
        /// <remarks>May be 0 to imply the value is not known.</remarks>
        public uint MaximumFrameSize {
            get { return maximumFrameSize; }
        }

        /// <summary>
        /// Sample rate in Hz. 
        /// </summary>
        /// <remarks>Though 20 bits are available, the maximum sample rate is limited by the structure of frame headers to 655350Hz. A value of 0 is invalid.</remarks>
        public uint SampleRateHz {
            get { return sampleRateHz; }
        }

        /// <summary>
        /// Number of channels -1.
        /// </summary>
        /// <remarks>FLAC supports from 1 to 8 channels.</remarks>
        public short Channels {
            get { return channels; }
        }

        /// <summary>
        /// Bits per sample -1.
        /// </summary>
        /// <remarks>FLAC supports from 4 to 32 bits per sample. Currently the reference encoder and decoders only support up to 24 bits per sample.</remarks>
        public short BitsPerSample {
            get { return bitsPerSample; }
        }

        /// <summary>
        /// Total samples in stream.
        /// </summary>
        /// <remarks>'Samples' means inter-channel sample, i.e. one second of 44.1Khz audio will have 44100 samples regardless of the number of channels. A value of zero here means the number of total samples is unknown.</remarks>
        public long Samples {
            get { return samples; }
        }

        /// <summary>
        /// MD5 signature (16 byte) of the unencoded audio data.
        /// </summary>
        /// <remarks>This allows the decoder to determine if an error exists in the audio data even when the error does not result in an invalid bitstream.</remarks>
        public byte[] MD5Signature {
            get { return md5Signature; }
        }

        /// <summary>
        /// The duration of the audio in seconds, calculated based on the stream info.
        /// </summary>
        public int Duration
        {
            get
            {
                return (int)Math.Round((double)(Samples / SampleRateHz));
            }
        }

        #endregion
    
    }
}
