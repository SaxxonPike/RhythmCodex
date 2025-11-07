using System;
using System.IO;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Arc.Processors;
// source: arcunpack (gergc)

[Service]
public class ArcLzEncoder : IArcLzEncoder
{
    public Memory<byte> Encode(ReadOnlySpan<byte> source)
    {
        var context = new LzCompress();
        context.Write(source);
        context.Close();
        return context.OutputData.ToArray();
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
        private readonly byte[] _matches = new byte[MaxMatch];
        private readonly byte[] _packet = new byte[0x20];
        private int _ringPos;
        private int _cursorCount;
        private int _matchCount;
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

        public MemoryStream OutputData => _output;

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

            _output.WriteByte((byte)(_flags & byte.MaxValue));
            _output.Write(_packet, 0, _pktPos);
            _output.Flush();
        }

        private void Write(ReadOnlySpan<byte> b, int off, int len)
        {
            for (var index = 0; index < len; ++index)
                Write(b[off + index] & byte.MaxValue);
        }

        public void Write(ReadOnlySpan<byte> b) =>
            Write(b, 0, b.Length);

        private void Write(int b)
        {
            if (b is < 0 or > byte.MaxValue)
                return;

            switch (_matchCount)
            {
                case 0:
                    InitCursors(b);
                    break;
                case MaxMatch:
                    EmitMatch();
                    InitCursors(b);
                    break;
                default:
                    UpdateCursors(b);
                    break;
            }

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
            var currentRingPos = (short)_ringPos;
            if (tail == Null)
                _heads[newValue] = currentRingPos;
            else
                _links[tail] = currentRingPos;
            _tails[newValue] = currentRingPos;
            _links[currentRingPos] = Null;
            _ring[_ringPos] = (byte)newValue;
            _ringPos = _ringPos + 1 & 0xFFF;
        }

        private void InitCursors(int b)
        {
            _cursorCount = 0;
            for (var index = _heads[b]; index != Null; index = _links[index])
            {
                if (index != (_ringPos & 0xFFF))
                    _cursors[_cursorCount++] = index;
            }

            if (_cursorCount > 0)
                _matches[_matchCount++] = (byte)b;
            else
                PushVerbatim((byte)b);
        }

        private void UpdateCursors(int b)
        {
            var index = 0;
            while (index < _cursorCount)
            {
                if ((_ring[_cursors[index] + _matchCount & 0xFFF] & byte.MaxValue) != b)
                {
                    if (_cursorCount > 1)
                    {
                        _cursors[index] = _cursors[--_cursorCount];
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

            _matches[_matchCount++] = (byte)b;
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
            if (_matchCount < MinMatch)
            {
                for (var index = 0; index < _matchCount; ++index)
                    PushVerbatim(_matches[index]);
            }
            else
            {
                _flags >>= 1;
                var num = (_ringPos - _matchCount & 0xFFF) - _cursors[0] & 0xFFF;
                _packet[_pktPos++] = (byte)(num >> 4);
                _packet[_pktPos++] = (byte)((num & 15) << 4 | _matchCount - MinMatch);
                if ((uint)(_flags & 0x100) > 0)
                    WritePacket();
            }

            _matchCount = 0;
        }

        private void WritePacket()
        {
            _output.WriteByte((byte)(_flags & byte.MaxValue));
            _output.Write(_packet, 0, _pktPos);
            _pktPos = 0;
            _flags = 0x10000;
        }
    }
}