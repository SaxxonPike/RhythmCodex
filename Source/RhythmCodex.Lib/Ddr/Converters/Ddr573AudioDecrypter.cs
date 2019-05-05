using RhythmCodex.Ddr.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class Ddr573AudioDecrypter : IDdr573AudioDecrypter
    {
        public byte[] Decrypt(byte[] data, byte[] key, byte[] scramble, int counter)
        {
            var dataLen = data.Length & ~1;
            var output = new byte[dataLen];
            var keyLen = key.Length;
            var scrambleLen = scramble.Length;
            var outputIdx = 0;
            var keyIdx = 0;
            var scrambleIdx = 0;

            for (var idx = 0; idx < dataLen; idx += 2)
            {
                while (keyIdx >= keyLen)
                    keyIdx -= keyLen;
                while (scrambleIdx >= scrambleLen)
                    scrambleIdx -= scrambleLen;

                var outputWord = 0;
                var curData = (data[idx + 1] << 8) | data[idx];
                var curKey = key[keyIdx++];
                var curScramble = scramble[scrambleIdx++];

                for (var curBit = 0; curBit < 8; curBit++)
                {
                    var evenBitShift = (curBit << 1) & 0xFF;
                    var oddBitShift = ((curBit << 1) + 1) & 0xFF;
                    var isEvenBitSet = (curData & (1 << evenBitShift)) != 0;
                    var isOddBitSet = (curData & (1 << oddBitShift)) != 0;
                    var isKeyBitSet = (curKey & (1 << curBit)) != 0;
                    var isScrambleBitSet = (curScramble & (1 << curBit)) != 0;
                    var isCounterBitSet = (counter & (1 << curBit)) != 0;
                    var isCounterBitInvSet = (counter & (1 << ((7 - curBit) & 0xFF))) != 0;

                    if (isScrambleBitSet)
                    {
                        var temp = isEvenBitSet;
                        isEvenBitSet = isOddBitSet;
                        isOddBitSet = temp;
                    }

                    if (isEvenBitSet ^ isCounterBitInvSet ^ isKeyBitSet)
                        outputWord |= 1 << evenBitShift;
                    if (isOddBitSet ^ isCounterBitSet)
                        outputWord |= 1 << oddBitShift;
                }

                output[outputIdx] = unchecked((byte) (outputWord >> 8));
                output[outputIdx + 1] = unchecked((byte) outputWord);
                outputIdx += 2;
                counter = (counter + 1) & 0xFF;
            }

            return output;
        }
    }
}