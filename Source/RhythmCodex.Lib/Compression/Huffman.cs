using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Compression
{
    public class Huffman
    {
        // this is a port of huffman.c
        // from the MAME source code originally written by Aaron Giles
        // more information:
        // https://github.com/mamedev/mame/blob/master/src/lib/util/huffman.c
        // https://github.com/mamedev/mame/blob/master/src/lib/util/huffman.h

        private readonly List<uint> _dataHisto = new();
        private readonly HuffmanNode[] _huffNode;
        private readonly ushort[] _lookup;
        private readonly byte _maxBits;
        private readonly uint _numCodes;

        public Huffman(int newNumCodes, int newMaxBits, ushort[] newLookup, uint[] newHisto, HuffmanNode[] newNodes)
        {
            _numCodes = (uint) newNumCodes;
            _maxBits = (byte) newMaxBits;
            if (newLookup != null)
                _lookup = newLookup;
            else
                _lookup = new ushort[1 << _maxBits];
            if (newHisto != null)
                _dataHisto.AddRange(newHisto);
            if (newNodes != null)
                _huffNode = newNodes;
            else
                _huffNode = Enumerable.Range(0, newNumCodes).Select(i => new HuffmanNode()).ToArray();
        }

        private void AssignCanonicalCodes()
        {
            // build up a histogram of bit lengths
            var bitHisto = new uint[33];
            for (var curCode = 0; curCode < _numCodes; curCode++)
            {
                var node = _huffNode[curCode];
                if (node.NumBits > _maxBits)
                    throw new Exception("Canonical code error- internal inconsistency.");
                if (node.NumBits <= 32)
                    bitHisto[node.NumBits]++;
            }

            // for each code length, determine the starting code number
            uint curStart = 0;
            for (var codeLen = 32; codeLen > 0; codeLen--)
            {
                var nextStart = (curStart + bitHisto[codeLen]) >> 1;
                if (codeLen != 1 && (nextStart * 2) != (curStart + bitHisto[codeLen]))
                    throw new Exception("Canonical code error- internal inconsistency.");
                bitHisto[codeLen] = curStart;
                curStart = nextStart;
            }

            // now assign canonical codes
            for (var curCode = 0; curCode < _numCodes; curCode++)
            {
                var node = _huffNode[curCode];
                if (node.NumBits > 0)
                    node.Bits = bitHisto[node.NumBits]++;
            }
        }

        private void BuildLookupTable()
        {
            // iterate over all codes
            for (var curCode = 0; curCode < _numCodes; curCode++)
            {
                // process all nodes which have non-zero bits
                var node = _huffNode[curCode];
                if (node.NumBits > 0)
                {
                    // set up the entry
                    var value = MakeLookup(curCode, node.NumBits);

                    // fill all matching entries
                    var shift = _maxBits - node.NumBits;
                    var dest = node.Bits << shift;
                    var destEnd = ((node.Bits + 1) << shift) - 1;
                    while (dest <= destEnd)
                    {
                        _lookup[(int) dest] = value;
                        dest++;
                    }
                }
            }
        }

        public void ImportTreeRle(BitReader reader)
        {
            // bits per entry depends on the maxbits
            int numBits;
            if (_maxBits >= 16)
                numBits = 5;
            else if (_maxBits >= 8)
                numBits = 4;
            else
                numBits = 3;

            // loop until we read all the nodes
            int curNode;
            for (curNode = 0; curNode < _numCodes;)
            {
                // a non-one value is just raw
                var nodeBits = (byte) reader.Read(numBits);
                if (nodeBits != 1)
                    _huffNode[curNode++].NumBits = nodeBits;

                // a one value is an escape code
                else
                {
                    // a double 1 is just a single 1
                    nodeBits = (byte) reader.Read(numBits);
                    if (nodeBits == 1)
                        _huffNode[curNode++].NumBits = nodeBits;

                    // otherwise, we need one for value for the repeat count
                    else
                    {
                        var repcount = reader.Read(numBits) + 3;
                        while (repcount-- > 0)
                        {
                            _huffNode[curNode++].NumBits = nodeBits;
                        }
                    }
                }
            }

            // make sure we ended up with the right number
            if (curNode != _numCodes)
                throw new Exception("Huffman tree import error- nodes != codes");

            // assign canonical codes for all nodes based on their code lengths
            AssignCanonicalCodes();

            // build the lookup table
            BuildLookupTable();
        }

        public int DecodeOne(BitReader reader)
        {
            // peek ahead to get maxbits worth of data
            var bits = reader.Peek(_maxBits);

            // look it up, then remove the actual number of bits for this code
            var lookup = _lookup[bits];
            reader.Remove(lookup & 0x1f);

            // return the value
            return lookup >> 5;
        }

        private static ushort MakeLookup(int code, int bits) =>
            (ushort) ((code << 5) | (bits & 0x1F));
    }
}