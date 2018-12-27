using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace CSCore.Codecs.FLAC
{
    internal sealed class FlacSubFrameFixed : FlacSubFrameBase
    {
        public FlacSubFrameFixed(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int bitsPerSample, int order)
            : base(header)
        {
            var resi = data.ResidualBuffer.Span;
            var dest = data.DestinationBuffer.Span;
            for (var i = 0; i < order; i++) //order = predictor order
            {
                resi[i] = dest[i] = reader.ReadBitsSigned(bitsPerSample);
            }

            new FlacResidual(reader, header, data, order); //necessary for decoding
            RestoreSignal(data, header.BlockSize - order, order);
        }

        private void RestoreSignal(FlacSubFrameData subframeData, int length, int order)
        {
            //see ftp://svr-ftp.eng.cam.ac.uk/pub/reports/auto-pdf/robinson_tr156.pdf chapter 3.2
            var residual = subframeData.ResidualBuffer.Span; //.Slice(order);
            var destBuffer = subframeData.DestinationBuffer.Span; //.Slice(order);

            switch (order)
            {
                case 0:
                {
                    residual.Slice(0, length).CopyTo(destBuffer);
                    break;                    
                }

                case 1:
                {
                    for (var i = 0; i < length; i++)
                    {
                        //s(t-1)
                        destBuffer[i + 1] = residual[i + 1] + destBuffer[i];
                    }
                    break;                    
                }

                case 2:
                    for (var i = 0; i < length; i++)
                    {
                        //2s(t-1) - s(t-2)
                        destBuffer[i + 2] = residual[i + 2] + 
                                            2 * destBuffer[i + 1] - 
                                            destBuffer[i];
                    }
                    break;

                case 3:
                    for (var t = 0; t < length; t++)
                    {
                        //3s(t-1) - 3s(t-2) + s(t-3)
                        destBuffer[t + 3] = residual[t + 3] + 
                                            3 * destBuffer[t + 2] - 
                                            3 * destBuffer[t + 1] + 
                                            destBuffer[t]; 
                    }
                    break;

                case 4:
                    //"FLAC adds a fourth-order predictor to the zero-to-third-order predictors used by Shorten." (see https://xiph.org/flac/format.html#prediction)
                    for (var t = 0; t < length; t++)
                    {
                        destBuffer[t + 4] = residual[t + 4] +
                            4 * destBuffer[t + 3] - 
                            6 * destBuffer[t + 2] + 
                            4 * destBuffer[t + 1] - 
                            destBuffer[t];
                    }
                    break;

                default:
                    Debug.WriteLine("Invalid FlacFixedSubFrame predictororder.");
                    return;
            }
        }
    }
}