using System;
using System.IO;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression
{
    // source: arcunpack (gergc)
    
    [Service]
    public class ArcLzEncoder : IArcLzEncoder
    {
        public byte[] Encode(byte[] source)
        {
            var context = new LzCompress();
            context.Write(source);
            context.Close();
            return context.Outputdata().ToArray();
        }

        private sealed class LzCompress
        {
            private const int MinMatch = 3;
            private const int MaxMatch = MinMatch + 15;
            private const short Null = -1;
            private readonly byte[] _ring = new byte[0x1000];
            private readonly short[] _links = new short[0x1000];
            private readonly short[] _heads = new short[0x100];
            private readonly short[] _tails = new short[0x100];
            private readonly short[] _cursors = new short[0x1000];
            private readonly byte[] _match = new byte[MaxMatch];
            private readonly byte[] _packet = new byte[0x20];
            private int _ringPos;
            private int _ncursors;
            private int _nmatched;
            private int _pktPos;
            private int _flags = 0x10000;
            private readonly MemoryStream _output;

            public LzCompress()
            {
                _output = new MemoryStream();
                for (var index = 0; index < 0x100; ++index)
                {
                    _heads[index] = Null;
                    _tails[index] = Null;
                }
            }

            public MemoryStream Outputdata()
            {
                return _output;
            }

            public void Close()
            {
                EmitMatch();
                _flags >>= 1;
                _packet[_pktPos++] = 0;
                _packet[_pktPos++] = 0;
                for (; (_flags & 0x100) == 0; _flags >>= 1)
                {
                    if (_flags == 0)
                        Console.WriteLine("Error!");
                }

                _output.WriteByte((byte) (_flags & byte.MaxValue));
                _output.Write(_packet, 0, _pktPos);
                _output.Flush();
            }

            private void Write(byte[] b, int off, int len)
            {
                for (var index = 0; index < len; ++index)
                    Write(b[off + index] & byte.MaxValue);
            }

            public void Write(byte[] b)
            {
                Write(b, 0, b.Length);
            }

            private void Write(int b)
            {
                if (b < 0 || b > byte.MaxValue)
                    return;
                if (_nmatched == 0)
                    InitCursors(b);
                else if (_nmatched == MaxMatch)
                {
                    EmitMatch();
                    InitCursors(b);
                }
                else
                    UpdateCursors(b);

                Advance(b);
            }

            private void Advance(int newValue)
            {
                var index = _ring[_ringPos] & byte.MaxValue;
                var head = _heads[index];
                if (head != Null)
                {
                    var link = _links[head];
                    if (link == Null)
                        _tails[index] = Null;
                    _heads[index] = link;
                }

                var tail = _tails[newValue];
                var currentRingPos = (short) _ringPos;
                if (tail == Null)
                    _heads[newValue] = currentRingPos;
                else
                    _links[tail] = currentRingPos;
                _tails[newValue] = currentRingPos;
                _links[currentRingPos] = Null;
                _ring[_ringPos] = (byte) newValue;
                _ringPos = _ringPos + 1 & 0xFFF;
            }

            private void InitCursors(int b)
            {
                _ncursors = 0;
                for (var index = _heads[b]; (int) index != (int) Null; index = _links[index])
                {
                    if (index != (_ringPos & 0xFFF))
                        _cursors[_ncursors++] = index;
                }

                if (_ncursors > 0)
                    _match[_nmatched++] = (byte) b;
                else
                    PushVerbatim((byte) b);
            }

            private void UpdateCursors(int b)
            {
                var index = 0;
                while (index < _ncursors)
                {
                    if ((_ring[_cursors[index] + _nmatched & 0xFFF] & byte.MaxValue) != b)
                    {
                        if (_ncursors > 1)
                        {
                            _cursors[index] = _cursors[--_ncursors];
                        }
                        else
                        {
                            EmitMatch();
                            InitCursors(b);
                            return;
                        }
                    }
                    else
                        ++index;
                }

                _match[_nmatched++] = (byte) b;
            }

            private void PushVerbatim(byte b)
            {
                _packet[_pktPos++] = b;
                _flags = (_flags >> 1) | 128;
                if ((_flags & 0x100) == 0)
                    return;
                WritePacket();
            }

            private void EmitMatch()
            {
                if (_nmatched < MinMatch)
                {
                    for (var index = 0; index < _nmatched; ++index)
                        PushVerbatim(_match[index]);
                }
                else
                {
                    _flags >>= 1;
                    var num = (_ringPos - _nmatched & 0xFFF) - _cursors[0] & 0xFFF;
                    _packet[_pktPos++] = (byte) (num >> 4);
                    _packet[_pktPos++] = (byte) ((num & 15) << 4 | _nmatched - MinMatch);
                    if ((uint) (_flags & 0x100) > 0)
                        WritePacket();
                }

                _nmatched = 0;
            }

            private void WritePacket()
            {
                _output.WriteByte((byte) (_flags & byte.MaxValue));
                _output.Write(_packet, 0, _pktPos);
                _pktPos = 0;
                _flags = 0x10000;
            }
        }
    }
}