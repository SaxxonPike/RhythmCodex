// BZip2InputStream.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2011 Dino Chiesa.
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License.
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// Last Saved: <2011-July-31 11:57:32>
//
// ------------------------------------------------------------------
//
// This module defines the BZip2InputStream class, which is a decompressing
// stream that handles BZIP2. This code is derived from Apache commons source code.
// The license below applies to the original Apache code.
//
// ------------------------------------------------------------------

/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

/*
 * This package is based on the work done by Keiron Liddle, Aftex Software
 * <keiron@aftexsw.com> to whom the Ant project is very grateful for his
 * great code.
 */

// compile: msbuild
// not: csc.exe /t:library /debug+ /out:Ionic.BZip2.dll BZip2InputStream.cs BCRC32.cs Rand.cs


using System;
using System.IO;

// ReSharper disable CheckNamespace
namespace Ionic.BZip2
{
    /// <summary>
    ///   A read-only decorator stream that performs BZip2 decompression on Read.
    /// </summary>
    public class BZip2InputStream : Stream
    {
        private bool _disposed;
        private readonly bool _leaveOpen;
        long _totalBytesRead;
        private int _last;

        /* for undoing the Burrows-Wheeler transform */
        private int _origPtr;

        // blockSize100k: 0 .. 9.
        //
        // This var name is a misnomer. The actual block size is 100000
        // * blockSize100k. (not 100k * blocksize100k)
        private int _blockSize100K;
        private bool _blockRandomised;
        private int _bsBuff;
        private int _bsLive;
        private readonly IonicCrc32 _crc = new IonicCrc32(true);
        private int _nInUse;
        private Stream _input;
        private int _currentChar = -1;

        /// <summary>
        ///   Compressor State
        /// </summary>
        private enum CState
        {
            Eof = 0,
            StartBlock = 1,
            RandPartA = 2,
            RandPartB = 3,
            RandPartC = 4,
            NoRandPartA = 5,
            NoRandPartB = 6,
            NoRandPartC = 7,
        }

        private CState _currentState = CState.StartBlock;

        private uint _storedBlockCrc, _storedCombinedCrc;
        private uint _computedBlockCrc, _computedCombinedCrc;

        // Variables used by setup* methods exclusively
        private int _suCount;
        private int _suCh2;
        private int _suChPrev;
        private int _suI2;
        private int _suJ2;
        private int _suRnToGo;
        private int _suRtPos;
        private int _suTPos;
        private char _suZ;
        private DecompressionState _data;


        /// <summary>
        ///   Create a BZip2InputStream with the given stream, and
        ///   specifying whether to leave the wrapped stream open when
        ///   the BZip2InputStream is closed.
        /// </summary>
        /// <param name='input'>The stream from which to read compressed data</param>
        /// <param name='leaveOpen'>
        ///   Whether to leave the input stream open, when the BZip2InputStream closes.
        /// </param>
        ///
        /// <example>
        ///
        ///   This example reads a bzip2-compressed file, decompresses it,
        ///   and writes the decompressed data into a newly created file.
        ///
        ///   <code>
        ///   var fname = "logfile.log.bz2";
        ///   using (var fs = File.OpenRead(fname))
        ///   {
        ///       using (var decompressor = new Ionic.BZip2.BZip2InputStream(fs))
        ///       {
        ///           var outFname = fname + ".decompressed";
        ///           using (var output = File.Create(outFname))
        ///           {
        ///               byte[] buffer = new byte[2048];
        ///               int n;
        ///               while ((n = decompressor.Read(buffer, 0, buffer.Length)) > 0)
        ///               {
        ///                   output.Write(buffer, 0, n);
        ///               }
        ///           }
        ///       }
        ///   }
        ///   </code>
        /// </example>
        public BZip2InputStream(Stream input, bool leaveOpen = false)
        {
            _input = input;
            _leaveOpen = leaveOpen;
            Init();
        }

        /// <summary>
        ///   Read data from the stream.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     To decompress a BZip2 data stream, create a <c>BZip2InputStream</c>,
        ///     providing a stream that reads compressed data.  Then call Read() on
        ///     that <c>BZip2InputStream</c>, and the data read will be decompressed
        ///     as you read.
        ///   </para>
        ///
        ///   <para>
        ///     A <c>BZip2InputStream</c> can be used only for <c>Read()</c>, not for <c>Write()</c>.
        ///   </para>
        /// </remarks>
        ///
        /// <param name="buffer">The buffer into which the read data should be placed.</param>
        /// <param name="offset">the offset within that data array to put the first byte read.</param>
        /// <param name="count">the number of bytes to read.</param>
        /// <returns>the number of bytes actually read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset < 0)
                throw new IndexOutOfRangeException($"offset ({offset}) must be > 0");

            if (count < 0)
                throw new IndexOutOfRangeException($"count ({count}) must be > 0");

            if (offset + count > buffer.Length)
                throw new IndexOutOfRangeException($"offset({offset}) count({count}) bLength({buffer.Length})");

            if (_input == null)
                throw new IOException("the stream is not open");


            var hi = offset + count;
            var destOffset = offset;
            for (int b; (destOffset < hi) && ((b = ReadByte()) >= 0);)
            {
                buffer[destOffset++] = (byte) b;
            }

            return (destOffset == offset) ? -1 : (destOffset - offset);
        }

        private void MakeMaps()
        {
            var inUse = _data.InUse;
            var seqToUnseq = _data.SeqToUnseq;

            var n = 0;

            for (var i = 0; i < 256; i++)
            {
                if (inUse[i])
                    seqToUnseq[n++] = (byte) i;
            }

            _nInUse = n;
        }

        /// <summary>
        ///   Read a single byte from the stream.
        /// </summary>
        /// <returns>the byte read from the stream, or -1 if EOF</returns>
        public override int ReadByte()
        {
            var retChar = _currentChar;
            _totalBytesRead++;
            switch (_currentState)
            {
                case CState.Eof:
                    return -1;

                case CState.StartBlock:
                    throw new IOException("bad state");

                case CState.RandPartA:
                    throw new IOException("bad state");

                case CState.RandPartB:
                    SetupRandPartB();
                    break;

                case CState.RandPartC:
                    SetupRandPartC();
                    break;

                case CState.NoRandPartA:
                    throw new IOException("bad state");

                case CState.NoRandPartB:
                    SetupNoRandPartB();
                    break;

                case CState.NoRandPartC:
                    SetupNoRandPartC();
                    break;

                default:
                    throw new IOException("bad state");
            }

            return retChar;
        }


        /// <summary>
        /// Indicates whether the stream can be read.
        /// </summary>
        /// <remarks>
        /// The return value depends on whether the captive stream supports reading.
        /// </remarks>
        public override bool CanRead
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("BZip2Stream");
                return _input.CanRead;
            }
        }


        /// <summary>
        /// Indicates whether the stream supports Seek operations.
        /// </summary>
        /// <remarks>
        /// Always returns false.
        /// </remarks>
        public override bool CanSeek => false;


        /// <summary>
        /// Indicates whether the stream can be written.
        /// </summary>
        /// <remarks>
        /// The return value depends on whether the captive stream supports writing.
        /// </remarks>
        public override bool CanWrite
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("BZip2Stream");
                return _input.CanWrite;
            }
        }

        /// <summary>
        /// Flush the stream.
        /// </summary>
        public override void Flush()
        {
            if (_disposed) throw new ObjectDisposedException("BZip2Stream");
            _input.Flush();
        }

        /// <summary>
        /// Reading this property always throws a <see cref="NotImplementedException"/>.
        /// </summary>
        public override long Length => throw new NotImplementedException();

        /// <summary>
        /// The position of the stream pointer.
        /// </summary>
        ///
        /// <remarks>
        ///   Setting this property always throws a <see
        ///   cref="NotImplementedException"/>. Reading will return the
        ///   total number of uncompressed bytes read in.
        /// </remarks>
        public override long Position
        {
            get => _totalBytesRead;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Calling this method always throws a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="offset">this is irrelevant, since it will always throw!</param>
        /// <param name="origin">this is irrelevant, since it will always throw!</param>
        /// <returns>irrelevant!</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calling this method always throws a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value">this is irrelevant, since it will always throw!</param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Calling this method always throws a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name='buffer'>this parameter is never used</param>
        /// <param name='offset'>this parameter is never used</param>
        /// <param name='count'>this parameter is never used</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///   Dispose the stream.
        /// </summary>
        /// <param name="disposing">
        ///   indicates whether the Dispose method was invoked by user code.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!_disposed)
                {
                    if (disposing)
                        _input?.Close();
                    _disposed = true;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }


        void Init()
        {
            if (null == _input)
                throw new IOException("No input Stream");

            if (!_input.CanRead)
                throw new IOException("Unreadable input Stream");

            CheckMagicChar('B', 0);
            CheckMagicChar('Z', 1);
            CheckMagicChar('h', 2);

            var blockSize = _input.ReadByte();

            if ((blockSize < '1') || (blockSize > '9'))
                throw new IOException("Stream is not BZip2 formatted: illegal "
                                      + "blocksize " + (char) blockSize);

            _blockSize100K = blockSize - '0';

            InitBlock();
            SetupBlock();
        }


        void CheckMagicChar(char expected, int position)
        {
            var magic = _input.ReadByte();
            if (magic != expected)
            {
                var msg = $"Not a valid BZip2 stream. byte {position}, expected '{(int) expected}', got '{magic}'";
                throw new IOException(msg);
            }
        }


        void InitBlock()
        {
            var magic0 = BsGetUByte();
            var magic1 = BsGetUByte();
            var magic2 = BsGetUByte();
            var magic3 = BsGetUByte();
            var magic4 = BsGetUByte();
            var magic5 = BsGetUByte();

            if (magic0 == 0x17 && magic1 == 0x72 && magic2 == 0x45
                && magic3 == 0x38 && magic4 == 0x50 && magic5 == 0x90)
            {
                Complete(); // end of file
            }
            else if (magic0 != 0x31 ||
                     magic1 != 0x41 ||
                     magic2 != 0x59 ||
                     magic3 != 0x26 ||
                     magic4 != 0x53 ||
                     magic5 != 0x59)
            {
                _currentState = CState.Eof;
                var msg = $"bad block header at offset 0x{_input.Position:X}";
                throw new IOException(msg);
            }
            else
            {
                _storedBlockCrc = BsGetInt();
                // Console.WriteLine(" stored block CRC     : {0:X8}", this.storedBlockCRC);

                _blockRandomised = (GetBits(1) == 1);

                // Lazily allocate data
                if (_data == null)
                    _data = new DecompressionState(_blockSize100K);

                // currBlockNo++;
                GetAndMoveToFrontDecode();

                _crc.Reset();
                _currentState = CState.StartBlock;
            }
        }


        private void EndBlock()
        {
            _computedBlockCrc = (uint) _crc.Crc32Result;

            // A bad CRC is considered a fatal error.
            if (_storedBlockCrc != _computedBlockCrc)
            {
                // make next blocks readable without error
                // (repair feature, not yet documented, not tested)
                // this.computedCombinedCRC = (this.storedCombinedCRC << 1)
                //     | (this.storedCombinedCRC >> 31);
                // this.computedCombinedCRC ^= this.storedBlockCRC;

                var msg = $"BZip2 CRC error (expected {_storedBlockCrc:X8}, computed {_computedBlockCrc:X8})";
                throw new IOException(msg);
            }

            // Console.WriteLine(" combined CRC (before): {0:X8}", this.computedCombinedCRC);
            _computedCombinedCrc = (_computedCombinedCrc << 1)
                                  | (_computedCombinedCrc >> 31);
            _computedCombinedCrc ^= _computedBlockCrc;
            // Console.WriteLine(" computed block  CRC  : {0:X8}", this.computedBlockCRC);
            // Console.WriteLine(" combined CRC (after) : {0:X8}", this.computedCombinedCRC);
            // Console.WriteLine();
        }


        private void Complete()
        {
            _storedCombinedCrc = BsGetInt();
            _currentState = CState.Eof;
            _data = null;

            if (_storedCombinedCrc != _computedCombinedCrc)
            {
                var msg = $"BZip2 CRC error (expected {_storedCombinedCrc:X8}, computed {_computedCombinedCrc:X8})";

                throw new IOException(msg);
            }
        }

        /// <summary>
        ///   Close the stream.
        /// </summary>
        public override void Close()
        {
            var inShadow = _input;
            if (inShadow != null)
            {
                try
                {
                    if (!_leaveOpen)
                        inShadow.Close();
                }
                finally
                {
                    _data = null;
                    _input = null;
                }
            }
        }


        /// <summary>
        ///   Read n bits from input, right justifying the result.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     For example, if you read 1 bit, the result is either 0
        ///     or 1.
        ///   </para>
        /// </remarks>
        /// <param name ="n">
        ///   The number of bits to read, always between 1 and 32.
        /// </param>
        private int GetBits(int n)
        {
            var bsLiveShadow = _bsLive;
            var bsBuffShadow = _bsBuff;

            if (bsLiveShadow < n)
            {
                do
                {
                    var thech = _input.ReadByte();

                    if (thech < 0)
                        throw new IOException("unexpected end of stream");

                    // Console.WriteLine("R {0:X2}", thech);

                    bsBuffShadow = (bsBuffShadow << 8) | thech;
                    bsLiveShadow += 8;
                } while (bsLiveShadow < n);

                _bsBuff = bsBuffShadow;
            }

            _bsLive = bsLiveShadow - n;
            return (bsBuffShadow >> (bsLiveShadow - n)) & ((1 << n) - 1);
        }


        // private bool bsGetBit()
        // {
        //     int bsLiveShadow = this.bsLive;
        //     int bsBuffShadow = this.bsBuff;
        //
        //     if (bsLiveShadow < 1)
        //     {
        //         int thech = this.input.ReadByte();
        //
        //         if (thech < 0)
        //         {
        //             throw new IOException("unexpected end of stream");
        //         }
        //
        //         bsBuffShadow = (bsBuffShadow << 8) | thech;
        //         bsLiveShadow += 8;
        //         this.bsBuff = bsBuffShadow;
        //     }
        //
        //     this.bsLive = bsLiveShadow - 1;
        //     return ((bsBuffShadow >> (bsLiveShadow - 1)) & 1) != 0;
        // }

        private bool BsGetBit()
        {
            var bit = GetBits(1);
            return bit != 0;
        }

        private char BsGetUByte()
        {
            return (char) GetBits(8);
        }

        private uint BsGetInt()
        {
            return (uint) ((((((GetBits(8) << 8) | GetBits(8)) << 8) | GetBits(8)) << 8) | GetBits(8));
        }


        /**
         * Called by createHuffmanDecodingTables() exclusively.
         */
        private static void HbCreateDecodeTables(int[] limit,
            int[] bbase, int[] perm, char[] length,
            int minLen, int maxLen, int alphaSize)
        {
            for (int i = minLen, pp = 0; i <= maxLen; i++)
            {
                for (var j = 0; j < alphaSize; j++)
                {
                    if (length[j] == i)
                    {
                        perm[pp++] = j;
                    }
                }
            }

            for (var i = BZip2.MaxCodeLength; --i > 0;)
            {
                bbase[i] = 0;
                limit[i] = 0;
            }

            for (var i = 0; i < alphaSize; i++)
            {
                bbase[length[i] + 1]++;
            }

            for (int i = 1, b = bbase[0]; i < BZip2.MaxCodeLength; i++)
            {
                b += bbase[i];
                bbase[i] = b;
            }

            for (int i = minLen, vec = 0, b = bbase[i]; i <= maxLen; i++)
            {
                var nb = bbase[i + 1];
                vec += nb - b;
                b = nb;
                limit[i] = vec - 1;
                vec <<= 1;
            }

            for (var i = minLen + 1; i <= maxLen; i++)
            {
                bbase[i] = ((limit[i - 1] + 1) << 1) - bbase[i];
            }
        }


        private void RecvDecodingTables()
        {
            var s = _data;
            var inUse = s.InUse;
            var pos = s.RecvDecodingTablesPos;
            //byte[] selector = s.selector;

            var inUse16 = 0;

            /* Receive the mapping table */
            for (var i = 0; i < 16; i++)
            {
                if (BsGetBit())
                {
                    inUse16 |= 1 << i;
                }
            }

            for (var i = 256; --i >= 0;)
            {
                inUse[i] = false;
            }

            for (var i = 0; i < 16; i++)
            {
                if ((inUse16 & (1 << i)) != 0)
                {
                    var i16 = i << 4;
                    for (var j = 0; j < 16; j++)
                    {
                        if (BsGetBit())
                        {
                            inUse[i16 + j] = true;
                        }
                    }
                }
            }

            MakeMaps();
            var alphaSize = _nInUse + 2;

            /* Now the selectors */
            var nGroups = GetBits(3);
            var nSelectors = GetBits(15);

            for (var i = 0; i < nSelectors; i++)
            {
                var j = 0;
                while (BsGetBit())
                {
                    j++;
                }

                s.SelectorMtf[i] = (byte) j;
            }

            /* Undo the MTF values for the selectors. */
            for (var v = nGroups; --v >= 0;)
            {
                pos[v] = (byte) v;
            }

            for (var i = 0; i < nSelectors; i++)
            {
                int v = s.SelectorMtf[i];
                var tmp = pos[v];
                while (v > 0)
                {
                    // nearly all times v is zero, 4 in most other cases
                    pos[v] = pos[v - 1];
                    v--;
                }

                pos[0] = tmp;
                s.Selector[i] = tmp;
            }

            var len = s.TempCharArray2d;

            /* Now the coding tables */
            for (var t = 0; t < nGroups; t++)
            {
                var curr = GetBits(5);
                var lenT = len[t];
                for (var i = 0; i < alphaSize; i++)
                {
                    while (BsGetBit())
                    {
                        curr += BsGetBit() ? -1 : 1;
                    }

                    lenT[i] = (char) curr;
                }
            }

            // finally create the Huffman tables
            CreateHuffmanDecodingTables(alphaSize, nGroups);
        }


        /**
         * Called by recvDecodingTables() exclusively.
         */
        private void CreateHuffmanDecodingTables(int alphaSize,
            int nGroups)
        {
            var s = _data;
            var len = s.TempCharArray2d;

            for (var t = 0; t < nGroups; t++)
            {
                var minLen = 32;
                var maxLen = 0;
                var lenT = len[t];
                for (var i = alphaSize; --i >= 0;)
                {
                    var lent = lenT[i];
                    if (lent > maxLen)
                        maxLen = lent;

                    if (lent < minLen)
                        minLen = lent;
                }

                HbCreateDecodeTables(s.GLimit[t], s.GBase[t], s.GPerm[t], len[t], minLen,
                    maxLen, alphaSize);
                s.GMinlen[t] = minLen;
            }
        }


        private void GetAndMoveToFrontDecode()
        {
            var s = _data;
            _origPtr = GetBits(24);

            if (_origPtr < 0)
                throw new IOException("BZ_DATA_ERROR");
            if (_origPtr > 10 + BZip2.BlockSizeMultiple * _blockSize100K)
                throw new IOException("BZ_DATA_ERROR");

            RecvDecodingTables();

            var yy = s.GetAndMoveToFrontDecodeYy;
            var limitLast = _blockSize100K * BZip2.BlockSizeMultiple;

            /*
             * Setting up the unzftab entries here is not strictly necessary, but it
             * does save having to do it later in a separate pass, and so saves a
             * block's worth of cache misses.
             */
            for (var i = 256; --i >= 0;)
            {
                yy[i] = (byte) i;
                s.Unzftab[i] = 0;
            }

            var groupNo = 0;
            var groupPos = BZip2.GSize - 1;
            var eob = _nInUse + 1;
            var nextSym = GetAndMoveToFrontDecode0(0);
            var bsBuffShadow = _bsBuff;
            var bsLiveShadow = _bsLive;
            var lastShadow = -1;
            var zt = s.Selector[groupNo] & 0xff;
            var baseZt = s.GBase[zt];
            var limitZt = s.GLimit[zt];
            var permZt = s.GPerm[zt];
            var minLensZt = s.GMinlen[zt];

            while (nextSym != eob)
            {
                if ((nextSym == BZip2.Runa) || (nextSym == BZip2.Runb))
                {
                    var es = -1;

                    for (var n = 1;; n <<= 1)
                    {
                        if (nextSym == BZip2.Runa)
                        {
                            es += n;
                        }
                        else if (nextSym == BZip2.Runb)
                        {
                            es += n << 1;
                        }
                        else
                        {
                            break;
                        }

                        if (groupPos == 0)
                        {
                            groupPos = BZip2.GSize - 1;
                            zt = s.Selector[++groupNo] & 0xff;
                            baseZt = s.GBase[zt];
                            limitZt = s.GLimit[zt];
                            permZt = s.GPerm[zt];
                            minLensZt = s.GMinlen[zt];
                        }
                        else
                        {
                            groupPos--;
                        }

                        var zn = minLensZt;

                        // Inlined:
                        // int zvec = GetBits(zn);
                        while (bsLiveShadow < zn)
                        {
                            var thech = _input.ReadByte();
                            if (thech >= 0)
                            {
                                bsBuffShadow = (bsBuffShadow << 8) | thech;
                                bsLiveShadow += 8;
                            }
                            else
                            {
                                throw new IOException("unexpected end of stream");
                            }
                        }

                        var zvec = (bsBuffShadow >> (bsLiveShadow - zn))
                                   & ((1 << zn) - 1);
                        bsLiveShadow -= zn;

                        while (zvec > limitZt[zn])
                        {
                            zn++;
                            while (bsLiveShadow < 1)
                            {
                                var thech = _input.ReadByte();
                                if (thech >= 0)
                                {
                                    bsBuffShadow = (bsBuffShadow << 8) | thech;
                                    bsLiveShadow += 8;
                                }
                                else
                                {
                                    throw new IOException("unexpected end of stream");
                                }
                            }

                            bsLiveShadow--;
                            zvec = (zvec << 1)
                                   | ((bsBuffShadow >> bsLiveShadow) & 1);
                        }

                        nextSym = permZt[zvec - baseZt[zn]];
                    }

                    var ch = s.SeqToUnseq[yy[0]];
                    s.Unzftab[ch & 0xff] += es + 1;

                    while (es-- >= 0)
                    {
                        s.Ll8[++lastShadow] = ch;
                    }

                    if (lastShadow >= limitLast)
                        throw new IOException("block overrun");
                }
                else
                {
                    if (++lastShadow >= limitLast)
                        throw new IOException("block overrun");

                    var tmp = yy[nextSym - 1];
                    s.Unzftab[s.SeqToUnseq[tmp] & 0xff]++;
                    s.Ll8[lastShadow] = s.SeqToUnseq[tmp];

                    /*
                     * This loop is hammered during decompression, hence avoid
                     * native method call overhead of System.Buffer.BlockCopy for very
                     * small ranges to copy.
                     */
                    if (nextSym <= 16)
                    {
                        for (var j = nextSym - 1; j > 0;)
                        {
                            yy[j] = yy[--j];
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(yy, 0, yy, 1, nextSym - 1);
                    }

                    yy[0] = tmp;

                    if (groupPos == 0)
                    {
                        groupPos = BZip2.GSize - 1;
                        zt = s.Selector[++groupNo] & 0xff;
                        baseZt = s.GBase[zt];
                        limitZt = s.GLimit[zt];
                        permZt = s.GPerm[zt];
                        minLensZt = s.GMinlen[zt];
                    }
                    else
                    {
                        groupPos--;
                    }

                    var zn = minLensZt;

                    // Inlined:
                    // int zvec = GetBits(zn);
                    while (bsLiveShadow < zn)
                    {
                        var thech = _input.ReadByte();
                        if (thech >= 0)
                        {
                            bsBuffShadow = (bsBuffShadow << 8) | thech;
                            bsLiveShadow += 8;
                        }
                        else
                        {
                            throw new IOException("unexpected end of stream");
                        }
                    }

                    var zvec = (bsBuffShadow >> (bsLiveShadow - zn))
                               & ((1 << zn) - 1);
                    bsLiveShadow -= zn;

                    while (zvec > limitZt[zn])
                    {
                        zn++;
                        while (bsLiveShadow < 1)
                        {
                            var thech = _input.ReadByte();
                            if (thech >= 0)
                            {
                                bsBuffShadow = (bsBuffShadow << 8) | thech;
                                bsLiveShadow += 8;
                            }
                            else
                            {
                                throw new IOException("unexpected end of stream");
                            }
                        }

                        bsLiveShadow--;
                        zvec = (zvec << 1) | ((bsBuffShadow >> bsLiveShadow) & 1);
                    }

                    nextSym = permZt[zvec - baseZt[zn]];
                }
            }

            _last = lastShadow;
            _bsLive = bsLiveShadow;
            _bsBuff = bsBuffShadow;
        }


        private int GetAndMoveToFrontDecode0(int groupNo)
        {
            var s = _data;
            var zt = s.Selector[groupNo] & 0xff;
            var limitZt = s.GLimit[zt];
            var zn = s.GMinlen[zt];
            var zvec = GetBits(zn);
            var bsLiveShadow = _bsLive;
            var bsBuffShadow = _bsBuff;

            while (zvec > limitZt[zn])
            {
                zn++;
                while (bsLiveShadow < 1)
                {
                    var thech = _input.ReadByte();

                    if (thech >= 0)
                    {
                        bsBuffShadow = (bsBuffShadow << 8) | thech;
                        bsLiveShadow += 8;
                    }
                    else
                    {
                        throw new IOException("unexpected end of stream");
                    }
                }

                bsLiveShadow--;
                zvec = (zvec << 1) | ((bsBuffShadow >> bsLiveShadow) & 1);
            }

            _bsLive = bsLiveShadow;
            _bsBuff = bsBuffShadow;

            return s.GPerm[zt][zvec - s.GBase[zt][zn]];
        }


        private void SetupBlock()
        {
            if (_data == null)
                return;

            int i;
            var s = _data;
            var tt = s.InitTt(_last + 1);

            //       xxxx

            /* Check: unzftab entries in range. */
            for (i = 0; i <= 255; i++)
            {
                if (s.Unzftab[i] < 0 || s.Unzftab[i] > _last)
                    throw new Exception("BZ_DATA_ERROR");
            }

            /* Actually generate cftab. */
            s.Cftab[0] = 0;
            for (i = 1; i <= 256; i++) s.Cftab[i] = s.Unzftab[i - 1];
            for (i = 1; i <= 256; i++) s.Cftab[i] += s.Cftab[i - 1];
            /* Check: cftab entries in range. */
            for (i = 0; i <= 256; i++)
            {
                if (s.Cftab[i] < 0 || s.Cftab[i] > _last + 1)
                {
                    var msg = $"BZ_DATA_ERROR: cftab[{i}]={s.Cftab[i]} last={_last}";
                    throw new Exception(msg);
                }
            }

            /* Check: cftab entries non-descending. */
            for (i = 1; i <= 256; i++)
            {
                if (s.Cftab[i - 1] > s.Cftab[i])
                    throw new Exception("BZ_DATA_ERROR");
            }

            int lastShadow;
            for (i = 0, lastShadow = _last; i <= lastShadow; i++)
            {
                tt[s.Cftab[s.Ll8[i] & 0xff]++] = i;
            }

            if ((_origPtr < 0) || (_origPtr >= tt.Length))
                throw new IOException("stream corrupted");

            _suTPos = tt[_origPtr];
            _suCount = 0;
            _suI2 = 0;
            _suCh2 = 256; /* not a valid 8-bit byte value?, and not EOF */

            if (_blockRandomised)
            {
                _suRnToGo = 0;
                _suRtPos = 0;
                SetupRandPartA();
            }
            else
            {
                SetupNoRandPartA();
            }
        }


        private void SetupRandPartA()
        {
            if (_suI2 <= _last)
            {
                _suChPrev = _suCh2;
                var suCh2Shadow = _data.Ll8[_suTPos] & 0xff;
                _suTPos = _data.Tt[_suTPos];
                if (_suRnToGo == 0)
                {
                    _suRnToGo = BZip2.Rand.Rnums(_suRtPos) - 1;
                    if (++_suRtPos == 512)
                    {
                        _suRtPos = 0;
                    }
                }
                else
                {
                    _suRnToGo--;
                }

                _suCh2 = suCh2Shadow ^= (_suRnToGo == 1) ? 1 : 0;
                _suI2++;
                _currentChar = suCh2Shadow;
                _currentState = CState.RandPartB;
                _crc.UpdateCrc((byte) suCh2Shadow);
            }
            else
            {
                EndBlock();
                InitBlock();
                SetupBlock();
            }
        }

        private void SetupNoRandPartA()
        {
            if (_suI2 <= _last)
            {
                _suChPrev = _suCh2;
                var suCh2Shadow = _data.Ll8[_suTPos] & 0xff;
                _suCh2 = suCh2Shadow;
                _suTPos = _data.Tt[_suTPos];
                _suI2++;
                _currentChar = suCh2Shadow;
                _currentState = CState.NoRandPartB;
                _crc.UpdateCrc((byte) suCh2Shadow);
            }
            else
            {
                _currentState = CState.NoRandPartA;
                EndBlock();
                InitBlock();
                SetupBlock();
            }
        }

        private void SetupRandPartB()
        {
            if (_suCh2 != _suChPrev)
            {
                _currentState = CState.RandPartA;
                _suCount = 1;
                SetupRandPartA();
            }
            else if (++_suCount >= 4)
            {
                _suZ = (char) (_data.Ll8[_suTPos] & 0xff);
                _suTPos = _data.Tt[_suTPos];
                if (_suRnToGo == 0)
                {
                    _suRnToGo = BZip2.Rand.Rnums(_suRtPos) - 1;
                    if (++_suRtPos == 512)
                    {
                        _suRtPos = 0;
                    }
                }
                else
                {
                    _suRnToGo--;
                }

                _suJ2 = 0;
                _currentState = CState.RandPartC;
                if (_suRnToGo == 1)
                {
                    _suZ ^= (char) 1;
                }

                SetupRandPartC();
            }
            else
            {
                _currentState = CState.RandPartA;
                SetupRandPartA();
            }
        }

        private void SetupRandPartC()
        {
            if (_suJ2 < _suZ)
            {
                _currentChar = _suCh2;
                _crc.UpdateCrc((byte) _suCh2);
                _suJ2++;
            }
            else
            {
                _currentState = CState.RandPartA;
                _suI2++;
                _suCount = 0;
                SetupRandPartA();
            }
        }

        private void SetupNoRandPartB()
        {
            if (_suCh2 != _suChPrev)
            {
                _suCount = 1;
                SetupNoRandPartA();
            }
            else if (++_suCount >= 4)
            {
                _suZ = (char) (_data.Ll8[_suTPos] & 0xff);
                _suTPos = _data.Tt[_suTPos];
                _suJ2 = 0;
                SetupNoRandPartC();
            }
            else
            {
                SetupNoRandPartA();
            }
        }

        private void SetupNoRandPartC()
        {
            if (_suJ2 < _suZ)
            {
                var suCh2Shadow = _suCh2;
                _currentChar = suCh2Shadow;
                _crc.UpdateCrc((byte) suCh2Shadow);
                _suJ2++;
                _currentState = CState.NoRandPartC;
            }
            else
            {
                _suI2++;
                _suCount = 0;
                SetupNoRandPartA();
            }
        }

        private sealed class DecompressionState
        {
            // (with blockSize 900k)
            public readonly bool[] InUse = new bool[256];
            public readonly byte[] SeqToUnseq = new byte[256]; // 256 byte
            public readonly byte[] Selector = new byte[BZip2.MaxSelectors]; // 18002 byte
            public readonly byte[] SelectorMtf = new byte[BZip2.MaxSelectors]; // 18002 byte

            /**
             * Freq table collected to save a pass over the data during
             * decompression.
             */
            public readonly int[] Unzftab;
            public readonly int[][] GLimit;
            public readonly int[][] GBase;
            public readonly int[][] GPerm;
            public readonly int[] GMinlen;

            public readonly int[] Cftab;
            public readonly byte[] GetAndMoveToFrontDecodeYy;
            public readonly char[][] TempCharArray2d;

            public readonly byte[] RecvDecodingTablesPos;
            // ---------------
            // 60798 byte

            public int[] Tt; // 3600000 byte
            public readonly byte[] Ll8; // 900000 byte

            // ---------------
            // 4560782 byte
            // ===============

            public DecompressionState(int blockSize100K)
            {
                Unzftab = new int[256]; // 1024 byte

                GLimit = BZip2.InitRectangularArray<int>(BZip2.NGroups, BZip2.MaxAlphaSize);
                GBase = BZip2.InitRectangularArray<int>(BZip2.NGroups, BZip2.MaxAlphaSize);
                GPerm = BZip2.InitRectangularArray<int>(BZip2.NGroups, BZip2.MaxAlphaSize);
                GMinlen = new int[BZip2.NGroups]; // 24 byte

                Cftab = new int[257]; // 1028 byte
                GetAndMoveToFrontDecodeYy = new byte[256]; // 512 byte
                TempCharArray2d = BZip2.InitRectangularArray<char>(BZip2.NGroups, BZip2.MaxAlphaSize);
                RecvDecodingTablesPos = new byte[BZip2.NGroups]; // 6 byte

                Ll8 = new byte[blockSize100K * BZip2.BlockSizeMultiple];
            }

            /**
             * Initializes the tt array.
             *
             * This method is called when the required length of the array is known.
             * I don't initialize it at construction time to avoid unneccessary
             * memory allocation when compressing small files.
             */
            public int[] InitTt(int length)
            {
                var ttShadow = Tt;

                // tt.length should always be >= length, but theoretically
                // it can happen, if the compressor mixed small and large
                // blocks. Normally only the last block will be smaller
                // than others.
                if ((ttShadow == null) || (ttShadow.Length < length))
                {
                    Tt = ttShadow = new int[length];
                }

                return ttShadow;
            }
        }
    }

    // /**
    //  * Checks if the signature matches what is expected for a bzip2 file.
    //  *
    //  * @param signature
    //  *            the bytes to check
    //  * @param length
    //  *            the number of bytes to check
    //  * @return true, if this stream is a bzip2 compressed stream, false otherwise
    //  *
    //  * @since Apache Commons Compress 1.1
    //  */
    // public static boolean MatchesSig(byte[] signature)
    // {
    //     if ((signature.Length < 3) ||
    //         (signature[0] != 'B') ||
    //         (signature[1] != 'Z') ||
    //         (signature[2] != 'h'))
    //         return false;
    //
    //     return true;
    // }


    internal static class BZip2
    {
        internal static T[][] InitRectangularArray<T>(int d1, int d2)
        {
            var x = new T[d1][];
            for (var i = 0; i < d1; i++)
            {
                x[i] = new T[d2];
            }

            return x;
        }

        public const int BlockSizeMultiple = 100000;
        public const int MinBlockSize = 1;
        public const int MaxBlockSize = 9;
        public const int MaxAlphaSize = 258;
        public const int MaxCodeLength = 23;
        public const char Runa = (char) 0;
        public const char Runb = (char) 1;
        public const int NGroups = 6;
        public const int GSize = 50;
        public const int NIters = 4;
        public const int MaxSelectors = 2 + 900000 / GSize;

        public static readonly int NumOvershootBytes = 20;

        /*
         * <p> If you are ever unlucky/improbable enough to get a stack
         * overflow whilst sorting, increase the following constant and
         * try again. In practice I have never seen the stack go above 27
         * elems, so the following limit seems very generous.  </p>
         */

        internal static class Rand
        {
            private static readonly int[] ValRnums =
            {
                619, 720, 127, 481, 931, 816, 813, 233, 566, 247,
                985, 724, 205, 454, 863, 491, 741, 242, 949, 214,
                733, 859, 335, 708, 621, 574, 73, 654, 730, 472,
                419, 436, 278, 496, 867, 210, 399, 680, 480, 51,
                878, 465, 811, 169, 869, 675, 611, 697, 867, 561,
                862, 687, 507, 283, 482, 129, 807, 591, 733, 623,
                150, 238, 59, 379, 684, 877, 625, 169, 643, 105,
                170, 607, 520, 932, 727, 476, 693, 425, 174, 647,
                73, 122, 335, 530, 442, 853, 695, 249, 445, 515,
                909, 545, 703, 919, 874, 474, 882, 500, 594, 612,
                641, 801, 220, 162, 819, 984, 589, 513, 495, 799,
                161, 604, 958, 533, 221, 400, 386, 867, 600, 782,
                382, 596, 414, 171, 516, 375, 682, 485, 911, 276,
                98, 553, 163, 354, 666, 933, 424, 341, 533, 870,
                227, 730, 475, 186, 263, 647, 537, 686, 600, 224,
                469, 68, 770, 919, 190, 373, 294, 822, 808, 206,
                184, 943, 795, 384, 383, 461, 404, 758, 839, 887,
                715, 67, 618, 276, 204, 918, 873, 777, 604, 560,
                951, 160, 578, 722, 79, 804, 96, 409, 713, 940,
                652, 934, 970, 447, 318, 353, 859, 672, 112, 785,
                645, 863, 803, 350, 139, 93, 354, 99, 820, 908,
                609, 772, 154, 274, 580, 184, 79, 626, 630, 742,
                653, 282, 762, 623, 680, 81, 927, 626, 789, 125,
                411, 521, 938, 300, 821, 78, 343, 175, 128, 250,
                170, 774, 972, 275, 999, 639, 495, 78, 352, 126,
                857, 956, 358, 619, 580, 124, 737, 594, 701, 612,
                669, 112, 134, 694, 363, 992, 809, 743, 168, 974,
                944, 375, 748, 52, 600, 747, 642, 182, 862, 81,
                344, 805, 988, 739, 511, 655, 814, 334, 249, 515,
                897, 955, 664, 981, 649, 113, 974, 459, 893, 228,
                433, 837, 553, 268, 926, 240, 102, 654, 459, 51,
                686, 754, 806, 760, 493, 403, 415, 394, 687, 700,
                946, 670, 656, 610, 738, 392, 760, 799, 887, 653,
                978, 321, 576, 617, 626, 502, 894, 679, 243, 440,
                680, 879, 194, 572, 640, 724, 926, 56, 204, 700,
                707, 151, 457, 449, 797, 195, 791, 558, 945, 679,
                297, 59, 87, 824, 713, 663, 412, 693, 342, 606,
                134, 108, 571, 364, 631, 212, 174, 643, 304, 329,
                343, 97, 430, 751, 497, 314, 983, 374, 822, 928,
                140, 206, 73, 263, 980, 736, 876, 478, 430, 305,
                170, 514, 364, 692, 829, 82, 855, 953, 676, 246,
                369, 970, 294, 750, 807, 827, 150, 790, 288, 923,
                804, 378, 215, 828, 592, 281, 565, 555, 710, 82,
                896, 831, 547, 261, 524, 462, 293, 465, 502, 56,
                661, 821, 976, 991, 658, 869, 905, 758, 745, 193,
                768, 550, 608, 933, 378, 286, 215, 979, 792, 961,
                61, 688, 793, 644, 986, 403, 106, 366, 905, 644,
                372, 567, 466, 434, 645, 210, 389, 550, 919, 135,
                780, 773, 635, 389, 707, 100, 626, 958, 165, 504,
                920, 176, 193, 713, 857, 265, 203, 50, 668, 108,
                645, 990, 626, 197, 510, 357, 358, 850, 858, 364,
                936, 638
            };


            /// <summary>
            ///   Returns the "random" number at a specific index.
            /// </summary>
            /// <param name='i'>the index</param>
            /// <returns>the random number</returns>
            internal static int Rnums(int i)
            {
                return ValRnums[i];
            }
        }
    }
}