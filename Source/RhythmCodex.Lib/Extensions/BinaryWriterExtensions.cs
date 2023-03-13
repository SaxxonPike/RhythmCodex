using System.Diagnostics;
using System.IO;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
public static class BinaryWriterExtensions
{
    public static void Write24(this BinaryWriter writer, int value)
    {
        writer.Write((byte)(value & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
    }

    public static void Write24(this BinaryWriter writer, uint value)
    {
        writer.Write((byte)(value & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
    }

    public static void Write24S(this BinaryWriter writer, int value)
    {
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    public static void Write24S(this BinaryWriter writer, uint value)
    {
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    public static void WriteS(this BinaryWriter writer, short value)
    {
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    public static void WriteS(this BinaryWriter writer, int value)
    {
        writer.Write((byte)((value >> 24) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    public static void WriteS(this BinaryWriter writer, long value)
    {
        writer.Write((byte)((value >> 56) & 0xFF));
        writer.Write((byte)((value >> 48) & 0xFF));
        writer.Write((byte)((value >> 40) & 0xFF));
        writer.Write((byte)((value >> 32) & 0xFF));
        writer.Write((byte)((value >> 24) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    public static void WriteS(this BinaryWriter writer, ushort value)
    {
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    public static void WriteS(this BinaryWriter writer, uint value)
    {
        writer.Write((byte)((value >> 24) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    public static void WriteS(this BinaryWriter writer, ulong value)
    {
        writer.Write((byte)((value >> 56) & 0xFF));
        writer.Write((byte)((value >> 48) & 0xFF));
        writer.Write((byte)((value >> 40) & 0xFF));
        writer.Write((byte)((value >> 32) & 0xFF));
        writer.Write((byte)((value >> 24) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    public static void WriteS(this BinaryWriter writer, byte[] value)
    {
        for (var i = value.Length - 1; i >= 0; i--)
            writer.Write(value[i]);
    }
}