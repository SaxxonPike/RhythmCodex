using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// An application specific block of data.
    /// </summary>
    public class ApplicationInfo : MetadataBlock {

        private uint applicationID;
        private byte[] applicationData;

        /// <summary>
        /// Creates an empty ApplicationInfo block.
        /// </summary>
        /// <remarks>Application id will be 0, application data will be empty.</remarks>
        public ApplicationInfo()
        {
            Header.Type = MetadataBlockHeader.MetadataBlockType.Application;
            applicationID = 0;
            applicationData = new byte[0];
        }

        /// <summary>
        /// Parses the given binary metadata to an ApplicationInfo block
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data) {
            applicationID = BinaryDataHelper.GetUInt32(data, 0);
            applicationData = new byte[data.Length - 4];

            for (var i = 0; i < applicationData.Length; i++)
            {
                applicationData[i] = data[i + 4]; // + 4 because the first four bytes are the application ID!
            }
        }

        /// <summary>
        /// Writes the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            Header.MetaDataBlockLength = 4 + (uint)applicationData.Length;
            Header.WriteHeaderData(targetStream);

            targetStream.Write(BinaryDataHelper.GetBytesUInt32(applicationID), 0, 4);
            targetStream.Write(applicationData, 0, applicationData.Length);
        }

        /// <summary>
        /// The application ID of the application for which the data is intended
        /// </summary>
        public uint ApplicationID {
            get { return applicationID; }
            set { applicationID = value; }
        }

        /// <summary>
        /// The additional data
        /// </summary>
        public byte[] ApplicationData {
            get { return applicationData; }
            set { applicationData = value; }
        }

    }
}
