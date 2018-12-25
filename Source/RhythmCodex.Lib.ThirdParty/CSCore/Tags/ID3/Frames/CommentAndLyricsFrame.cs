﻿using System.Text;

namespace CSCore.Tags.ID3.Frames
{
    public class CommentAndLyricsFrame : Frame
    {
        public string Language { get; private set; }

        public string Description { get; private set; }

        public string Text { get; private set; }

        public CommentAndLyricsFrame(FrameHeader header)
            : base(header)
        {
        }

        protected override void Decode(byte[] content)
        {
            var encoding = ID3Utils.GetEncoding(content, 0, 4);
            Language = ID3Utils.ReadString(content, 1, 3, ID3Utils.Iso88591);
            Description = ID3Utils.ReadString(content, 4, -1, encoding, out var read);
            Text = ID3Utils.ReadString(content, read + 4, -1, encoding);
        }
    }
}