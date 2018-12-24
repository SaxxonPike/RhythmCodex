using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A metadata block header.
    /// </summary>
    public class MetadataBlockHeader {

        /// <summary>
        /// Defines the type of meta data.
        /// </summary>
        public enum MetadataBlockType {
            /// <summary>
            /// An unknown type of metadata.
            /// </summary>
            None = 7,
            /// <summary>
            /// Information on the flac audio stream.
            /// </summary>
            StreamInfo = 0,
            /// <summary>
            /// A metadata block that pads some space. It has no further meaning.
            /// </summary>
            Padding = 1,
            /// <summary>
            /// A metadata block with application specific information.
            /// </summary>
            Application = 2,
            /// <summary>
            /// A metadata block that has some information for seektables.
            /// </summary>
            Seektable = 3,
            /// <summary>
            /// A metadata block that contains the vorbis comments (artist, field, ...)
            /// </summary>
            VorbisComment = 4,
            /// <summary>
            /// A metadata block containing cue sheet information.
            /// </summary>
            CueSheet = 5,
            /// <summary>
            /// A metadata block with picture information.
            /// </summary>
            Picture = 6,
            /// <summary>
            /// A metadata block that is not valid or could not be parsed.
            /// </summary>
            Invalid
        }

        public MetadataBlockHeader()
        {
            Type = MetadataBlockType.None;
            metaDataBlockLength = 0;
        }

        /// <summary>
        /// Creates a new metadata block header from the provided data.
        /// </summary>
        /// <param name="data"></param>
        public MetadataBlockHeader(byte[] data) {
            ParseData(data);
        }

        /// <summary>
        /// Writes the metadata to the given stream.
        /// </summary>
        /// <param name="targetStream">The stream where the data will be written to.</param>
        public void WriteHeaderData(Stream targetStream) {
            var data = isLastMetaDataBlock ? (byte)128 : (byte)0; // The 128 because the last metadata flag is the most significant bit set to 1 ...
            data += (byte)(typeID & 0x7F); // We make sure to chop off the last bit

            targetStream.WriteByte(data);

            // 24-bit metaDataBlockLength
            targetStream.Write(BinaryDataHelper.GetBytes((ulong)metaDataBlockLength, 3), 0, 3);
        }

        private bool isLastMetaDataBlock;

        private int typeID;

        /// <summary>
        /// Indicates if this is the last metadata block in the file (meaning that it is followed by the actual audio stream).
        /// </summary>
        public bool IsLastMetaDataBlock {
            get { return isLastMetaDataBlock; }
            set { isLastMetaDataBlock = value; }
        }

        private MetadataBlockType type;

        /// <summary>
        /// Defines what kind of metadatablock this is.
        /// </summary>
        public MetadataBlockType Type {
            get { return type; }
            set {
                type = value;
                typeID = (int)value;
            }
        }

        private uint metaDataBlockLength;

        /// <summary>
        /// Defines the length of the metadata block.
        /// </summary>
        public uint MetaDataBlockLength {
            get { return metaDataBlockLength; }
            set { metaDataBlockLength = value; }
        }

        /// <summary>
        /// Interprets the meta data block header.
        /// </summary>
        /// <param name="data"></param>
        protected void ParseData(byte[] data) {
            // Parses the 4 byte header data:
            // Bit 1:   Last-metadata-block flag: '1' if this block is the last metadata block before the audio blocks, '0' otherwise.
            // Bit 2-8: Block Type, 
            //  0 : STREAMINFO 
            //  1 : PADDING 
            //  2 : APPLICATION 
            //  3 : SEEKTABLE 
            //  4 : VORBIS_COMMENT 
            //  5 : CUESHEET 
            //  6 : PICTURE 
            //  7-126 : reserved 
            //  127 : invalid, to avoid confusion with a frame sync code
            // Next 3 bytes: Length (in bytes) of metadata to follow (does not include the size of the METADATA_BLOCK_HEADER)

            isLastMetaDataBlock = BinaryDataHelper.GetBoolean(data, 0, 0);

            typeID = data[0] & 0x7F;
            switch (typeID) {
                case 0:
                    type = MetadataBlockType.StreamInfo;
                    metaDataBlockLength = 34;
                    break;
                case 1:
                    type = MetadataBlockType.Padding;
                    break;
                case 2:
                    type = MetadataBlockType.Application;
                    break;
                case 3:
                    type = MetadataBlockType.Seektable;
                    break;
                case 4:
                    type = MetadataBlockType.VorbisComment;
                    break;
                case 5:
                    type = MetadataBlockType.CueSheet;
                    break;
                case 6:
                    type = MetadataBlockType.Picture;
                    break;
            }
            if (typeID > 6 && typeID < 127) {
                type = MetadataBlockType.None;
            } else if(typeID >= 127) {
                type = MetadataBlockType.Invalid;
            }

            metaDataBlockLength = (BinaryDataHelper.GetUInt24(data, 1));
        }
    }
}
