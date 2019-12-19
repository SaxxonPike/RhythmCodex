using System.IO;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression
{
    // source: arcunpack (gergc)
    
    [Service]
    public class ArcLzDecoder : IArcLzDecoder
    {
        public byte[] Decode(byte[] source)
        {
            var context = new LzDecompress();
            return context.Decompress(new MemoryStream(source));
        }

        private sealed class LzDecompress
        {
            private bool _eof;
            private readonly byte[] _ring = new byte[0x1000];
            private int _pos;
            private int _copyPos;
            private int _copyLen;
            private int _flags = 1;
            private Stream _bin;

            private int InRead()
            {
                var num = _bin.ReadByte();
                return num;
            }

            private int NextFlag()
            {
                if (_flags == 1)
                {
                    var num = InRead();
                    if (num < 0)
                        return num;
                    _flags = 0x100 | num;
                }

                var num1 = _flags & 1;
                _flags >>= 1;
                return num1;
            }

            private int ReadBackRef()
            {
                var num1 = InRead();
                if (num1 < 0)
                    return num1;
                var num2 = InRead();
                _copyLen = num2 & 15;
                _copyPos = (num1 << 4) | (num2 >> 4);
                if (_copyPos > 0)
                {
                    _copyLen += 3;
                    _copyPos = (_pos - _copyPos) & 0xFFF;
                    var num3 = _ring[_copyPos] & 0xFF;
                    _copyPos = (_copyPos + 1) & 0xFFF;
                    --_copyLen;
                    return num3;
                }

                _eof = true;
                return -1;
            }

            private int Read()
            {
                if (_eof)
                    return -1;
                int num;
                if (_copyLen > 0)
                {
                    num = _ring[_copyPos] & 0xFF;
                    _copyPos = (_copyPos + 1) & 0xFFF;
                    --_copyLen;
                }
                else
                {
                    switch (NextFlag())
                    {
                        case 0:
                            num = ReadBackRef();
                            break;
                        case 1:
                            num = InRead();
                            break;
                        default:
                            num = -1;
                            break;
                    }
                }

                if (num < 0)
                    return num;
                _ring[_pos] = unchecked((byte)num);
                _pos = (_pos + 1) & 0xFFF;
                return num;
            }

            public byte[] Decompress(Stream stream)
            {
                _bin = stream;
                var output = new MemoryStream();
                while (true)
                {
                    var num = Read();
                    if (num >= 0)
                        output.WriteByte(unchecked((byte)num));
                    else
                        break;
                }

                return output.ToArray();
            }
        }
    }
}