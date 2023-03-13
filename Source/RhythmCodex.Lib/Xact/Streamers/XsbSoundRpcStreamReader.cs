using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

[Service]
public class XsbSoundRpcStreamReader : IXsbSoundRpcStreamReader
{
    public XsbSoundRpc Read(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var length = reader.ReadInt16();
        var count = reader.ReadByte();

        var result = new XsbSoundRpc
        {
            Curves = new int[count]
        };

        for (var i = 0; i < count; i++)
            result.Curves[i] = reader.ReadInt32();

        result.ExtraData = reader.ReadBytes(length - 3 - 4 * count);

        return result;
    }
}