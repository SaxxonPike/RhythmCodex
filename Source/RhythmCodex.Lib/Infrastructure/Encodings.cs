using System.Text;

namespace RhythmCodex.Infrastructure
{
    public static class Encodings
    {
        public static readonly Encoding CP437 = CodePagesEncodingProvider.Instance.GetEncoding(437);
        public static readonly Encoding CP932 = CodePagesEncodingProvider.Instance.GetEncoding(932);
    }
}