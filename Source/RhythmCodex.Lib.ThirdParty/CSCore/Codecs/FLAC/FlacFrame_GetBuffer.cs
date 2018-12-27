using System;

namespace CSCore.Codecs.FLAC
{
    public partial class FlacFrame
    {
        private int GetBufferInternal(ref Memory<byte> mem)
        {
            var desiredsize = Header.BlockSize * Header.Channels * ((Header.BitsPerSample + 7) / 2);
            if (mem.Length < desiredsize)
                mem = new byte[desiredsize];
            var ptrBuffer = mem.Span;

            {
                var ptr = 0;
                switch (Header.BitsPerSample)
                {
#region 8
                    case 8:
                        switch (Header.Channels)
                        {
                            case 1:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[0].DestinationBuffer.Span[i] + 0x80);
                                }
                                break;
                            case 2:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[0].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[1].DestinationBuffer.Span[i] + 0x80);
                                }
                                break;
                            case 3:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[0].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[1].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[2].DestinationBuffer.Span[i] + 0x80);
                                }
                                break;
                            case 4:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[0].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[1].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[2].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[3].DestinationBuffer.Span[i] + 0x80);
                                }
                                break;
                            case 5:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[0].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[1].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[2].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[3].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[4].DestinationBuffer.Span[i] + 0x80);
                                }
                                break;
                            case 6:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[0].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[1].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[2].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[3].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[4].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[5].DestinationBuffer.Span[i] + 0x80);
                                }
                                break;
                            case 7:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[0].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[1].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[2].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[3].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[4].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[5].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[6].DestinationBuffer.Span[i] + 0x80);
                                }
                                break;
                            case 8:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[0].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[1].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[2].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[3].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[4].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[5].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[6].DestinationBuffer.Span[i] + 0x80);
                                    ptrBuffer[ptr++] = (byte)(_subFrameData[7].DestinationBuffer.Span[i] + 0x80);
                                }
                                break;
                            default:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    for (var c = 0; c < Header.Channels; c++)
                                    {
                                        ptrBuffer[ptr++] = (byte)(_subFrameData[c].DestinationBuffer.Span[i] + 0x80);
                                    }
                                }
                                break;
                        }
                        break;
#endregion
#region 16
                    case 16:
                        short vals;
                        switch (Header.Channels)
                        {
                            case 1:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vals = (short)(_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                }
                                break;
                            case 2:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vals = (short)(_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                }
                                break;
                            case 3:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vals = (short)(_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                }
                                break;
                            case 4:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vals = (short)(_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                }
                                break;
                            case 5:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vals = (short)(_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[4].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                }
                                break;
                            case 6:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vals = (short)(_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[4].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[5].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                }
                                break;
                            case 7:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vals = (short)(_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[4].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[5].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[6].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                }
                                break;
                            case 8:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vals = (short)(_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[4].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[5].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[6].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                    vals = (short)(_subFrameData[7].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vals & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vals >> 8) & 0xFF);

                                }
                                break;
                            default:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    for (var c = 0; c < Header.Channels; c++)
                                    {
                                        var val = (short)(_subFrameData[c].DestinationBuffer.Span[i]);
                                        ptrBuffer[ptr++] = (byte)(val & 0xFF);
                                        ptrBuffer[ptr++] = (byte)((val >> 8) & 0xFF);
                                    }
                                }
                                break;
                        }
                        break;
#endregion
#region 24
                    case 24:
                        int   vali;
                        switch (Header.Channels)
                        {
                            case 1:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vali = (_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                }
                                break;
                            case 2:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vali = (_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                }
                                break;
                            case 3:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vali = (_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                }
                                break;
                            case 4:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vali = (_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                }
                                break;
                            case 5:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vali = (_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[4].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                }
                                break;
                            case 6:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vali = (_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[4].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[5].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                }
                                break;
                            case 7:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vali = (_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[4].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[5].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[6].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                }
                                break;
                            case 8:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    vali = (_subFrameData[0].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[1].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[2].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[3].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[4].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[5].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[6].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                    vali = (_subFrameData[7].DestinationBuffer.Span[i]);
                                    ptrBuffer[ptr++] = (byte)(vali & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 8) & 0xFF);
                                    ptrBuffer[ptr++] = (byte)((vali >> 16) & 0xFF);

                                }
                                break;
                            default:
                                for (var i = 0; i < Header.BlockSize; i++)
                                {
                                    for (var c = 0; c < Header.Channels; c++)
                                    {
                                        var val = (_subFrameData[c].DestinationBuffer.Span[i]);
                                        ptrBuffer[ptr++] = (byte)(val & 0xFF);
                                        ptrBuffer[ptr++] = (byte)((val >> 8) & 0xFF);
                                        ptrBuffer[ptr++] = (byte)((val >> 16) & 0xFF);
                                    }
                                }
                                break;
                        }
                        break;
#endregion
                    default: //default bits per sample
                        throw new FlacException(
                            $"FlacFrame::GetBuffer: Invalid BitsPerSample value: {Header.BitsPerSample}", FlacLayer.Frame);
                }
                return ptr;
            }
        }
    }
}