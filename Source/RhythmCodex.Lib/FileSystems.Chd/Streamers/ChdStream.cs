using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using RhythmCodex.Compressions.Lzma.Converters;
using RhythmCodex.Compressions.Models;
using RhythmCodex.Extensions;
using RhythmCodex.FileSystems.Chd.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.FileSystems.Chd.Streamers;

public class ChdStream : Stream
{
    private readonly ILzmaDecoder _lzmaDecoder;
    private readonly Stream _baseStream;
    private readonly ChdStream? _parent;

    internal ChdStream(ILzmaDecoder lzmaDecoder, Stream baseStream)
    {
        _reader = new BinaryReader(baseStream);

        if (new string(_reader.ReadChars(8)) != "MComprHD")
            throw new RhythmCodexException("Bad CHD header");

        var headerLength = _reader.ReadUInt32S();
        var version = _reader.ReadUInt32S();

        switch (version)
        {
            case 1:
                _header = ReadHeaderV1();
                _map = ReadMapV1();
                _readHunk = ReadHunkV1;
                break;
            case 2:
                _header = ReadHeaderV2();
                _map = ReadMapV1();
                _readHunk = ReadHunkV1;
                break;
            case 3:
                _header = ReadHeaderV3();
                _map = ReadMapV3();
                _readHunk = ReadHunkV3;
                break;
            case 4:
                _header = ReadHeaderV4();
                _map = ReadMapV3();
                _readHunk = ReadHunkV3;
                break;
            case 5:
                _header = ReadHeaderV5();
                _map = ReadMapV5();
                _readHunk = ReadHunkV5;
                break;
            default:
                throw new RhythmCodexException("Unrecognized CHD version");
        }

        _lzmaDecoder = lzmaDecoder;
        _baseStream = baseStream;
    }

    internal ChdStream(ILzmaDecoder lzmaDecoder, Stream baseStream, ChdStream parent)
        : this(lzmaDecoder, baseStream)
    {
        _parent = parent;
    }

    private struct CachedHunk
    {
        public int Index;
        public Memory<byte> Data;
    }

    private const int HunkCacheMaxSize = 256;

    private CachedHunk _currentHunk;
    private readonly List<CachedHunk> _hunkCache = [];
    private readonly BinaryReader _reader;
    private readonly Func<int, Memory<byte>> _readHunk;

    private long _hunkOffset;
    private long _hunkSize;
    private ulong _dataLength;
    private long _position;
    private readonly ChdHeaderInfo _header;
    private readonly List<ChdMapInfo> _map;

    private CachedHunk CacheHunk(int index)
    {
        var hunk = new CachedHunk
        {
            Data = _readHunk(index),
            Index = index
        };

        _hunkCache.Add(hunk);
        if (_hunkCache.Count >= HunkCacheMaxSize)
            _hunkCache.RemoveAt(0);

        return hunk;
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override void Flush()
    {
        // do nothing
    }

    private void GetHunk(int index)
    {
        _hunkOffset = index * _hunkSize;
        var count = _hunkCache.Count;
        for (var i = 0; i < count; i++)
        {
            var hunk = _hunkCache[i];
            if (hunk.Index == index)
            {
                _currentHunk = hunk;
                return;
            }
        }

        _currentHunk = CacheHunk(index);
    }

    public override long Length => (long) _dataLength;

    public override long Position
    {
        get => _position;
        set => _position = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var total = count;

        if (_currentHunk.Data.IsEmpty)
            GetHunk((int) (_position / _hunkSize));

        if (_currentHunk.Data.IsEmpty)
            return 0;

        var hunkPosition = (int)(_position - _hunkOffset);
        while (count > 0)
        {
            if (hunkPosition >= _hunkSize || hunkPosition < 0)
            {
                GetHunk((int) (_position / _hunkSize));
                hunkPosition = (int)(_position - _hunkOffset);
            }

            buffer[offset] = _currentHunk.Data.Span[hunkPosition];
            hunkPosition++;
            offset++;
            _position++;
            count--;
        }

        return total;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Current:
                Position = _position + offset;
                break;
            case SeekOrigin.End:
                Position = _position - offset;
                break;
            default:
                Position = offset;
                break;
        }

        return _position;
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    private Memory<byte> DecompressMini(ulong data, uint decompressedLength)
    {
        var result = new byte[decompressedLength];
        Span<byte> buffer = stackalloc byte[8];

        buffer[0] = (byte) ((data >> 56) & 0xFF);
        buffer[1] = (byte) ((data >> 48) & 0xFF);
        buffer[2] = (byte) ((data >> 40) & 0xFF);
        buffer[3] = (byte) ((data >> 32) & 0xFF);
        buffer[4] = (byte) ((data >> 24) & 0xFF);
        buffer[5] = (byte) ((data >> 16) & 0xFF);
        buffer[6] = (byte) ((data >> 8) & 0xFF);
        buffer[7] = (byte) (data & 0xFF);

        var j = 0;
        for (var i = 0; i < decompressedLength; i++)
        {
            result[i] = buffer[j];
            j++;
            if (j == 8)
                j = 0;
        }

        return result;
    }

    private Memory<byte> DecompressParentHunk(ulong offs)
    {
        return _parent?._readHunk((int) (offs & 0x7FFFFFFFul)) ?? Memory<byte>.Empty;
    }

    private Memory<byte> DecompressSelfHunk(ulong offs)
    {
        return _readHunk((int) (offs & 0x7FFFFFFFul));
    }

    private Memory<byte> DecompressZlib(uint decompressedLength)
    {
        var buffer = new byte[decompressedLength];
        using var ds = new DeflateStream(_baseStream, CompressionMode.Decompress, true);
        ds.ReadAtLeast(buffer, (int)decompressedLength);

        return buffer;
    }

    private Memory<byte> DecompressLzma(uint compressedLength, uint decompressedLength)
    {
        var buffer = new byte[decompressedLength];
        var lc = 3;
        var lp = 0;
        var pb = 2;
        var dictSize = 1 << 26;

        var decoderProperties = new[]
        {
            (byte) (lc + lp * 9 + pb * 45),
            unchecked((byte) dictSize),
            unchecked((byte) (dictSize >> 8)),
            unchecked((byte) (dictSize >> 16)),
            unchecked((byte) (dictSize >> 24))
        };

        return _lzmaDecoder.Decode(_baseStream, (int) compressedLength, (int) decompressedLength, decoderProperties);
    }

    private Memory<byte> DecompressFlac()
    {
        var endian = _baseStream.ReadByte();

        Action<byte[]> postProcess = endian switch
        {
            0x42 => // B
                BigEndianPostProcess,
            0x4C => // L
                LittleEndianPostProcess,
            _ => throw new Exception($"Unknown FLAC endian type {endian:X2}")
        };

        // determine FLAC block size, which must be 16-65535
        // clamp to 2k since that's supposed to be the sweet spot
        var flacBlockSize = _header.hunkBytes / 4;
        while (flacBlockSize > 2048)
            flacBlockSize /= 2;

        // TODO: this (the old FLAC decoder was removed)
        return Array.Empty<byte>();

        void LittleEndianPostProcess(byte[] _)
        {
        }

        void BigEndianPostProcess(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i += 2)
                (buffer[i], buffer[i + 1]) = (buffer[i + 1], buffer[i]);
        }
    }

    private Memory<byte> DecompressCustom(ChdMapInfo map)
    {
        var id = _header.compressors[map.compression];
        switch (id)
        {
            case 0x7A6C6962: //zlib
                _baseStream.Position = (long) map.offset;
                return DecompressZlib(_header.hunkBytes);
            case 0x6C7A6D61: //lzma
                _baseStream.Position = (long) map.offset;
                return DecompressLzma((uint) map.length, _header.hunkBytes);
            case 0x666C6163: //flac
                _baseStream.Position = (long) map.offset;
                return DecompressFlac();
            case 0x68756666: //huff
                throw new NotImplementedException();
            default:
                throw new RhythmCodexException($"Unknown compression type {id:X8}");
        }
    }

    private ChdHeaderInfo ReadHeaderV1()
    {
        var header = new ChdHeaderInfo
        {
            flags = _reader.ReadUInt32S(),
            compression = _reader.ReadUInt32S(),
            hunkSize = _reader.ReadUInt32S(),
            totalHunks = _reader.ReadUInt32S(),
            cylinders = _reader.ReadUInt32S(),
            heads = _reader.ReadUInt32S(),
            sectors = _reader.ReadUInt32S(),
            md5 = _reader.ReadMD5S(),
            parentmd5 = _reader.ReadMD5S(),
            seclen = 512
        };

        _dataLength = header.totalHunks * header.hunkSize * 512;
        _hunkSize = header.hunkSize * 512;
        return header;
    }

    private ChdHeaderInfo ReadHeaderV2()
    {
        var header = new ChdHeaderInfo
        {
            flags = _reader.ReadUInt32S(),
            compression = _reader.ReadUInt32S(),
            hunkSize = _reader.ReadUInt32S(),
            totalHunks = _reader.ReadUInt32S(),
            cylinders = _reader.ReadUInt32S(),
            heads = _reader.ReadUInt32S(),
            sectors = _reader.ReadUInt32S(),
            md5 = _reader.ReadMD5S(),
            parentmd5 = _reader.ReadMD5S(),
            seclen = _reader.ReadUInt32S()
        };

        _dataLength = header.totalHunks * header.hunkSize * header.seclen;
        _hunkSize = header.hunkSize * header.seclen;
        return header;
    }

    private ChdHeaderInfo ReadHeaderV3()
    {
        var header = new ChdHeaderInfo
        {
            flags = _reader.ReadUInt32S(),
            compression = _reader.ReadUInt32S(),
            totalHunks = _reader.ReadUInt32S(),
            logicalBytes = _reader.ReadUInt64S(),
            metaOffset = _reader.ReadUInt64S(),
            md5 = _reader.ReadMD5S(),
            parentmd5 = _reader.ReadMD5S(),
            hunkBytes = _reader.ReadUInt32S(),
            sha1 = _reader.ReadSHA1S(),
            parentsha1 = _reader.ReadSHA1S()
        };

        _dataLength = header.logicalBytes;
        _hunkSize = header.hunkBytes;
        return header;
    }

    private ChdHeaderInfo ReadHeaderV4()
    {
        var header = new ChdHeaderInfo
        {
            flags = _reader.ReadUInt32S(),
            compression = _reader.ReadUInt32S(),
            totalHunks = _reader.ReadUInt32S(),
            logicalBytes = _reader.ReadUInt64S(),
            metaOffset = _reader.ReadUInt64S(),
            hunkBytes = _reader.ReadUInt32S(),
            sha1 = _reader.ReadSHA1S(),
            parentsha1 = _reader.ReadSHA1S(),
            rawsha1 = _reader.ReadSHA1S()
        };

        _dataLength = header.logicalBytes;
        _hunkSize = header.hunkBytes;
        return header;
    }

    private ChdHeaderInfo ReadHeaderV5()
    {
        var header = new ChdHeaderInfo
        {
            compressors =
            [
                _reader.ReadUInt32S(),
                    _reader.ReadUInt32S(),
                    _reader.ReadUInt32S(),
                    _reader.ReadUInt32S()
            ],
            logicalBytes = _reader.ReadUInt64S(),
            mapOffset = _reader.ReadUInt64S(),
            metaOffset = _reader.ReadUInt64S(),
            hunkBytes = _reader.ReadUInt32S(),
            unitBytes = _reader.ReadUInt32S(),
            rawsha1 = _reader.ReadSHA1S(),
            sha1 = _reader.ReadSHA1S(),
            parentsha1 = _reader.ReadSHA1S()
        };
        _dataLength = header.logicalBytes;
        _hunkSize = header.hunkBytes;
        header.totalHunks = (uint) ((header.logicalBytes + header.hunkBytes - 1) / header.hunkBytes);
        return header;
    }

    private Memory<byte> ReadHunkV1(int index)
    {
        var entry = _map[index];
        Memory<byte> result;

        _baseStream.Position = (long) entry.offset;
        if (entry.length == _header.hunkSize)
        {
            result = new byte[_header.hunkSize];
            _baseStream.ReadAtLeast(result.Span, (int)(_header.hunkSize * _header.seclen));
        }
        else
        {
            result = DecompressZlib(_header.hunkSize * _header.seclen);
        }

        return result;
    }

    private Memory<byte> ReadHunkV3(int index)
    {
        var entry = _map[index];
        Memory<byte> result;

        switch (entry.flags & 0xF)
        {
            case 0x1:
                _baseStream.Position = (long) entry.offset;
                result = DecompressZlib(_header.hunkBytes);
                break;
            case 0x2:
                _baseStream.Position = (long) entry.offset;
                result = new byte[_header.hunkBytes];
                _baseStream.ReadExactly(result.Span);
                break;
            case 0x3:
                result = DecompressMini(entry.offset, _header.hunkBytes);
                break;
            case 0x4:
                result = DecompressSelfHunk(entry.offset);
                break;
            case 0x5:
                result = DecompressParentHunk(entry.offset);
                break;
            case 0x6:
                throw new Exception("Unsupported V3 hunk type.");
            default:
                throw new Exception("Invalid V3 hunk type.");
        }

        return result;
    }

    private Memory<byte> ReadHunkV5(int index)
    {
        var entry = _map[index];

        switch ((V5CompressionType) entry.compression)
        {
            case V5CompressionType.COMPRESSION_TYPE_0:
            case V5CompressionType.COMPRESSION_TYPE_1:
            case V5CompressionType.COMPRESSION_TYPE_2:
            case V5CompressionType.COMPRESSION_TYPE_3:
                return DecompressCustom(entry);

            case V5CompressionType.COMPRESSION_NONE:
                _baseStream.Position = (long) entry.offset;
                var result = new byte[_header.hunkBytes];
                _baseStream.Read(result, 0, (int) _header.hunkBytes);
                return result;

            case V5CompressionType.COMPRESSION_SELF:
                return DecompressSelfHunk(entry.offset);

            case V5CompressionType.COMPRESSION_PARENT:
                return DecompressParentHunk(entry.offset);

            default:
                throw new Exception("Invalid V5 hunk type.");
        }
    }

    private List<ChdMapInfo> ReadMapV1()
    {
        var map = new List<ChdMapInfo>();
        for (uint i = 0; i < _header.totalHunks; i++)
        {
            var entry = new ChdMapInfo();
            var raw = _reader.ReadUInt64S();
            entry.offset = (raw >> 20) & 0xFFFFFFFFFFFul;
            entry.length = raw & 0xFFFFFul;
            _map.Add(entry);
        }

        return map;
    }

    private List<ChdMapInfo> ReadMapV3()
    {
        var map = new List<ChdMapInfo>();
        for (uint i = 0; i < _header.totalHunks; i++)
        {
            var entry = new ChdMapInfo
            {
                offset = _reader.ReadUInt64S(),
                crc32 = _reader.ReadUInt32S(),
                length = _reader.ReadUInt16S()
            };
            entry.length |= (ulong) _reader.ReadByte() << 16;
            entry.flags = _reader.ReadByte();
            _map.Add(entry);
        }

        return map;
    }

    private List<ChdMapInfo> ReadMapV5()
    {
        var map = new List<ChdMapInfo>();
        _reader.BaseStream.Position = (long) _header.mapOffset;

        var compressed = _header.compressors[0] != 0;

        if (compressed)
        {
            // compressed map header
            var mapbytes = _reader.ReadUInt32S();
            var firstoffs = _reader.ReadUValueS(6);
            var mapcrc = _reader.ReadUInt16S();
            var lengthbits = _reader.ReadByte();
            var selfbits = _reader.ReadByte();
            var parentbits = _reader.ReadByte();
            _reader.ReadByte(); // reserved

            // decompress the map
            var bitbuf = new BitReader(_reader.BaseStream);
            var decoder = new Huffman(16, 8, null, null, null);
            decoder.ImportTreeRle(bitbuf);
            byte lastcomp = 0;
            var repcount = 0;

            for (var hunknum = 0; hunknum < _header.totalHunks; hunknum++)
            {
                var rawmap = new ChdMapInfo();
                if (repcount > 0)
                {
                    rawmap.compression = lastcomp;
                    repcount--;
                }
                else
                {
                    var val = decoder.DecodeOne(bitbuf);
                    if (val == (int) V5CompressionType.COMPRESSION_RLE_SMALL)
                    {
                        rawmap.compression = lastcomp;
                        repcount = 2 + decoder.DecodeOne(bitbuf);
                    }
                    else if (val == (int) V5CompressionType.COMPRESSION_RLE_LARGE)
                    {
                        rawmap.compression = lastcomp;
                        repcount = 2 + 16 + (decoder.DecodeOne(bitbuf) << 4);
                        repcount += decoder.DecodeOne(bitbuf);
                    }
                    else
                    {
                        rawmap.compression = lastcomp = unchecked((byte) val);
                    }
                }

                map.Add(rawmap);
            }

            var curoffset = firstoffs;
            var last_self = 0UL;
            var last_parent = 0UL;

            for (var hunknum = 0; hunknum < _header.totalHunks; hunknum++)
            {
                var rawmap = map[hunknum];
                var offset = curoffset;
                uint length = 0;
                ushort crc = 0;
                switch ((V5CompressionType) rawmap.compression)
                {
                    // base types
                    case V5CompressionType.COMPRESSION_TYPE_0:
                    case V5CompressionType.COMPRESSION_TYPE_1:
                    case V5CompressionType.COMPRESSION_TYPE_2:
                    case V5CompressionType.COMPRESSION_TYPE_3:
                        curoffset += length = (uint) bitbuf.Read(lengthbits);
                        crc = (ushort) bitbuf.Read(16);
                        break;

                    case V5CompressionType.COMPRESSION_NONE:
                        curoffset += length = _header.hunkBytes;
                        crc = (ushort) bitbuf.Read(16);
                        break;

                    case V5CompressionType.COMPRESSION_SELF:
                        offset = (ulong) bitbuf.Read(selfbits);
                        last_self = (uint) offset;
                        break;

                    case V5CompressionType.COMPRESSION_PARENT:
                        offset = (ulong) bitbuf.Read(parentbits);
                        last_parent = offset;
                        break;

                    // pseudo-types; convert into base types
                    case V5CompressionType.COMPRESSION_SELF_1:
                        last_self++;
                        rawmap.compression = (uint) V5CompressionType.COMPRESSION_SELF;
                        offset = last_self;
                        break;
                    case V5CompressionType.COMPRESSION_SELF_0:
                        rawmap.compression = (uint) V5CompressionType.COMPRESSION_SELF;
                        offset = last_self;
                        break;

                    case V5CompressionType.COMPRESSION_PARENT_SELF:
                        rawmap.compression = (uint) V5CompressionType.COMPRESSION_PARENT;
                        last_parent = offset = unchecked((ulong) (hunknum * _header.hunkBytes / _header.unitBytes));
                        break;

                    case V5CompressionType.COMPRESSION_PARENT_1:
                        last_parent += _header.hunkBytes / _header.unitBytes;
                        rawmap.compression = (uint) V5CompressionType.COMPRESSION_PARENT;
                        offset = last_parent;
                        break;
                    case V5CompressionType.COMPRESSION_PARENT_0:
                        rawmap.compression = (uint) V5CompressionType.COMPRESSION_PARENT;
                        offset = last_parent;
                        break;
                }

                rawmap.length = length;
                rawmap.offset = offset;
                rawmap.crc32 = crc;
                map[hunknum] = rawmap;
            }
        }

        return map;
    }
}