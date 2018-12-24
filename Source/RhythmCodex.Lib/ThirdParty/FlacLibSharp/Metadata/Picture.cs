using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Exceptions;
using FlacLibSharp.Helpers;

namespace FlacLibSharp
{
    /// <summary>
    /// Picture type according to the ID3v2 APIC frame.
    /// </summary>
    public enum PictureType
    {
        /// <summary>
        /// A general picture.
        /// </summary>
        Other = 0,
        /// <summary>
        /// The picture is a file icon.
        /// </summary>
        FileIcon = 1,
        /// <summary>
        /// The picture is another file icon.
        /// </summary>
        OtherFileIcon = 2,
        /// <summary>
        /// The picture is the front cover of an album.
        /// </summary>
        CoverFront = 3,
        /// <summary>
        /// The picture is the back cover of an album.
        /// </summary>
        CoverBack = 4,
        /// <summary>
        /// The picture is the leaflet page of an album.
        /// </summary>
        LeafletPage = 5,
        /// <summary>
        /// The picture is a media page (e.g. label of CD).
        /// </summary>
        Media = 6,
        /// <summary>
        /// The picture of the lead artist.
        /// </summary>
        LeadArtist = 7,
        /// <summary>
        /// Picture of the artist.
        /// </summary>
        Artist = 8,
        /// <summary>
        /// Picture of the conductor.
        /// </summary>
        Conductor = 9,
        /// <summary>
        /// Picture of the band.
        /// </summary>
        Band = 10,
        /// <summary>
        /// picture of the composer.
        /// </summary>
        Composer = 11,
        /// <summary>
        /// Picture of the Lyricist.
        /// </summary>
        Lyricist = 12,
        /// <summary>
        /// Picture of the recording location.
        /// </summary>
        RecordingLocation = 13,
        /// <summary>
        /// Picture during the recording.
        /// </summary>
        DuringRecording = 14,
        /// <summary>
        /// Picture during the performance.
        /// </summary>
        DuringPerformance = 15,
        /// <summary>
        /// A movie screen capture picture.
        /// </summary>
        MovieScreenCapture = 16, 
        /// <summary>
        /// A picture of a bright coloured fish. Yes, really ... a fish. Brightly coloured even!
        /// </summary>
        BrightColouredFish = 17,
        /// <summary>
        /// A picture of an illustration.
        /// </summary>
        Illustration = 18,
        /// <summary>
        /// A picture of the artist logo.
        /// </summary>
        ArtistLogotype = 19,
        /// <summary>
        /// The studio logo.
        /// </summary>
        StudioLogotype = 20
    }

    /// <summary>
    /// A picture metadata block.
    /// </summary>
    public class Picture : MetadataBlock
    {
        private const uint FIXED_BLOCK_LENGTH = 8 * 4; // There are 8 32-bit fields = total block size - variable length data

        private PictureType pictureType;

        private string mimeType;

        private string description;

        private uint width, height, colorDepth, colors;

        private byte[] data;

        private string url;

        public Picture()
        {
            Header.Type = MetadataBlockHeader.MetadataBlockType.Picture;
            mimeType = string.Empty;
            description = string.Empty;
            data = new byte[] {};
            CalculateMetadataBlockLength();
        }

        /// <summary>
        /// Loads the picture data from a Metadata block.
        /// </summary>
        public override void LoadBlockData(byte[] data)
        {
            // First 32-bit: picture type according to the ID3v2 APIC frame
            pictureType = (PictureType)(int)BinaryDataHelper.GetUInt32(data, 0);

            // Then the length of the MIME type text (32-bit) and the mime type
            var mimeTypeLength = (int)BinaryDataHelper.GetUInt32(data, 4);
            var mimeData = BinaryDataHelper.GetDataSubset(data, 8, mimeTypeLength);
            mimeType = Encoding.ASCII.GetString(mimeData);

            var byteOffset = 8 + mimeTypeLength;

            // Then the description (in UTF-8)
            var descriptionLength = (int)BinaryDataHelper.GetUInt32(data, byteOffset);
            var descriptionData = BinaryDataHelper.GetDataSubset(data, byteOffset + 4, descriptionLength);
            description = Encoding.UTF8.GetString(descriptionData);

            byteOffset += 4 + descriptionLength;

            width = BinaryDataHelper.GetUInt32(data, byteOffset);
            height = BinaryDataHelper.GetUInt32(data, byteOffset + 4);
            colorDepth = BinaryDataHelper.GetUInt32(data, byteOffset + 8);
            colors = BinaryDataHelper.GetUInt32(data, byteOffset + 12);

            byteOffset += 16;

            var dataLength = (int)BinaryDataHelper.GetUInt32(data, byteOffset);
            this.data = BinaryDataHelper.GetDataSubset(data, byteOffset + 4, dataLength);

            // According to the FLAC format, if the mimeType is the string -->, the data contains
            // a URL, pointing to the image. A URL should be ASCII encoded, but using UTF-8 seems 
            // more sensible.
            if (mimeType == "-->")
            {
                url = Encoding.UTF8.GetString(this.data);
            }
        }

        /// <summary>
        /// Writes the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            // This is where the header will come
            var headerPosition = targetStream.Position;
            // Moving along, we'll write the header last!
            targetStream.Seek(4, SeekOrigin.Current);

            // 32-bit picture type
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)pictureType), 0, 4);

            var mimeTypeData = Encoding.ASCII.GetBytes(mimeType);
            // Length of the MIME type string (in bytes ...)
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)mimeTypeData.Length), 0, 4);
            // Only allows printable ascii characters (0x20 - 0x7e)
            for (var i = 0; i < mimeTypeData.Length; i++)
            {
                if (mimeTypeData[i] < 0x20 || mimeTypeData[i] > 0x7e)
                {
                    // Make sure we write the text correctly as specified by the format.
                    mimeTypeData[i] = 0x20;
                }
            }
            targetStream.Write(mimeTypeData, 0, mimeTypeData.Length);

            var descriptionData = Encoding.UTF8.GetBytes(description);
            // Length of the description string (in bytes ...)
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)descriptionData.Length), 0, 4);
            // The description of the picture (in UTF-8)
            targetStream.Write(descriptionData, 0, descriptionData.Length);

            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)width), 0, 4);
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)height), 0, 4);
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)colorDepth), 0, 4);
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)colors), 0, 4);

            // If --> is the mime type, the data contains the URL ...
            if (mimeType == "-->")
            {
                data = Encoding.ASCII.GetBytes(url);
            }

            if (data == null)
            {
                data = new byte[] {};
            }

            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)data.Length), 0, 4);
            targetStream.Write(data, 0, data.Length);

            // Writing the header, now we have the required information on the variable length fields
            CalculateMetadataBlockLength((uint)mimeTypeData.Length, (uint)descriptionData.Length, (uint)data.Length);
            
            var currentPosition = targetStream.Position;
            targetStream.Position = headerPosition;
            Header.WriteHeaderData(targetStream);
            targetStream.Position = currentPosition;
        }

        /// <summary>
        /// Calculates the total size of this block, taking into account the lengths of the variable length fields.
        /// </summary>
        private void CalculateMetadataBlockLength()
        {
            var mimeLength = (uint)Encoding.ASCII.GetByteCount(mimeType);
            var descriptionLength = (uint)Encoding.UTF8.GetByteCount(description);
            var pictureDataLength = (uint)data.Length;

            CalculateMetadataBlockLength(mimeLength, descriptionLength, pictureDataLength);
        }

        /// <summary>
        /// Calculates the total size of this block, taking into account the lengths of the variable length fields.
        /// </summary>
        /// <param name="mimeLength"></param>
        /// <param name="descriptionLength"></param>
        /// <param name="pictureDataLength"></param>
        /// <remarks>If the lengths of the variable length fields are already available, use this function, otherwise use the parameterless override.</remarks>
        private void CalculateMetadataBlockLength(uint mimeLength, uint descriptionLength, uint pictureDataLength)
        {
            Header.MetaDataBlockLength = FIXED_BLOCK_LENGTH + mimeLength + descriptionLength + pictureDataLength;
        }

        /// <summary>
        /// What kind of picture this is.
        /// </summary>
        public PictureType PictureType { 
            get { return pictureType; }
            set { pictureType = value; }
        }

        /// <summary>
        /// The MIME type of the picture file.
        /// </summary>
        public string MIMEType {
            get { return mimeType; }
            set { mimeType = value; }
        }

        /// <summary>
        /// A description for the picture.
        /// </summary>
        public string Description {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Width of the picture, in pixels.
        /// </summary>
        public uint Width {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Height of the picture, in pixels.
        /// </summary>
        public uint Height {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// The colour depth of the picture.
        /// </summary>
        public uint ColorDepth {
            get { return colorDepth; }
            set { colorDepth = value; }
        }

        /// <summary>
        /// For color indexed pictures, all of the colours in the picture.
        /// </summary>
        public uint Colors {
            get { return colors; }
            set { colors = value;  }
        }

        /// <summary>
        /// The actual picture data in a stream.
        /// </summary>
        public byte[] Data {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// The URL for the image if the MIME Type indicates a URL reference (MIME Type = '-->').
        /// </summary>
        public string URL {
            get { return url; }
            set { url = value;  }
        }

    }
}
