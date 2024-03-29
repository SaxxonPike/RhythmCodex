﻿// /***************************************************************************
//  * CircularByteBuffer.cs
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

using System;

namespace RhythmCodex.Plugin.MP3Sharp.Lib.Decoding
{
    [Serializable]
    internal sealed class CircularByteBuffer
    {
        private byte[] m_DataArray;
        private int m_Index;
        private int m_Length;
        private int m_NumValid;

        public CircularByteBuffer(int size)
        {
            m_DataArray = new byte[size];
            m_Length = size;
        }

        /// <summary>
        ///     Initialize by copying the CircularByteBuffer passed in
        /// </summary>
        public CircularByteBuffer(CircularByteBuffer cdb)
        {
            lock (cdb)
            {
                m_Length = cdb.m_Length;
                m_NumValid = cdb.m_NumValid;
                m_Index = cdb.m_Index;
                m_DataArray = new byte[m_Length];
                for (var c = 0; c < m_Length; c++)
                {
                    m_DataArray[c] = cdb.m_DataArray[c];
                }
            }
        }

        /// <summary>
        ///     The physical size of the Buffer (read/write)
        /// </summary>
        public int BufferSize
        {
            get => m_Length;
            set
            {
                var newDataArray = new byte[value];

                var minLength = (m_Length > value) ? value : m_Length;
                for (var i = 0; i < minLength; i++)
                {
                    newDataArray[i] = InternalGet(i - m_Length + 1);
                }
                m_DataArray = newDataArray;
                m_Index = minLength - 1;
                m_Length = value;
            }
        }

        /// <summary>
        ///     e.g. Offset[0] is the current value
        /// </summary>
        public byte this[int index]
        {
            get => InternalGet(-1 - index);
            set => InternalSet(-1 - index, value);
        }

        /// <summary>
        ///     How far back it is safe to look (read/write).  Write only to reduce NumValid.
        /// </summary>
        public int NumValid
        {
            get => m_NumValid;
            set
            {
                if (value > m_NumValid)
                    throw new Exception("Can't set NumValid to " + value +
                                        " which is greater than the current numValid value of " + m_NumValid);
                m_NumValid = value;
            }
        }

        public CircularByteBuffer Copy()
        {
            return new CircularByteBuffer(this);
        }

        public void Reset()
        {
            m_Index = 0;
            m_NumValid = 0;
        }

        /// <summary>
        ///     Push a byte into the buffer.  Returns the value of whatever comes off.
        /// </summary>
        public byte Push(byte newValue)
        {
            byte ret;
            lock (this)
            {
                ret = InternalGet(m_Length);
                m_DataArray[m_Index] = newValue;
                m_NumValid++;
                if (m_NumValid > m_Length) m_NumValid = m_Length;
                m_Index++;
                m_Index %= m_Length;
            }
            return ret;
        }

        /// <summary>
        ///     Pop an integer off the start of the buffer. Throws an exception if the buffer is empty (NumValid == 0)
        /// </summary>
        public byte Pop()
        {
            lock (this)
            {
                if (m_NumValid == 0) throw new Exception("Can't pop off an empty CircularByteBuffer");
                    m_NumValid--;
                return this[m_NumValid];
            }
        }

        /// <summary>
        ///     Returns what would fall out of the buffer on a Push.  NOT the same as what you'd get with a Pop().
        /// </summary>
        public byte Peek()
        {
            lock (this)
            {
                return InternalGet(m_Length);
            }
        }

        private byte InternalGet(int offset)
        {
            var ind = m_Index + offset;

            // Do thin modulo (should just drop through)
            for (; ind >= m_Length; ind -= m_Length)
                ;
            for (; ind < 0; ind += m_Length)
                ;
            // Set value
            return m_DataArray[ind];
        }

        private void InternalSet(int offset, byte valueToSet)
        {
            var ind = m_Index + offset;

            // Do thin modulo (should just drop through)
            for (; ind > m_Length; ind -= m_Length)
                ;
            for (; ind < 0; ind += m_Length)
                ;
            // Set value
            m_DataArray[ind] = valueToSet;
        }

        /// <summary>
        ///     Returns a range (in terms of Offsets) in an int array in chronological (oldest-to-newest) order. e.g. (3, 0)
        ///     returns the last four ints pushed, with result[3] being the most recent.
        /// </summary>
        public byte[] GetRange(int str, int stp)
        {
            var outByte = new byte[str - stp + 1];

            for (int i = str, j = 0; i >= stp; i--,j++)
            {
                outByte[j] = this[i];
            }

            return outByte;
        }

        public override string ToString()
        {
            var ret = "";
            for (var i = 0; i < m_DataArray.Length; i++)
            {
                ret += m_DataArray[i] + " ";
            }
            ret += "\n index = " + m_Index + " numValid = " + NumValid;
            return ret;
        }
    }
}
