using System;
using System.IO;
using System.Linq;
using RhythmCodex.Streamers;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Streamers
{
    public class VagStream : PassthroughStream
    {
        private readonly VagConfig _vagCodecConfiguration;

        private static readonly int[][] DefaultCoefficients =
        {
            new[] {0, 0},
            new[] {60, 0},
            new[] {115, -52},
            new[] {98, -55},
            new[] {122, -60}
        };

        private readonly int _interleave;
        private readonly int _channels;
        private int _outputBufferPosition;
        private readonly int _outputBufferSize;
        private readonly byte[] _outputBuffer;
        private readonly byte[] _inputBuffer;
        private readonly int[] _lastSample1;
        private readonly int[] _lastSample0;
        private bool _ended;
        private bool _started;
        private readonly int[][] _coefficients;
        private long _subStreamBytesRemaining;

        /// <summary>
        /// Create a decoder stream.
        /// </summary>
        public VagStream(Stream baseStream, VagConfig vagCodecConfiguration) : base(baseStream)
        {
            _vagCodecConfiguration = vagCodecConfiguration;

            _channels = _vagCodecConfiguration.Channels;
            if (_channels < 1)
                _channels = 1;

            _outputBufferPosition = 0;

            _interleave = (vagCodecConfiguration.Interleave >> 4) << 4;
            if (_interleave < 16)
                _interleave = 16;

            _outputBufferSize = _channels * _interleave * 56 / 16;
            _outputBuffer = new byte[_outputBufferSize];
            _inputBuffer = new byte[_channels * _interleave];
            _lastSample0 = new int[_channels];
            _lastSample1 = new int[_channels];

            _ended = false;
            _started = !vagCodecConfiguration.DoNotDecodeUntilStartMarker;

            _coefficients = vagCodecConfiguration.Coefficients ?? DefaultCoefficients;
            _subStreamBytesRemaining = vagCodecConfiguration.MaximumLength;
        }

        /// <summary>
        /// Decode and deinterleave blocks for all channels.
        /// </summary>
        private void DecodeInputBufferToOutputBuffer()
        {
            var inputDecodeOffset = 0;
            var priorEnded = _ended;
            var priorStarted = _started;
            var resultEnded = _ended;
            var resultStarted = _started;

            for (var channel = 0; channel < _channels; channel++)
            {
                var ended = priorEnded;
                var started = priorStarted;

                var interleaveRemaining = _interleave;
                var last1 = _lastSample1[channel];
                var last0 = _lastSample0[channel];
                var delta = new int[2];
                var outputDecodeOffset = channel << 1;

                while (interleaveRemaining > 0)
                {
                    var fm = _inputBuffer[inputDecodeOffset++];
                    var filter = fm >> 4;
                    var magnitude = fm & 0xF;
                    var flags = _inputBuffer[inputDecodeOffset++];

                    if (_vagCodecConfiguration.StopDecodingOnBlankLine &&
                        _inputBuffer.Skip(inputDecodeOffset).Take(16)
                            .All(i => i == 0x00))
                    {
                        ended = true;
                    }

                    if (_vagCodecConfiguration.StopDecodingOnEndMarker &&
                        (flags & 0x1) == 0x1)
                    {
                        ended = true;
                    }

                    if (_vagCodecConfiguration.DoNotDecodeUntilStartMarker &&
                        (flags & 0x5) == 0x4)
                    {
                        started = true;
                    }

                    if (magnitude > 12 || filter > 4)
                    {
                        magnitude = 12;
                        filter = 0;
                    }

                    for (var i = 0; i < 14; i++)
                    {
                        var deltas = _inputBuffer[inputDecodeOffset++];
                        delta[0] = (deltas & 0x0F) << 28;
                        delta[1] = (deltas & 0xF0) << 24;

                        for (var j = 0; j < 2; j++)
                        {
                            var filter0 = _coefficients[filter][0] * last0;
                            var filter1 = _coefficients[filter][1] * last1;
                            var sample = (delta[j] >> (magnitude + 16)) + ((filter0 + filter1) >> 6);
                            if (sample > short.MaxValue)
                                sample = short.MaxValue;
                            else if (sample < short.MinValue)
                                sample = short.MinValue;
                            last1 = last0;
                            last0 = sample;

                            if (!started || ended)
                            {
                                // silence output
                                _outputBuffer[outputDecodeOffset++] = 0;
                                _outputBuffer[outputDecodeOffset++] = 0;
                            }
                            else
                            {
                                _outputBuffer[outputDecodeOffset++] = unchecked((byte) sample);
                                _outputBuffer[outputDecodeOffset++] = unchecked((byte) (sample >> 8));
                            }

                            if (_channels > 1)
                                outputDecodeOffset += (_channels - 1) << 1;
                        }
                    }

                    interleaveRemaining -= 16;
                }

                _lastSample1[channel] = last1;
                _lastSample0[channel] = last0;

                resultEnded |= ended;
                resultStarted |= started;
            }

            _ended = resultEnded;
            _started = resultStarted;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var outputBytesRead = 0;
            while (count > 0)
            {
                if (_outputBufferPosition == 0 || _outputBufferPosition == _outputBufferSize)
                {
                    if (_ended || _subStreamBytesRemaining <= 0)
                        break;

                    _outputBufferPosition = 0;

                    var bytesToRead = Math.Min(_inputBuffer.Length, _subStreamBytesRemaining);
                    if (bytesToRead < _inputBuffer.Length)
                        for (var i = bytesToRead; i < _inputBuffer.Length; i++)
                            _inputBuffer[i] = 0x00;

                    var readOffset = 0;
                    var retries = 2;

                    while (bytesToRead > 0 && _subStreamBytesRemaining > 0)
                    {
                        var bytesRead = base.Read(_inputBuffer, readOffset, (int) bytesToRead);
                        bytesToRead -= bytesRead;
                        readOffset += bytesRead;
                        _subStreamBytesRemaining -= bytesRead;
                        if (bytesRead == 0)
                        {
                            if (--retries <= 0)
                            {
                                _ended = true;
                                for (var i = readOffset; i < _inputBuffer.Length; i++)
                                    _inputBuffer[i] = 0x00;
                                break;
                            }
                        }
                        else
                        {
                            retries = 2;
                        }
                    }

                    DecodeInputBufferToOutputBuffer();
                }

                if (_started)
                {
                    buffer[offset++] = _outputBuffer[_outputBufferPosition++];
                    count--;
                    outputBytesRead++;
                }
                else
                {
                    _outputBufferPosition++;
                    count--;
                }
            }

            return outputBytesRead;
        }
    }
}