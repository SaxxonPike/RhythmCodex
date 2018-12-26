using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace CSCore.Codecs.FLAC
{
// ReSharper disable once InconsistentNaming
    internal sealed partial class FlacSubFrameLPC : FlacSubFrameBase
    {
        public unsafe FlacSubFrameLPC(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int bitsPerSample, int order)
            : base(header)
        {
            var warmup = new int[order];
            for (var i = 0; i < order; i++)
            {
                warmup[i] = data.ResidualBuffer[i] = reader.ReadBitsSigned(bitsPerSample);
            }

            var coefPrecision = (int)reader.ReadBits(4);
            if (coefPrecision == 0x0F)
                throw new FlacException("Invalid \"quantized linear predictor coefficients' precision in bits\" was invalid. Must not be 0x0F.",
                    FlacLayer.SubFrame);
            coefPrecision += 1;

            var shiftNeeded = reader.ReadBitsSigned(5);
            if (shiftNeeded < 0)
                throw new FlacException("'\"Quantized linear predictor coefficient shift needed in bits\" was negative.", FlacLayer.SubFrame);

            var q = new int[order];
            for (var i = 0; i < order; i++)
            {
                q[i] = reader.ReadBitsSigned(coefPrecision);
            }

            //decode the residual
            var residual = new FlacResidual(reader, header, data, order);
            for (var i = 0; i < order; i++)
            {
                data.DestinationBuffer[i] = data.ResidualBuffer[i];
            }

            var residualBuffer0 = data.ResidualBuffer + order;
            var destinationBuffer0 = data.DestinationBuffer + order;
            var blockSizeToProcess = header.BlockSize - order;

            if (bitsPerSample + coefPrecision + Log2(order) <= 32)
            {
                RestoreLPCSignal32(residualBuffer0, destinationBuffer0, blockSizeToProcess, order, q, shiftNeeded);
            }
            else
            {
                RestoreLPCSignal64(residualBuffer0, destinationBuffer0, blockSizeToProcess, order, q, shiftNeeded);
            }
        }

        /// <summary>
        /// Copied from http://stackoverflow.com/questions/8970101/whats-the-quickest-way-to-compute-log2-of-an-integer-in-c 14.01.2015
        /// </summary>
        private int Log2(int x)
        {
            var bits = 0;
            while (x > 0)
            {
                bits++;
                x >>= 1;
            }
            return bits;
        }
    }
}