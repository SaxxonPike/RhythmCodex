using System.Diagnostics;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
internal static class BinaryReaderExtensions
{
    public static void Skip(this BinaryReader reader, long offset) => 
        reader.BaseStream.Skip(offset);

    public static byte[] ReadBytesS(this BinaryReader reader, int count)
    {
        var input = reader.ReadBytes(count);
        var result = new byte[count];
        for (int i = 0, j = count - 1; i < count; i++)
            result[i] = input[j--];
        return result;
    }

    public static short ReadInt16S(this BinaryReader reader)
    {
        var input = reader.ReadBytes(2);
        int result = input[0];
        result <<= 8;
        result |= input[1];
        return unchecked((short) result);
    }

    public static int ReadInt24(this BinaryReader reader)
    {
        var input = reader.ReadBytes(3);
        int result = input[2];
        result <<= 8;
        result |= input[1];
        result <<= 8;
        result |= input[0];
        return result;
    }

    public static int ReadInt24S(this BinaryReader reader)
    {
        var input = reader.ReadBytes(3);
        int result = input[0];
        result <<= 8;
        result |= input[1];
        result <<= 8;
        result |= input[2];
        return result;
    }

    public static int ReadInt32S(this BinaryReader reader)
    {
        var input = reader.ReadBytes(4);
        int result = input[0];
        result <<= 8;
        result |= input[1];
        result <<= 8;
        result |= input[2];
        result <<= 8;
        result |= input[3];
        return result;
    }

    public static long ReadInt64S(this BinaryReader reader)
    {
        var input = reader.ReadBytes(8);
        long result = input[0];
        result <<= 8;
        result |= input[1];
        result <<= 8;
        result |= input[2];
        result <<= 8;
        result |= input[3];
        result <<= 8;
        result |= input[4];
        result <<= 8;
        result |= input[5];
        result <<= 8;
        result |= input[6];
        result <<= 8;
        result |= input[7];
        return result;
    }

    public static byte[] ReadMD5(this BinaryReader reader)
    {
        return reader.ReadBytes(16);
    }

    public static byte[] ReadMD5S(this BinaryReader reader)
    {
        return reader.ReadBytesS(16);
    }

    public static byte[] ReadSHA1(this BinaryReader reader)
    {
        return reader.ReadBytes(20);
    }

    public static byte[] ReadSHA1S(this BinaryReader reader)
    {
        return reader.ReadBytesS(20);
    }

    public static long ReadValue(this BinaryReader reader, int bytes)
    {
        var buffer = reader.ReadBytes(bytes);
        long result = 0;

        while (bytes > 0)
        {
            bytes--;
            result <<= 8;
            result |= buffer[bytes];
        }

        return result;
    }

    public static long ReadValueS(this BinaryReader reader, int bytes)
    {
        var buffer = reader.ReadBytesS(bytes);
        long result = 0;

        while (bytes > 0)
        {
            bytes--;
            result <<= 8;
            result |= buffer[bytes];
        }

        return result;
    }

    public static ushort ReadUInt16S(this BinaryReader reader)
    {
        var input = reader.ReadBytes(2);
        ushort result = input[0];
        result <<= 8;
        result |= input[1];

        return result;
    }

    public static uint ReadUInt24(this BinaryReader reader)
    {
        var input = reader.ReadBytes(3);
        uint result = input[2];
        result <<= 8;
        result |= input[1];
        result <<= 8;
        result |= input[0];

        return result;
    }

    public static uint ReadUInt24S(this BinaryReader reader)
    {
        var input = reader.ReadBytes(3);
        uint result = input[0];
        result <<= 8;
        result |= input[1];
        result <<= 8;
        result |= input[2];

        return result;
    }

    public static uint ReadUInt32S(this BinaryReader reader)
    {
        var input = reader.ReadBytes(4);
        uint result = input[0];
        result <<= 8;
        result |= input[1];
        result <<= 8;
        result |= input[2];
        result <<= 8;
        result |= input[3];

        return result;
    }

    public static ulong ReadUInt64S(this BinaryReader reader)
    {
        var input = reader.ReadBytes(8);
        ulong result = input[0];
        result <<= 8;
        result |= input[1];
        result <<= 8;
        result |= input[2];
        result <<= 8;
        result |= input[3];
        result <<= 8;
        result |= input[4];
        result <<= 8;
        result |= input[5];
        result <<= 8;
        result |= input[6];
        result <<= 8;
        result |= input[7];

        return result;
    }

    public static ulong ReadUValue(this BinaryReader reader, int bytes)
    {
        var buffer = reader.ReadBytes(bytes);
        ulong result = 0;

        while (bytes > 0)
        {
            bytes--;
            result <<= 8;
            result |= buffer[bytes];
        }

        return result;
    }

    public static ulong ReadUValueS(this BinaryReader reader, int bytes)
    {
        var buffer = reader.ReadBytesS(bytes);
        ulong result = 0;

        while (bytes > 0)
        {
            bytes--;
            result <<= 8;
            result |= buffer[bytes];
        }

        return result;
    }
}