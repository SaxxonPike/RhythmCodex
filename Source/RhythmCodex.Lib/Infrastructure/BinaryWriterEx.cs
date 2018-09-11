using System;
using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class BinaryWriterEx : BinaryWriter
    {
        public BinaryWriterEx(Stream target)
            : base(target)
        {
        }

        public void Write24(int value)
        {
            Write((byte)(value & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)((value >> 16) & 0xFF));
        }

        public void Write24(uint value)
        {
            Write((byte)(value & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)((value >> 16) & 0xFF));
        }

        public void Write24S(int value)
        {
            Write((byte)((value >> 16) & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)(value & 0xFF));
        }

        public void Write24S(uint value)
        {
            Write((byte)((value >> 16) & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)(value & 0xFF));
        }

        public void WriteS(short value)
        {
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)(value & 0xFF));
        }

        public void WriteS(int value)
        {
            Write((byte)((value >> 24) & 0xFF));
            Write((byte)((value >> 16) & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)(value & 0xFF));
        }

        public void WriteS(long value)
        {
            Write((byte)((value >> 56) & 0xFF));
            Write((byte)((value >> 48) & 0xFF));
            Write((byte)((value >> 40) & 0xFF));
            Write((byte)((value >> 32) & 0xFF));
            Write((byte)((value >> 24) & 0xFF));
            Write((byte)((value >> 16) & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)(value & 0xFF));
        }

        public void WriteS(ushort value)
        {
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)(value & 0xFF));
        }

        public void WriteS(uint value)
        {
            Write((byte)((value >> 24) & 0xFF));
            Write((byte)((value >> 16) & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)(value & 0xFF));
        }

        public void WriteS(ulong value)
        {
            Write((byte)((value >> 56) & 0xFF));
            Write((byte)((value >> 48) & 0xFF));
            Write((byte)((value >> 40) & 0xFF));
            Write((byte)((value >> 32) & 0xFF));
            Write((byte)((value >> 24) & 0xFF));
            Write((byte)((value >> 16) & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)(value & 0xFF));
        }

        public void WriteS(byte[] value)
        {
            for (var i = value.Length - 1; i >= 0; i--)
                Write(value[i]);
        }
    }
}