using JetBrains.Annotations;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex;

[PublicAPI]
public static class TestHelper
{
    public static void CreateDirectory(this IResolver resolver, string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
        
    public static void WriteSound(this IResolver resolver, Sound? decoded, string outFileName)
    {
        var encoder = resolver.Resolve<IRiffPcm16SoundEncoder>();
        var writer = resolver.Resolve<IRiffStreamWriter>();
        var dsp = resolver.Resolve<IAudioDsp>();

        var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);

        CreateDirectory(resolver, Path.GetDirectoryName(outPath)!);

        var encoded = encoder.Encode(dsp.ApplyEffects(decoded));
        using var outStream = new MemoryStream();
        writer.Write(outStream, encoded);
        outStream.Flush();
        File.WriteAllBytes(outPath, outStream.ToArray());
    }

    public static void WriteFile(this IResolver resolver, byte[] data, string outFileName)
    {
        WriteFile(resolver, data.AsSpan(), outFileName);
    }

    public static void WriteFile(this IResolver resolver, ReadOnlyMemory<byte> data, string outFileName)
    {
        WriteFile(resolver, data.Span, outFileName);
    }

    public static void WriteFile(this IResolver resolver, ReadOnlySpan<byte> data, string outFileName)
    {
        var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);
        CreateDirectory(resolver, Path.GetDirectoryName(outPath)!);
        using var stream = File.OpenWrite(outPath);
        stream.Write(data);
        stream.Flush();
    }

    public static Stream OpenWrite(this IResolver resolver, string outFileName)
    {
        var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);
        CreateDirectory(resolver, Path.GetDirectoryName(outPath)!);
        return File.Open(outPath, FileMode.Create, FileAccess.ReadWrite);
    }
}