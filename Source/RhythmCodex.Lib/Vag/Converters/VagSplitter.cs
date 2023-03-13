using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters;

[Service]
public class VagSplitter : IVagSplitter
{
    private readonly IVagDecrypter _vagDecrypter;

    public VagSplitter(IVagDecrypter vagDecrypter)
    {
        _vagDecrypter = vagDecrypter;
    }

    public IList<ISample> Split(VagChunk chunk)
    {
        return SplitInternal(chunk).AsParallel().ToList();
    }

    private IEnumerable<ISample> SplitInternal(VagChunk chunk)
    {
        if (chunk.Channels == 1)
        {
            var length = (int) (chunk.Length ?? chunk.Data.Length);
            var totalSamples = length * 28 / 16;
            var output = new float[totalSamples];
            _vagDecrypter.Decrypt(chunk.Data, output, length, new VagState());
                
            yield return new Sample
            {
                Data = output
            };
        }
        else
        {
            var interleave = chunk.Interleave;
            var interval = interleave * chunk.Channels;
            var buffer = new float[interleave * 28 / 16];
            var length = chunk.Length ?? chunk.Data.Length;
            
            for (var channel = 0; channel < chunk.Channels; channel++)
            {
                var output = new List<float>();
                var state = new VagState();
                var offset = channel * interleave;
                while (offset < length)
                {
                    _vagDecrypter.Decrypt(chunk.Data.AsSpan(offset), buffer, interleave, state);
                    offset += interval;
                    output.AddRange(buffer);
                }

                yield return new Sample
                {
                    Data = output
                };
            }
        }
    }
}