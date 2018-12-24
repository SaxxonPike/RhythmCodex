using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlacLibSharp.Exceptions;

namespace FlacLibSharp
{
    /// <summary>
    /// Parses FLAC data from the given Stream or file.
    /// </summary>
    /// <remarks>Only metadata parsing is supported, audio is not decoded.</remarks>
    public class FlacFile : IDisposable
    {
        private Stream dataStream;
        private List<MetadataBlock> metadata;
        
        // Some housekeeping for the file save
        private long frameStart;
        private readonly string filePath = string.Empty;

        private static readonly byte[] magicFlacMarker = { 0x66, 0x4C, 0x61, 0x43 }; // fLaC

        #region Constructors

        /// <summary>
        /// Opens a flac file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public FlacFile(string path)
        {
            filePath = path;
            dataStream = File.OpenRead(path);
            Initialize();
        }

        /// <summary>
        /// Opens a flac file from a stream of data.
        /// </summary>
        /// <param name="data">Any stream of data</param>
        /// <remarks>Stream is assumed to be at the beginning of the FLAC data.</remarks>
        public FlacFile(Stream data)
        {
            dataStream = data;
            Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        /// List of all the available metadata.
        /// </summary>
        public List<MetadataBlock> Metadata
        {
            get
            {
                if (metadata == null)
                {
                    metadata = new List<MetadataBlock>();
                }
                return metadata;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Verifies the flac identity and loads the available metadata blocks.
        /// </summary>
        protected void Initialize()
        {
            VerifyFlacIdentity();
            ReadMetadata();
        }

        /// <summary>
        /// Verifies whether or not the first four bytes of the file indicate this is a flac file.
        /// </summary>
        private void VerifyFlacIdentity()
        {
            var data = new byte[4];

            try
            {
                dataStream.Read(data, 0, 4);
                for (var i = 0; i < data.Length; i++)
                {
                    if (data[i] != magicFlacMarker[i])
                    {
                        throw new FlacLibSharpInvalidFormatException("In Verify Flac Identity");
                    }
                }
            }
            catch (ArgumentException)
            {
                throw new FlacLibSharpInvalidFormatException("In Verify Flac Identity");
            }
        }

        #endregion

        #region Reading

        /// <summary>
        /// Parses all the metadata blocks available in the file.
        /// </summary>
        protected void ReadMetadata()
        {
            var foundStreamInfo = false;
            MetadataBlock lastMetaDataBlock = null;
            do
            {
                lastMetaDataBlock = MetadataBlock.Create(dataStream);
                Metadata.Add(lastMetaDataBlock);
                switch(lastMetaDataBlock.Header.Type) {
                    case MetadataBlockHeader.MetadataBlockType.StreamInfo:
                        foundStreamInfo = true;
                        streamInfo = (StreamInfo)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.Application:
                        applicationInfo = (ApplicationInfo)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.CueSheet:
                        cueSheet = (CueSheet)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.Seektable:
                        seekTable = (SeekTable)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.VorbisComment:
                        vorbisComment = (VorbisComment)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.Padding:
                        padding = (Padding)lastMetaDataBlock;
                        break;
                }
            } while (!lastMetaDataBlock.Header.IsLastMetaDataBlock);

            if (!foundStreamInfo)
                throw new FlacLibSharpStreamInfoMissing();
            
            // Remember where the frame data starts
            frameStart = dataStream.Position;
        }

#endregion

        #region Quick metadata access

        /* Direct access to different meta data */

        private StreamInfo streamInfo;
        private ApplicationInfo applicationInfo;
        private VorbisComment vorbisComment;
        private CueSheet cueSheet;
        private SeekTable seekTable;
        private Padding padding;

        /// <summary>
        /// Returns the StreamInfo metedata of the loaded Flac file.
        /// </summary>
        public StreamInfo StreamInfo { get { return streamInfo; } }
        
        /// <summary>
        /// Returns the first ApplicationInfo metadata of the loaded Flac file or null if this data is not available.
        /// </summary>
        public ApplicationInfo ApplicationInfo { get { return applicationInfo; } }

        /// <summary>
        /// Returns all ApplicationInfo metadata, if there is any.
        /// </summary>
        public IEnumerable<ApplicationInfo> GetAllApplicationInfo()
        {
            var result = new List<ApplicationInfo>();
            foreach (var block in Metadata)
            {
                if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Application)
                {
                    result.Add((ApplicationInfo)block);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the VorbisComment metadata of the loaded Flac file or null if this data is not available.
        /// </summary>
        public VorbisComment VorbisComment { get { return vorbisComment; } }

        /// <summary>
        /// Returns the CueSheet metadata of the loaded Flac file or null if this data is not available.
        /// </summary>
        public CueSheet CueSheet { get { return cueSheet; } }

        /// <summary>
        /// Returns all CueSheet metadata, if there is any.
        /// </summary>
        public IEnumerable<CueSheet> GetAllCueSheets() {
            var result = new List<CueSheet>();
            foreach (var block in Metadata)
            {
                if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.CueSheet)
                {
                    result.Add((CueSheet)block);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the SeekTable metadata of the loaded Flac file or null if this data is not available.
        /// </summary>
        public SeekTable SeekTable { get { return seekTable; } }

        /// <summary>
        /// Returns the Padding metadata of the loaded Flac file or null if this data is not available.
        /// </summary>
        public Padding Padding { get { return padding; } }

        /// <summary>
        /// Returns all Padding metadata, if there is any.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Padding> GetAllPadding()
        {
            var result = new List<Padding>();
            foreach (var block in Metadata)
            {
                if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Padding)
                {
                    result.Add((Padding)block);
                }
            }
            return result;
        }

        /// <summary>
        /// Will return all Picture metadata, if there is any.
        /// </summary>
        /// <returns></returns>
        public List<Picture> GetAllPictures()
        {
            var result = new List<Picture>();

            foreach (var block in Metadata)
            {
                if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Picture)
                {
                    result.Add((Picture)block);
                }
            }

            return result;
        }

        #endregion

        #region Writing

        /// <summary>
        /// Writes the metadata back to the flacfile, overwriting the original file.
        /// </summary>
        public void Save()
        {
            if (string.IsNullOrEmpty(filePath) || !dataStream.CanSeek)
            {
                throw new FlacLibSharpSaveNotSupportedException();
            }

            var bufferFile = Path.GetTempFileName();
            using (var fs = new FileStream(bufferFile, FileMode.Create))
            {
                // First write the magic flac bytes ...
                fs.Write(magicFlacMarker, 0, magicFlacMarker.Length);

                for (var i = 0; i < Metadata.Count; i++) {
                    var block = Metadata[i];

                    // We have to make sure to set the last metadata bit correctly.
                    if (i == Metadata.Count - 1)
                    {
                        block.Header.IsLastMetaDataBlock = true;
                    }
                    else
                    {
                        block.Header.IsLastMetaDataBlock = false;
                    }

                    try
                    {
                        var startLength = fs.Length;
                        block.WriteBlockData(fs);
                        var writtenBytes = fs.Length - startLength;

                        // minus 4 bytes because the MetaDataBlockLength excludes the size of the header
                        if (writtenBytes - 4 != block.Header.MetaDataBlockLength)
                        {
                            throw new Exception(
                                $"The header of metadata block of type {block.Header.Type} claims a length of {block.Header.MetaDataBlockLength} bytes but the total amount of data written was {writtenBytes} + 4 bytes");
                        }
                    }
                    catch (NotImplementedException)
                    {
                        // Ignore for now (testing) - we'll remove this handler later!
                    }
                }

                // Metadata is written back to the new file stream, now we can copy the rest of the frames
                var dataBuffer = new byte[4096];
                dataStream.Seek(frameStart, SeekOrigin.Begin);

                var read = 0;
                do {
                    read = dataStream.Read(dataBuffer, 0, dataBuffer.Length);
                    fs.Write(dataBuffer, 0, read);
                } while (read > 0);
            }

            dataStream.Dispose();

            // Issue #35: Cannot use "File.Move" because it does not retain the original file's attributes.
            //            File.Copy does: https://docs.microsoft.com/en-us/dotnet/api/system.io.file.copy?view=netframework-4.7#System_IO_File_Copy_System_String_System_String_System_Boolean_
            File.Copy(bufferFile, filePath, true);
            File.Delete(bufferFile);
        }

        #endregion

        /// <summary>
        /// Closes the Flac file.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up managed resources
                if (dataStream != null)
                {
                    dataStream.Dispose();
                    dataStream = null;
                }
            }
        }

     }
}
