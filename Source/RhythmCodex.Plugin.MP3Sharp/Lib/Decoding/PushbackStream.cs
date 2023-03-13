// /***************************************************************************
//  * PushbackStream.cs
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
using System.IO;

namespace RhythmCodex.Plugin.MP3Sharp.Lib.Decoding;

/// <summary>
/// A PushbackStream is a stream that can "push back" or "unread" data. This is useful in situations where it is convenient for a
/// fragment of code to read an indefinite number of data bytes that are delimited by a particular byte value; after reading the
/// terminating byte, the code fragment can "unread" it, so that the next read operation on the input stream will reread the byte
/// that was pushed back.
/// </summary>
internal sealed class PushbackStream
{
    private readonly int m_BackBufferSize;
    private readonly CircularByteBuffer m_CircularByteBuffer;
    private readonly Stream m_Stream;
    private readonly byte[] m_TemporaryBuffer;
    private int m_NumForwardBytesInBuffer;

    public PushbackStream(Stream s, int backBufferSize)
    {
        m_Stream = s;
        m_BackBufferSize = backBufferSize;
        m_TemporaryBuffer = new byte[m_BackBufferSize];
        m_CircularByteBuffer = new CircularByteBuffer(m_BackBufferSize);
    }

    public int Read(sbyte[] toRead, int offset, int length)
    {
        // Read 
        var currentByte = 0;
        var canReadStream = true;
        while (currentByte < length && canReadStream)
        {
            if (m_NumForwardBytesInBuffer > 0)
            {
                // from mem
                m_NumForwardBytesInBuffer--;
                toRead[offset + currentByte] = (sbyte) m_CircularByteBuffer[m_NumForwardBytesInBuffer];
                currentByte++;
            }
            else
            {
                // from stream
                var newBytes = length - currentByte;
                var numRead = m_Stream.Read(m_TemporaryBuffer, 0, newBytes);
                canReadStream = numRead >= newBytes;
                for (var i = 0; i < numRead; i++)
                {
                    m_CircularByteBuffer.Push(m_TemporaryBuffer[i]);
                    toRead[offset + currentByte + i] = (sbyte) m_TemporaryBuffer[i];
                }
                currentByte += numRead;
            }
        }
        return currentByte;
    }

    public void UnRead(int length)
    {
        m_NumForwardBytesInBuffer += length;
        if (m_NumForwardBytesInBuffer > m_BackBufferSize)
        {
            throw new Exception("The backstream cannot unread the requested number of bytes.");
        }
    }

    public void Close()
    {
        m_Stream.Close();
    }
}