using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlacLibSharp.Helpers;

namespace FlacLibSharp
{
    /// <summary>
    /// The value or values of a single Vorbis Comment Field.
    /// </summary>
    public class VorbisCommentValues : List<string>
    {
        /// <summary>
        /// Creates an empty vorbis comment.
        /// </summary>
        public VorbisCommentValues() { }

        /// <summary>
        /// Creates a vorbis comment with one value.
        /// </summary>
        public VorbisCommentValues(string value)
        {
            Add(value);
        }

        /// <summary>
        /// Creates a vorbis comment with the given values.
        /// </summary>
        public VorbisCommentValues(IEnumerable<string> values)
        {
            AddRange(values);
        }

        /// <summary>
        /// The first value of all values.
        /// </summary>
        public string Value {
            get
            {
                if (Count == 0) { return string.Empty; }
                return this[0];
            }
            set
            {
                if (Count == 0) { Add(value); }
                else { this[0] = value; }
            }
        }

        /// <summary>
        /// If the comment has at least one value, the first value is returned, otherwise an empty string.
        /// </summary>
        public override string ToString()
        {
            return Value;
        }
    }

    /// <summary>
    /// A metadata block containing the "Vorbis Comment" (artist, title, ...)
    /// </summary>
    public class VorbisComment : MetadataBlock, IEnumerable<KeyValuePair<string, VorbisCommentValues>>
    {
        // Vorbis format: http://www.xiph.org/vorbis/doc/v-comment.html

        private readonly Dictionary<string, VorbisCommentValues> comments;
        private string vendor;

        /// <summary>
        /// Initializes a Vorbis comment block, without any content.
        /// </summary>
        public VorbisComment()
        {
            Header.Type = MetadataBlockHeader.MetadataBlockType.VorbisComment;
            comments = new Dictionary<string, VorbisCommentValues>(StringComparer.OrdinalIgnoreCase);
            vendor = string.Empty;
        }

        /// <summary>
        /// Loads the Vorbis from a block of data.
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data)
        {
            var vendorLength = BinaryDataHelper.GetUInt32(BinaryDataHelper.SwitchEndianness(data, 0, 4), 0);
            vendor = Encoding.UTF8.GetString(BinaryDataHelper.GetDataSubset(data, 4, (int)vendorLength));

            var startOfComments = 4 + (int)vendorLength;
            var userCommentListLength = BinaryDataHelper.GetUInt32(BinaryDataHelper.SwitchEndianness(data, startOfComments, 4), 0);
            // Start of comments actually four bytes further (first piece is the count of items in the list)
            startOfComments += 4;
            for (uint i = 0; i < userCommentListLength; i++)
            {
                var commentLength = BinaryDataHelper.GetUInt32(BinaryDataHelper.SwitchEndianness(data, startOfComments, 4), 0);
                var comment = Encoding.UTF8.GetString(BinaryDataHelper.GetDataSubset(data, startOfComments + 4, (int)commentLength));
                // We're moving on in the array ...
                startOfComments += 4 + (int)commentLength;

                AddComment(comment);
            }

            // All done, note that FLAC doesn't have the "fraiming bit" for vorbis ...
        }

        /// <summary>
        /// Will write the data describing this metadata to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            uint totalLength = 0;

            var headerPosition = targetStream.Position;

            Header.WriteHeaderData(targetStream);

            // Write the vendor string (first write the length as a 32-bit uint and then the actual bytes
            var vendorData = Encoding.UTF8.GetBytes(vendor);
            var number = BinaryDataHelper.GetBytesUInt32((uint)vendorData.Length);
            targetStream.Write(BinaryDataHelper.SwitchEndianness(number, 0, 4), 0, 4);
            targetStream.Write(vendorData, 0, vendorData.Length);
            totalLength += 4 + (uint)vendorData.Length;

            // In FlacLibSharp a single comment can have multiple values, but
            // in the FLAC format each value is a comment by itself. So
            // we can't use this.comments.Count, since 1 comment could have 10
            // values, which results in 10 comments in the FLAC file.
            var totalValueCount = 0;
            foreach(var comment in comments)
            {
                foreach (var value in comment.Value)
                {
                    totalValueCount++;
                }
            }
            number = BinaryDataHelper.GetBytesUInt32((uint)totalValueCount);
            targetStream.Write(BinaryDataHelper.SwitchEndianness(number, 0, 4), 0, 4);
            totalLength += 4;

            foreach (var comment in comments)
            {
                foreach (var value in comment.Value)
                {
                    var commentText = $"{comment.Key}={value}";
                    var commentData = Encoding.UTF8.GetBytes(commentText);
                    number = BinaryDataHelper.GetBytesUInt32((uint)commentData.Length);
                    targetStream.Write(BinaryDataHelper.SwitchEndianness(number, 0, 4), 0, 4);
                    targetStream.Write(commentData, 0, commentData.Length);
                    totalLength += 4 + (uint)commentData.Length;
                }
            }

            var endPosition = targetStream.Position;

            targetStream.Seek(headerPosition, SeekOrigin.Begin);

            Header.MetaDataBlockLength = totalLength;
            Header.WriteHeaderData(targetStream);

            targetStream.Seek(endPosition, SeekOrigin.Begin);

            // Note: FLAC does NOT have the framing bit for vorbis so we don't have to write this.
        }

        /// <summary>
        /// Returns an enumerator that iterates through all VorbisComments.
        /// </summary>
        public IEnumerator<KeyValuePair<string, VorbisCommentValues>> GetEnumerator()
        {
            return comments.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all VorbisComments.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return comments.GetEnumerator();
        }

        /// <summary>
        /// Adds a comment to the list of vorbis comments.
        /// </summary>
        /// <param name="comment">Comment in the format "FIELDNAME=VALUE".</param>
        protected void AddComment(string comment)
        {
            var splitIndex = comment.IndexOf('=');
            var key = comment.Substring(0, splitIndex);
            var value = comment.Substring(splitIndex + 1);

            AddComment(key, value);
        }

        protected void AddComment(string fieldName, VorbisCommentValues values)
        {
            if (comments.ContainsKey(fieldName))
            {
                comments[fieldName].AddRange(values);
            }
            else
            {
                comments.Add(fieldName, values);
            }
        }

        /// <summary>
        /// Adds a comment to the list of vorbis comments.
        /// </summary>
        protected void AddComment(string fieldName, string value)
        {
            if (comments.ContainsKey(fieldName))
            {
                comments[fieldName].Add(value);
            } else
            {
                comments.Add(fieldName, new VorbisCommentValues(value));
            }
        }

        /// <summary>
        /// The Vendor of the Flac file.
        /// </summary>
        public string Vendor { get { return vendor; } }

        /// <summary>
        /// Get one of the vorbis comments.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field.</param>
        /// <returns>The value of the vorbis comment field.</returns>
        public VorbisCommentValues this[string key]
        {
            get
            {
                if (!comments.ContainsKey(key))
                {
                    comments.Add(key, new VorbisCommentValues());
                }

                return comments[key];
            }
            set
            {
                if (!comments.ContainsKey(key))
                {
                    comments.Add(key, value);
                } else
                {
                    comments[key] = value;
                }
            }
        }

        /// <summary>
        /// Checks whether a field with the given key is present in the Vorbis Comment data.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field.</param>
        /// <returns>True if such a field is available.</returns>
        public bool ContainsField(string key)
        {
            return comments.ContainsKey(key);
        }

        /// <summary>
        /// Removes all Vorbis Comment values for the given key.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be removed.</param>
        /// <remarks>Does nothing if no Vorbis Comments with the key are found.</remarks>
        public void Remove(string key)
        {
            if (comments.ContainsKey(key))
            {
                comments.Remove(key);
            }
        }

        /// <summary>
        /// Removes the given value from the VorbisComment.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be removed.</param>
        /// <remarks>Does nothing if no Vorbis Comments with the key are found.</remarks>
        public void Remove(string key, string value)
        {
            if (comments.ContainsKey(key))
            {
                for(var i = comments[key].Count - 1; i >= 0; i--)
                {
                    if (comments[key][i].Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        comments[key].RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new Vorbis Comment with the given key and a single value.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be added.</param>
        /// <param name="value">The value for this comment.</param>
        /// <remarks>If a tag with the key already exists, the value is appended.</remarks>
        public void Add(string key, string value)
        {
            if (ContainsField(key))
            {
                this[key].Add(value);
            } else
            {
                this[key] = new VorbisCommentValues(value);
            }
        }

        /// <summary>
        /// Adds a new Vorbis Comment with the given key and a list of values.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be removed.</param>
        /// <param name="values">The values for this comment.</param>
        /// <remarks>If a tag with the key already exists, the values are appended.</remarks>
        public void Add(string key, IEnumerable<string> values)
        {
            if (ContainsField(key))
            {
                this[key].AddRange(values);
            } else
            {
                this[key] = new VorbisCommentValues(values);
            }
        }

        /// <summary>
        /// Replaces all the values for a given tag with the given value.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be replaced.</param>
        /// <param name="values">The values for this comment.</param>
        public void Replace(string key, string value)
        {
            this[key] = new VorbisCommentValues(value);
        }

        /// <summary>
        /// Replaces all the values for a given tag with the given value.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be replaced.</param>
        /// <param name="values">The values for this comment.</param>
        public void Replace(string key, IEnumerable<string> values)
        {
            this[key] = new VorbisCommentValues(values);
        }

        /// <summary>
        /// Gets or sets the Artist if available.
        /// </summary>
        public VorbisCommentValues Artist {
            get { return this["ARTIST"]; }
            set { this["ARTIST"] = value; }
        }

        /// <summary>
        /// Gets or sets the Title if available.
        /// </summary>
        public VorbisCommentValues Title {
            get { return this["TITLE"]; }
            set { this["TITLE"] = value; }
        }

        /// <summary>
        /// Gets or sets the Album if available.
        /// </summary>
        public VorbisCommentValues Album {
            get { return this["ALBUM"]; }
            set { this["ALBUM"] = value; }
        }

        /// <summary>
        /// Gets or sets the Date if available.
        /// </summary>
        public VorbisCommentValues Date {
            get { return this["DATE"]; }
            set { this["DATE"] = value; }
        }

        /// <summary>
        /// Gets or sets the Tacknumber if available.
        /// </summary>
        public VorbisCommentValues TrackNumber {
            get { return this["TRACKNUMBER"]; }
            set { this["TRACKNUMBER"] = value; }
        }

        /// <summary>
        /// Gets or sets the Genre if available.
        /// </summary>
        public VorbisCommentValues Genre {
            get { return this["GENRE"]; }
            set { this["GENRE"] = value; }
        }

        /// <summary>
        /// Gets or sets the embedded CueSheet if available.
        /// </summary>
        public VorbisCommentValues CueSheet
        {
            get { return this["CUESHEET"]; }
            set { this["CUESHEET"] = value; }
        }
    }
}
