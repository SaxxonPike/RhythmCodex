using System;
using System.IO;

namespace RhythmCodex.Streamers
{
    public class ByteSwappedReadStream : Stream
    {
        private readonly Stream _baseStream;
        private int _buffer;
        private bool _secondByte;
        private readonly BinaryReader _reader;
        private bool _bufferReady;

        public ByteSwappedReadStream(Stream baseStream)
        {
            _baseStream = baseStream;
            _reader = new BinaryReader(_baseStream);
        }
        
        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin != SeekOrigin.Begin) 
                throw new NotImplementedException();
            
            _baseStream.Seek(offset & ~1L, SeekOrigin.Begin);
            _secondByte = (offset & 1) != 0;
            _bufferReady = false;
            return offset;
        }

        public override void SetLength(long value) => _baseStream.SetLength(value);

        private byte ReadOne()
        {
            if (!_bufferReady)
            {
                _buffer = _reader.ReadInt16();
                _bufferReady = true;
            }

            if (!_secondByte)
            {
                _secondByte = true;
                return unchecked((byte)(_buffer >> 8));
            }
            
            _secondByte = false;
            _bufferReady = false;
            return unchecked((byte)_buffer);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var total = 0;

            try
            {
                var o = offset;
                var c = count;

                // Get word aligned.
                if (c > 0 && _secondByte)
                {
                    buffer[o++] = ReadOne();
                    total++;
                    c--;
                }

                // Shortcut for zero byte reads.
                if (c <= 0)
                    return total;
                
                // Read pairs of bytes at once.
                var inBuffer = new byte[c & ~1];
                var i = 0;
                _baseStream.Read(inBuffer, 0, inBuffer.Length);
                while (c > 1)
                {
                    buffer[o++] = inBuffer[i + 1];
                    buffer[o++] = inBuffer[i];
                    i += 2;
                    c -= 2;
                    total += 2;
                }

                // If any data remains, read it normally.
                if (c > 0)
                {
                    buffer[o] = ReadOne();
                    total++;
                }
            }
            catch (EndOfStreamException)
            {
                // End is ignored, just return what we have
            }
            
            return total;
        }

        public override int ReadByte()
        {
            try
            {
                return ReadOne();
            }
            catch (EndOfStreamException)
            {
                return -1;
            }
        }

        public override void Write(byte[] buffer, int offset, int count) => 
            throw new NotImplementedException();

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _baseStream.Length;

        public override long Position
        {
            get => _baseStream.Position;
            set => Seek(value, SeekOrigin.Begin);
        }
    }
}
