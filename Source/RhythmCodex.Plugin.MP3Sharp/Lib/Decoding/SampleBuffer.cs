// /***************************************************************************
//  * SampleBuffer.cs
//  * Copyright (c) 2015 the authors.
//  * 
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the GNU Lesser General Public License
//  * (LGPL) version 3 which accompanies this distribution, and is available at
//  * https://www.gnu.org/licenses/lgpl-3.0.en.html
//  *
//  * This library is distributed in the hope that it will be useful,
//  * but WITHOUT ANY WARRANTY; without even the implied warranty of
//  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  * Lesser General Public License for more details.
//  *
//  ***************************************************************************/
namespace RhythmCodex.Plugin.MP3Sharp.Lib.Decoding;

/// <summary>
///     The SampleBuffer class implements an output buffer
///     that provides storage for a fixed size block of samples.
/// </summary>
internal sealed class SampleBuffer : ABuffer
{
    private readonly int[] bufferp;

    /// <summary>
    ///     Constructor
    /// </summary>
    public SampleBuffer(int sample_frequency, int number_of_channels)
    {
        Buffer = new short[OBUFFERSIZE];
        bufferp = new int[MAXCHANNELS];
        ChannelCount = number_of_channels;
        SampleFrequency = sample_frequency;

        for (var i = 0; i < number_of_channels; ++i)
            bufferp[i] = (short) i;
    }

    public int ChannelCount { get; }

    public int SampleFrequency { get; }

    public short[] Buffer { get; }

    public int BufferLength => bufferp[0];

    /// <summary>
    ///     Takes a 16 Bit PCM sample.
    /// </summary>
    public override void Append(int channel, short valueRenamed)
    {
        Buffer[bufferp[channel]] = valueRenamed;
        bufferp[channel] += ChannelCount;
    }

    public override void AppendSamples(int channel, float[] f)
    {
        var pos = bufferp[channel];

        short s;
        float fs;
        for (var i = 0; i < 32;)
        {
            fs = f[i++];
            fs = (fs > 32767.0f ? 32767.0f : (fs < -32767.0f ? -32767.0f : fs));

            //UPGRADE_WARNING: Narrowing conversions may produce unexpected results in C#. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042"'
            s = (short) fs;
            Buffer[pos] = s;
            pos += ChannelCount;
        }

        bufferp[channel] = pos;
    }

    /// <summary>
    ///     Write the samples to the file (Random Acces).
    /// </summary>
    public override void WriteBuffer(int val)
    {
        //for (int i = 0; i < channels; ++i) 
        //	bufferp[i] = (short)i;
    }

    public override void Close()
    {
    }

    /// <summary>
    ///     *
    /// </summary>
    public override void ClearBuffer()
    {
        for (var i = 0; i < ChannelCount; ++i)
            bufferp[i] = (short) i;
    }

    /// <summary>
    ///     *
    /// </summary>
    public override void SetStopFlag()
    {

    }
}