using RhythmCodex.IoC;
using RhythmCodex.Thirdparty;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class Ddr573ChecksumCalculator : IDdr573ChecksumCalculator
    {
        // Known to be incorrect, placeholder
        // TODO: figure this out for real

        public int CalculateChecksum(byte[] data)
        {
            var crc32 = new Crc32(Crc32.DefaultPolynomial, 0xA8E06D56);
            var hash = crc32.ComputeHash(data);
            return hash[0] | (hash[1] << 8) | (hash[2] << 16) | (hash[3] << 24);
//            
//            const int msbBit = ~0x7FFFFFFF;
//            const int core = unchecked((int) 0xA8E06D56);
//            var sum = core;
//
//            unchecked
//            {
//                foreach (var d in data)
//                {
//                    var msb = sum & msbBit;
//                    sum <<= 1;
//                    sum ^= msb >> 31;
//                }
//            }
//
//            return sum;
        }
    }
}