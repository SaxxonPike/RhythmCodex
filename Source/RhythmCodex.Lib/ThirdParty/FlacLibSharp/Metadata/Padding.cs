using System;
using System.IO;
using FlacLibSharp.Exceptions;

namespace FlacLibSharp {
    /// <summary>
    /// Empty space inside the flac file ... a number of meaningless bits.
    /// </summary>
    public class Padding : MetadataBlock {

        private uint emptyBitCount;

        /// <summary>
        /// Creates an empty metadata block.
        /// </summary>
        public Padding()
        {
            Header.Type = MetadataBlockHeader.MetadataBlockType.Padding;
            emptyBitCount = 0;
        }

        /// <summary>
        /// Loads the padding data from the given data.
        /// </summary>
        public override void LoadBlockData(byte[] data) {
            emptyBitCount = (uint)(Header.MetaDataBlockLength * 8);
        }

        /// <summary>
        /// Writes the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            Header.WriteHeaderData(targetStream);
            
            // write a bunch of 0 bytes (probably shouldn't do this byte per byte ...)
            var bytes = emptyBitCount / 8;
            for (uint i = 0; i < bytes; i++)
            {
                targetStream.WriteByte(0);
            }
        }

        /// <summary>
        /// How many empty bits there are in the padding, must be a multiple of eight.
        /// </summary>
        public uint EmptyBitCount {
            get
            {
                return emptyBitCount;
            }
            set
            {
                if (value % 8 != 0)
                {
                    throw new FlacLibSharpInvalidPaddingBitCount(
                        $"Padding for {value} bits is impossible, the bitcount must be a multiple of eight.");
                }
            
                emptyBitCount = value;
                Header.MetaDataBlockLength = value / 8;
            }
        }

    }
}
