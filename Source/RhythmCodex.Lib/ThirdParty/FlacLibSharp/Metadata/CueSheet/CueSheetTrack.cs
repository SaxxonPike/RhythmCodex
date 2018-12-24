using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// TODO: A single track in the cuesheet.
    /// </summary>
    public class CueSheetTrack {

        private const int ISRC_LENGTH = 12;
        private const int RESERVED_NULLDATA_LENGTH = 13;

        public CueSheetTrack()
        {
            // Initializing some "reasonable" defaults
            trackOffset = 0;
            trackNumber = 0;
            isrc = string.Empty;
            isAudioTrack = true;
            isPreEmphasis = false;

            indexPoints = new CueSheetTrackIndexCollection();
        }

        /// <summary>
        /// Initialize the CueSheetTrack.
        /// </summary>
        /// <param name="data">The full data array.</param>
        /// <param name="dataOffset">Where the cuesheet track begins.</param>
        public CueSheetTrack(byte[] data, int dataOffset) {
            trackOffset = BinaryDataHelper.GetUInt64(data, dataOffset);
            trackNumber = (byte)BinaryDataHelper.GetUInt64(data, dataOffset + 8, 8);
            isrc = Encoding.ASCII.GetString(data, dataOffset + 9, 12).Trim(new char[] { '\0' });
            isAudioTrack = !BinaryDataHelper.GetBoolean(data, dataOffset + 21, 1); // 0 for audio
            isPreEmphasis = BinaryDataHelper.GetBoolean(data, dataOffset + 21, 2);
            // 6 bits + 13 bytes need to be zero, won't check this
            var indexPointCount = (byte)BinaryDataHelper.GetUInt64(data, dataOffset + 35, 8);

            if (indexPointCount > 100)
            {
                throw new Exceptions.FlacLibSharpInvalidFormatException(
                    $"CueSheet track nr {TrackNumber} has an invalid Track Index Count of {indexPointCount}. Maximum allowed is 100.");
            }

            // For all tracks, except the lead-in track, one or more track index points
            dataOffset += 36;
            for (var i = 0; i < indexPointCount; i++)
            {
                IndexPoints.Add(new CueSheetTrackIndex(data, dataOffset));
                dataOffset += 12; // Index points are always 12 bytes long
            }

            if (indexPointCount != IndexPoints.Count)
            {
                // Should we be so strict?
                throw new Exceptions.FlacLibSharpInvalidFormatException(
                    $"CueSheet track nr {TrackNumber} indicates {indexPointCount} index points, but actually {IndexPoints.Count} index points are present.");
            }

        }

        /// <summary>
        /// Writes the data representing this CueSheet track to the given stream.
        /// </summary>
        /// <param name="targetStream"></param>
        public void WriteBlockData(Stream targetStream)
        {
            targetStream.Write(BinaryDataHelper.GetBytesUInt64(trackOffset), 0, 8);
            targetStream.WriteByte(TrackNumber);
            targetStream.Write(BinaryDataHelper.GetPaddedAsciiBytes(isrc, ISRC_LENGTH), 0, ISRC_LENGTH);
            
            byte trackAndEmphasis = 0;
            if (IsAudioTrack)
            {
                trackAndEmphasis += 0x80; // Most significant bit to 1
            }
            if (IsPreEmphasis)
            {
                trackAndEmphasis += 0x40; // Second most significant bit to 1
            }
            targetStream.WriteByte(trackAndEmphasis);

            var nullData = new byte[RESERVED_NULLDATA_LENGTH];
            targetStream.Write(nullData, 0, nullData.Length);

            targetStream.WriteByte(IndexPointCount);

            foreach (var indexPoint in IndexPoints)
            {
                indexPoint.WriteBlockData(targetStream);
            }
        }

        private ulong trackOffset;

        /// <summary>
        /// Offset of a track.
        /// </summary>
        public ulong TrackOffset {
            get { return trackOffset; }
            set { trackOffset = value;  }
        }

        private byte trackNumber;

        /// <summary>
        /// Number of the track.
        /// </summary>
        public byte TrackNumber {
            get { return trackNumber; }
            set { trackNumber = value;  }
        }

        private string isrc;

        /// <summary>
        /// The ISRC of the track.
        /// </summary>
        public string ISRC {
            get { return isrc; }
            set { isrc = value; }
        }

        private bool isAudioTrack;

        /// <summary>
        /// Indicates whether or not this is an audio track.
        /// </summary>
        public bool IsAudioTrack {
            get { return isAudioTrack; }
            set { isAudioTrack = value; }
        }

        private bool isPreEmphasis;

        /// <summary>
        /// The Pre Emphasis flag.
        /// </summary>
        public bool IsPreEmphasis {
            get { return isPreEmphasis; }
            set { isPreEmphasis = value; }
        }

        /// <summary>
        /// Checks whether or not the track is a lead-out track (meaning track number is either 170 or 255, depending on CD-DA or not)
        /// </summary>
        public bool IsLeadOut
        {
            get
            {
                if (TrackNumber == CueSheet.CUESHEET_LEADOUT_TRACK_NUMBER_CDDA ||
                    trackNumber == CueSheet.CUESHEET_LEADOUT_TRACK_NUMBER_NON_CDDA)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Number of track index points. There must be at least one index in every track in a CUESHEET except for the lead-out track, which must have zero. For CD-DA, this number may be no more than 100.
        /// </summary>
        public byte IndexPointCount {
            get { return (byte)IndexPoints.Count; }
        }

        private CueSheetTrackIndexCollection indexPoints;

        /// <summary>
        /// All of the index points in the cue sheet track.
        /// </summary>
        public CueSheetTrackIndexCollection IndexPoints {
            get {
                if (indexPoints == null) { 
                    indexPoints = new CueSheetTrackIndexCollection();
                }
                return indexPoints;
            }
            set { indexPoints = value; }
        }

    }
}
