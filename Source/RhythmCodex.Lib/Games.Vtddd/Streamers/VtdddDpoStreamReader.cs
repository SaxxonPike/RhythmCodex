using System.IO;
using RhythmCodex.Games.Vtddd.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Vtddd.Streamers;

[Service]
public class VtdddDpoStreamReader : IVtdddDpoStreamReader
{
    public VtdddDpoFile? Read(Stream stream, int length)
    {
        if (length < 4)
            return null;

        var reader = new BinaryReader(stream);
        var data = reader.ReadBytes(length);
            
        // derive key from known plaintext
        var key = new[]
        {
            (byte)(data[0] ^ 'O'),
            (byte)(data[1] ^ 'g'),
            (byte)(data[2] ^ 'g'),
            (byte)(data[3] ^ 'S')
        };
            
        // decrypt
        for (int i = 0, k = 0; i < data.Length; i++, k = (k + 1) & 3)
            data[i] ^= key[k];

        return new VtdddDpoFile
        {
            Key = key,
            Data = data
        };
    }
}