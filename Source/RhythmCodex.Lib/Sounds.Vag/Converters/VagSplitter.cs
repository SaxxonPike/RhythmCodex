using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Converters;

[Service]
public class VagSplitter(IVagDecrypter vagDecrypter)
    : IVagSplitter
{
    public List<Sample> Split(VagChunk? chunk)
    {
        return SplitInternal(chunk).AsParallel().ToList();
    }

    private IEnumerable<Sample> SplitInternal(VagChunk? chunk)
    {
        if (chunk == null)
            yield break;
        
        if (chunk.Channels == 1)
        {
            var length = (int) (chunk.Length ?? chunk.Data.Length);
            var totalSamples = length * 28 / 16;
            var output = new float[totalSamples];
            var state = new VagState();
            vagDecrypter.Decrypt(chunk.Data.Span, output, length, state);

            yield return DecorateLoop(new Sample
            {
                Data = output,
            }, state);
        }
        else
        {
            var interleave = chunk.Interleave;
            var interval = interleave * chunk.Channels;
            var length = chunk.Length ?? chunk.Data.Length;
            var blockCount = (length + interval - 1) / interval;
            var outBlockSize = interleave * 28 / 16;
            var totalSamples = blockCount * outBlockSize;
            
            for (var channel = 0; channel < chunk.Channels; channel++)
            {
                var output = new float[totalSamples];
                var state = new VagState();
                var offset = channel * interleave;
                var totalWritten = 0;

                while (offset < length)
                {
                    totalWritten += vagDecrypter
                        .Decrypt(chunk.Data.Span[offset..], output.AsSpan(totalWritten, outBlockSize), interleave, state);

                    offset += interval;
                }

                yield return DecorateLoop(new Sample
                {
                    Data = output,
                }, state);
            }
        }

        yield break;

        static Sample DecorateLoop(Sample sample, VagState state)
        {
            if (state.LoopStart is not { } loopStart) 
                return sample;

            sample[NumericData.LoopStart] = state.LoopStart;
                
            if (state.LoopEnd is {} loopEnd)
                sample[NumericData.LoopLength] = loopEnd - loopStart;
            else
                sample[NumericData.LoopLength] = sample.Data.Length - loopStart;

            return sample;
        }
    }
}