﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CSCore.Codecs.FLAC;

namespace RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC
{
    /// <summary>
    ///     Provides a decoder for decoding flac (Free Lostless Audio Codec) data.
    /// </summary>
    public sealed class FlacFile : IFlacFile
    {
        private readonly Stream _stream;
        private readonly FlacMetadataStreamInfo _streamInfo;
        private readonly FlacPreScan _scan;

        private readonly object _bufferLock = new();
        private readonly bool _closeStream;
        
        private Memory<byte> _overflowBuffer;
        private int _overflowCount;
        private int _overflowOffset;

        private int _frameIndex;

        /// <summary>
        ///     Gets the output <see cref="Lib.WaveFormat" /> of the decoder.
        /// </summary>
        public WaveFormat WaveFormat { get; }

        /// <summary>
        ///     Gets a value which indicates whether the seeking is supported. True means that seeking is supported; False means
        ///     that seeking is not supported.
        /// </summary>
        public bool CanSeek => _scan != null;

        private FlacFrame _frame;

        private FlacFrame Frame => _frame ?? (_frame = FlacFrame.FromStream(_stream, _streamInfo));

        /// <summary>
        ///     Initializes a new instance of the <see cref="FlacFile" /> class.
        /// </summary>
        /// <param name="fileName">Filename which of a flac file which should be decoded.</param>
        public FlacFile(string fileName)
            : this(File.OpenRead(fileName))
        {
            _closeStream = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FlacFile" /> class.
        /// </summary>
        /// <param name="stream">Stream which contains flac data which should be decoded.</param>
        /// <param name="scanFlag">Scan mode which defines how to scan the flac data for frames.</param>
        /// <param name="onscanFinished">
        ///     Callback which gets called when the pre scan processes finished. Should be used if the
        ///     <paramref name="scanFlag" /> argument is set the <see cref="FlacPreScanMode.Async" />.
        /// </param>
        public FlacFile(Stream stream, FlacPreScanMode scanFlag = FlacPreScanMode.Default,
            Action<FlacPreScanFinishedEventArgs> onscanFinished = null)
        {
            if (stream == null)
                throw new ArgumentNullException();
            if (!stream.CanRead)
                throw new ArgumentException("Stream is not readable.", nameof(stream));

            _stream = stream;
            _closeStream = true;

            //skip ID3v2
            SkipId3V2(stream);

            //read metadata
            var metadata = FlacMetadata.ReadAllMetadataFromStream(stream).ToList();

            if (metadata.Count <= 0)
                throw new FlacException("No Metadata found.", FlacLayer.Metadata);

            var streamInfo =
                metadata.First(x => x.MetaDataType == FlacMetaDataType.StreamInfo) as FlacMetadataStreamInfo;

            _streamInfo = streamInfo ?? throw new FlacException("No StreamInfo-Metadata found.", FlacLayer.Metadata);
            WaveFormat = CreateWaveFormat(streamInfo);
            Debug.WriteLine("Flac StreamInfo found -> WaveFormat: " + WaveFormat);
            Debug.WriteLine("Flac-File-Metadata read.");

            //prescan stream
            if (scanFlag != FlacPreScanMode.None)
            {
                var scan = new FlacPreScan(stream);
                scan.ScanFinished += (_, e) =>
                {
                    onscanFinished?.Invoke(e);
                };
                scan.ScanStream(_streamInfo, scanFlag);
                _scan = scan;
            }
        }

        private void SkipId3V2(Stream stream)
        {
            var buffer = new byte[10];
            while (true)
            {
                if (stream.Read(buffer, 0, 4) < 4)
                    throw new EndOfStreamException("Can not read \"fLaC\" sync.");

                if (buffer[0] == 'I' && buffer[1] == 'D' && buffer[2] == '3')
                {
                    if (stream.Read(buffer, 4, 6) < 6)
                        throw new EndOfStreamException("Can not finish reading ID3 tag.");
                    var size = ((buffer[6] & 0x7F) << 21) |
                               ((buffer[7] & 0x7F) << 14) |
                               ((buffer[8] & 0x7F) << 7) |
                               (buffer[9] & 0x7F);
                    stream.Read(new byte[size], 0, size);
                    continue;
                }

                if (buffer[0] == 'f' && buffer[1] == 'L' && buffer[2] == 'a' && buffer[3] == 'C')
                    return;
                
                throw new FlacException("Invalid Flac-File. \"fLaC\" Sync not found.", FlacLayer.OutSideOfFrame);
            }
        }

        private WaveFormat CreateWaveFormat(FlacMetadataStreamInfo streamInfo)
        {
            if (streamInfo.Channels > 2 && streamInfo.Channels <= 8)
            {
                ChannelMask channelMask;
                switch (streamInfo.Channels)
                {
                    case 3:
                        //2.1
                        channelMask = ChannelMask.SpeakerFrontLeft | ChannelMask.SpeakerFrontRight |
                                      ChannelMask.SpeakerFrontCenter;
                        break;
                    case 4:
                        //quadraphonic
                        channelMask = ChannelMask.SpeakerFrontLeft | ChannelMask.SpeakerFrontRight |
                                      ChannelMask.SpeakerBackLeft | ChannelMask.SpeakerBackRight;
                        break;
                    case 5:
                        //5.0
                        channelMask = ChannelMask.SpeakerFrontLeft | ChannelMask.SpeakerFrontRight |
                                      ChannelMask.SpeakerFrontCenter | ChannelMask.SpeakerSideLeft |
                                      ChannelMask.SpeakerSideRight;
                        break;
                    case 6:
                        //5.1
                        channelMask = ChannelMask.SpeakerFrontLeft | ChannelMask.SpeakerFrontRight |
                                      ChannelMask.SpeakerFrontCenter | ChannelMask.SpeakerLowFrequency |
                                      ChannelMask.SpeakerSideLeft | ChannelMask.SpeakerSideRight;
                        break;
                    case 7:
                        //6.1
                        channelMask = ChannelMask.SpeakerFrontLeft | ChannelMask.SpeakerFrontRight |
                                      ChannelMask.SpeakerFrontCenter | ChannelMask.SpeakerLowFrequency |
                                      ChannelMask.SpeakerSideLeft | ChannelMask.SpeakerSideRight |
                                      ChannelMask.SpeakerBackCenter;
                        break;
                    case 8:
                        //7.1
                        channelMask = ChannelMask.SpeakerFrontLeft | ChannelMask.SpeakerFrontRight |
                                      ChannelMask.SpeakerFrontCenter | ChannelMask.SpeakerLowFrequency |
                                      ChannelMask.SpeakerBackLeft | ChannelMask.SpeakerBackRight |
                                      ChannelMask.SpeakerSideLeft | ChannelMask.SpeakerSideRight;
                        break;
                    default:
                        throw new Exception("Invalid number of channels. This error should not occur.");
                }
                return new WaveFormatExtensible(streamInfo.SampleRate, streamInfo.BitsPerSample, streamInfo.Channels,
                    AudioSubTypes.Pcm, channelMask);
            }
            return new WaveFormat(streamInfo.SampleRate, streamInfo.BitsPerSample, streamInfo.Channels, AudioEncoding.Pcm);
        }

        /// <summary>
        ///     Reads a sequence of bytes from the <see cref="FlacFile" /> and advances the position within the stream by the
        ///     number of bytes read.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the <paramref name="buffer" /> contains the specified
        ///     byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> +
        ///     <paramref name="count" /> - 1) replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in the <paramref name="buffer" /> at which to begin storing the data
        ///     read from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to read from the current source.</param>
        /// <returns>The total number of bytes read into the buffer.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            CheckForDisposed();

            var read = 0;
            count -= (count % WaveFormat.BlockAlign);

            lock (_bufferLock)
            {
                read += GetOverflows(buffer, ref offset, count);

                while (read < count)
                {
                    var frame = Frame;
                    if (frame == null)
                        return read;

                    while (!frame.NextFrame())
                    {
                        if (CanSeek) //go to next frame
                        {
                            if (++_frameIndex >= _scan.Frames.Count)
                                return read;
                            _stream.Position = _scan.Frames[_frameIndex].StreamOffset;
                        }
                    }
                    _frameIndex++;

                    var bufferlength = frame.GetBuffer(ref _overflowBuffer);
                    var bytesToCopy = Math.Min(count - read, bufferlength);
                    _overflowBuffer.Span.Slice(0, bytesToCopy).CopyTo(buffer.AsSpan().Slice(offset));
                    read += bytesToCopy;
                    offset += bytesToCopy;

                    _overflowCount = ((bufferlength > bytesToCopy) ? (bufferlength - bytesToCopy) : 0);
                    _overflowOffset = ((bufferlength > bytesToCopy) ? (bytesToCopy) : 0);
                }
            }
            _position += read;

            return read;
        }

        private int GetOverflows(Memory<byte> buffer, ref int offset, int count)
        {
            if (_overflowCount != 0 && count > 0)
            {
                var bytesToCopy = Math.Min(count, _overflowCount);
                _overflowBuffer.Span.Slice(_overflowOffset, bytesToCopy).CopyTo(buffer.Span.Slice(offset));

                _overflowCount -= bytesToCopy;
                _overflowOffset += bytesToCopy;
                offset += bytesToCopy;
                return bytesToCopy;
            }
            return 0;
        }

        private long _position;
        private bool _disposed;

        /// <summary>
        ///     Gets or sets the position of the <see cref="FlacFile" /> in bytes.
        /// </summary>
        public long Position
        {
            get
            {
                if (!CanSeek || _disposed)
                    return 0;

                lock (_bufferLock)
                {
                    return _position;
                }
            }
            set
            {
                CheckForDisposed();

                if (!CanSeek)
                    return;
                lock (_bufferLock)
                {
                    value = Math.Max(Math.Min(value, Length), 0);
                    value -= (value % WaveFormat.BlockAlign);

                    for (var i = 0; i < _scan.Frames.Count; i++)
                    {
                        if ((value / WaveFormat.BlockAlign) <= _scan.Frames[i].SampleOffset)
                        {
                            if (i != 0)
                                i--;

                            _stream.Position = _scan.Frames[i].StreamOffset;
                            _frameIndex = i;
                            if (_stream.Position >= _stream.Length)
                                throw new EndOfStreamException("Stream got EOF.");
                            _position = _scan.Frames[i].SampleOffset * WaveFormat.BlockAlign;
                            _overflowCount = 0;
                            _overflowOffset = 0;

                            var diff = (int) (value - Position);
                            diff -= (diff % WaveFormat.BlockAlign);
                            if (diff > 0)
                            {
                                _stream.Read(new byte[diff], 0, diff);
                            }

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the length of the <see cref="FlacFile" /> in bytes.
        /// </summary>
        public long Length
        {
            get
            {
                if (_disposed)
                    return 0;
                if (CanSeek)
                    return _scan.TotalSamples * WaveFormat.BlockAlign;
                return -1;
            }
        }

        /// <summary>
        ///     Disposes the <see cref="FlacFile" /> instance and disposes the underlying stream.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes the <see cref="FlacFile" /> instance and disposes the underlying stream.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            lock (_bufferLock)
            {
                if (!_disposed)
                {
                    if (_frame != null)
                    {
                        _frame.Dispose();
                        _frame = null;
                    }

                    if (_stream != null && !(!_stream.CanRead && !_stream.CanWrite) && _closeStream)
                        _stream.Dispose();

                    _disposed = true;
                }
            }
        }

        private void CheckForDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <summary>
        ///     Destructor which calls the <see cref="Dispose(bool)" /> method.
        /// </summary>
        ~FlacFile()
        {
            Dispose(false);
        }
    }
}