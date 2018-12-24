using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A seek point in a frame.
    /// </summary>
    public class SeekPoint : IComparable<SeekPoint> {

        private ulong firstSampleNumber;
        private ulong byteOffset;
        private ushort numberOfSamples;
        private bool isPlaceHolder;

        /// <summary>
        /// Creates a new seekpoint.
        /// </summary>
        /// <param name="data"></param>
        public SeekPoint(byte[] data) {
            firstSampleNumber = BinaryDataHelper.GetUInt64(data, 0);
            byteOffset = BinaryDataHelper.GetUInt64(data, 8);
            numberOfSamples = BinaryDataHelper.GetUInt16(data, 16);
            ValidateIsPlaceholder();
        }

        /// <summary>
        /// Writes the data representing this SeekPoint to the given stream.
        /// </summary>
        /// <param name="targetStream"></param>
        public void WriteData(Stream targetStream) {
            targetStream.Write(BinaryDataHelper.GetBytesUInt64(firstSampleNumber), 0, 8);
            targetStream.Write(BinaryDataHelper.GetBytesUInt64(byteOffset), 0, 8);
            targetStream.Write(BinaryDataHelper.GetBytesUInt16(numberOfSamples), 0, 2);
        }

        /// <summary>
        /// Creates a place holder seekpoint.
        /// </summary>
        public SeekPoint() {
            firstSampleNumber = long.MaxValue;
            isPlaceHolder = true;
        }

        /// <summary>
        /// Sample number of the first sample in a target frame, 0xFFFFFFFFFFFFFFFF for a placeholder point.
        /// </summary>
        public ulong FirstSampleNumber {
            get { return firstSampleNumber; }
            set {
                firstSampleNumber = value;
                ValidateIsPlaceholder();
            }
        }

        /// <summary>
        /// Offset, in bytes, from the first byte of the first frame header to the first byte of the target frame's header.
        /// </summary>
        public ulong ByteOffset {
            get { return byteOffset; }
            set { byteOffset = value; }
        }

        /// <summary>
        /// Number of samples in the target frame.
        /// </summary>
        public ushort NumberOfSamples {
            get { return numberOfSamples; }
            set { numberOfSamples = value; }
        }

        /// <summary>
        /// Indicates if this seekpoint is a place holder.
        /// </summary>
        public bool IsPlaceHolder {
            get { return isPlaceHolder; }
            set { isPlaceHolder = value; }
        }

        /// <summary>
        /// Checks if this SeekPoint is a place holder.
        /// </summary>
        private void ValidateIsPlaceholder() {
            if (FirstSampleNumber == ulong.MaxValue) {
                isPlaceHolder = true;
            } else {
                isPlaceHolder = false;
            }
        }

        #region IComparable<FLACSeekPoint> Members
        
        /// <summary>
        /// Compares two seekpoints based on the "first sample number".
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SeekPoint other) {
            if (firstSampleNumber == other.firstSampleNumber) {
                // We're equal... in the reality of FLAC this may never happen
                return 0;
            } else if (firstSampleNumber < other.firstSampleNumber) {
                // I precede the other, because my samplenumber is smaller
                return -1;
            } else {
                // I follow the other, because my samplenumber is smaller
                return 1;
            }
        }

        #endregion
    }
}
