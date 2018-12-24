using System;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// This block stores various information for use in a cue sheet.
    /// </summary>
    public class CueSheet : MetadataBlock {

        // See spec for details
        public static readonly byte CUESHEET_LEADOUT_TRACK_NUMBER_CDDA = 170;
        public static readonly byte CUESHEET_LEADOUT_TRACK_NUMBER_NON_CDDA = 255;
        private const uint CUESHEET_BLOCK_DATA_LENGTH = 396;
        private const uint CUESHEET_TRACK_LENGTH = 36;
        private const uint CUESHEET_TRACK_INDEXPOINT_LENGTH = 12;
        private const int MEDIACATALOG_MAX_LENGTH = 128;
        private const int RESERVED_NULLDATA_LENGTH = 258;

        public CueSheet()
        {
            Header.Type = MetadataBlockHeader.MetadataBlockType.CueSheet;

            CalculateMetaDataBlockLength();
        }

        /// <summary>
        /// Parses the binary metadata from the flac file into a CueSheet object.
        /// </summary>
        /// <param name="data">The binary data from the flac file.</param>
        public override void LoadBlockData(byte[] data) {
            mediaCatalog = Encoding.ASCII.GetString(data, 0, 128).Trim(new char[]{ '\0' });
            leadInSampleCount = BinaryDataHelper.GetUInt64(data, 128);
            isCDCueSheet = BinaryDataHelper.GetBoolean(data, 136, 0);
            // We're skipping 7 bits + 258 bytes which is reserved null data
            var trackCount = data[395];
            if (trackCount > 100)
            {
                // Do we really need to throw an exception here?
                throw new Exceptions.FlacLibSharpInvalidFormatException(
                    $"CueSheet has invalid track count {trackCount}. Cannot be more than 100.");
            }

            var cueSheetTrackOffset = 396;
            for (var i = 0; i < trackCount; i++)
            {
                var newTrack = new CueSheetTrack(data, cueSheetTrackOffset);
                cueSheetTrackOffset += 36 + (12 * newTrack.IndexPointCount); // 36 bytes for the cueSheetTrack and 12 bytes per index point ...
                Tracks.Add(newTrack);
            }
        }

        /// <summary>
        /// Writes the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            if (Tracks.Count > 0)
            {
                var lastTrack = Tracks[Tracks.Count - 1];
                if (!lastTrack.IsLeadOut)
                {
                    throw new Exceptions.FlacLibSharpInvalidFormatException(
                        $"CueSheet is invalid, last track (nr {lastTrack.TrackNumber}) is not the lead-out track.");
                }
            }
            else
            {
                throw new Exceptions.FlacLibSharpInvalidFormatException("CueSheet is invalid as it has no tracks, it must have at least one track (the lead-out track).");
            }

            // TODO: this value in the header should also update when someone add/removes tracks or track index points ...
            Header.MetaDataBlockLength = CalculateMetaDataBlockLength();
            Header.WriteHeaderData(targetStream);

            targetStream.Write(BinaryDataHelper.GetPaddedAsciiBytes(MediaCatalog, MEDIACATALOG_MAX_LENGTH), 0, MEDIACATALOG_MAX_LENGTH);
            targetStream.Write(BinaryDataHelper.GetBytesUInt64(LeadInSampleCount), 0, 8);
            
            byte isCDCueSheet = 0;
            if(this.isCDCueSheet) {
                isCDCueSheet = 0x80; // Most significant bit should be 1
            }
            targetStream.WriteByte(isCDCueSheet);

            // Now we need to write 258 bytes of 0 data ("reserved")
            var nullData = new byte[RESERVED_NULLDATA_LENGTH];
            targetStream.Write(nullData, 0, nullData.Length);

            // The number of tracks i 1 byte in size ...
            targetStream.WriteByte(TrackCount);

            foreach (var track in Tracks)
            {
                track.WriteBlockData(targetStream);
            }

        }

        private string mediaCatalog;

        /// <summary>
        /// Gets or sets the media catalog number.
        /// </summary>
        public string MediaCatalog {
            get {
                if (mediaCatalog == null)
                {
                    mediaCatalog = string.Empty;
                }
                return mediaCatalog;
            }
            set { mediaCatalog = value; }
        }

        private ulong leadInSampleCount;

        /// <summary>
        /// Gets or sets the number of lead-in samples, this field is only relevant for CD-DA cuesheets.
        /// </summary>
        public ulong LeadInSampleCount {
            get { return leadInSampleCount; }
            set { leadInSampleCount = value; }
        }

        private bool isCDCueSheet;

        /// <summary>
        /// Gets or sets whether the cuesheet corresponds to a Compact Disc.
        /// </summary>
        public bool IsCDCueSheet {
            get { return isCDCueSheet; }
            set { isCDCueSheet = value; }
        }

        /// <summary>
        /// The number of tracks.
        /// </summary>
        public byte TrackCount {
            get {
                return (byte)Tracks.Count;
            }
        }

        private CueSheetTrackCollection tracks;

        /// <summary>
        /// All Tracks in the cuesheet.
        /// </summary>
        public CueSheetTrackCollection Tracks {
            get {
                if (tracks == null) {
                    tracks = new CueSheetTrackCollection();
                }
                return tracks;
            }
        }

        /// <summary>
        /// Calculates the total Block Length of this metadata block, for use in the Header.
        /// </summary>
        /// <returns></returns>
        private uint CalculateMetaDataBlockLength()
        {
            var totalLength = CUESHEET_BLOCK_DATA_LENGTH; // See the specs ...
            // The length of this metadata block is: 
            // 396 bytes for the CueSheet block data itself (see spec for details)
            // + 36 bytes per CueSheetTrack
            // + 12 bytes per CueSheetTrackIndex
            foreach (var track in Tracks)
            {
                totalLength += CUESHEET_TRACK_LENGTH + (track.IndexPointCount * CUESHEET_TRACK_INDEXPOINT_LENGTH);
            }

            return totalLength;
        }

    }
}
