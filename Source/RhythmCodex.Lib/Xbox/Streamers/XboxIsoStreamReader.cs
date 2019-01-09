using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Iso.Model;
using RhythmCodex.Iso.Streamers;
using RhythmCodex.Xbox.Converters;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers
{
    [Service]
    public class XboxIsoStreamReader : IXboxIsoStreamReader
    {
        private string MediaSectorId = "MICROSOFT*XBOX*MEDIA";
        
        private readonly IXboxIsoInfoDecoder _xboxIsoInfoDecoder;

        public XboxIsoStreamReader(
            IXboxIsoInfoDecoder xboxIsoInfoDecoder)
        {
            _xboxIsoInfoDecoder = xboxIsoInfoDecoder;
        }

        public IEnumerable<XboxIsoFileEntry> Read(Stream stream, long length)
        {
            // read xbox info header
            var reader = new BinaryReader(stream);
            var basePosition = stream.Position;
            stream.Position = basePosition + 0x800 * 0x20;
            var mediaSector = reader.ReadBytes(0x800);
            if (Encodings.CP437.GetString(mediaSector.AsSpan().Slice(0, 20).ToArray()) != MediaSectorId)
                throw new RhythmCodexException("This doesn't appear to be an Xbox ISO.");
            var mediaInfo = _xboxIsoInfoDecoder.Decode(mediaSector);

            // read root directory
            basePosition += 0x800L * mediaInfo.DirectorySectorNumber;
            stream.Position = basePosition;
            return ReadDirectory(reader, "", basePosition).ToList();
        }

        private IEnumerable<XboxIsoFileEntry> ReadDirectory(BinaryReader reader, string path, long basePosition)
        {
            var binaryTableLeft = reader.ReadInt16() * 4;
            var binaryTableRight = reader.ReadInt16() * 4;
            
            var entry = new XboxIsoFileEntry
            {
                StartSector = reader.ReadInt32(),
                FileSize = reader.ReadInt32(),
                Attributes = (XboxIsoFileAttributes) reader.ReadByte()
            };

            var nameLength = reader.ReadByte();
            entry.FileName = $"{path}{Encodings.UTF8.GetString(reader.ReadBytes(nameLength))}";

            if (!entry.Attributes.HasFlag(XboxIsoFileAttributes.Directory))
            {
                yield return entry;
            }
            
            if (binaryTableLeft > 0)
            {
                reader.BaseStream.Position = basePosition + binaryTableLeft;
                foreach (var e in ReadDirectory(reader, path, basePosition))
                    yield return e;
            }
            
            if (binaryTableRight > 0)
            {
                reader.BaseStream.Position = basePosition + binaryTableRight;
                foreach (var e in ReadDirectory(reader, path, basePosition))
                    yield return e;
            }

            if (entry.Attributes.HasFlag(XboxIsoFileAttributes.Directory))
            {
                var newBase = entry.StartSector * 0x800L;
                reader.BaseStream.Position = newBase;
                foreach (var e in ReadDirectory(reader, $"{entry.FileName}/", newBase))
                    yield return e;
            }
        }

        public byte[] Extract(Stream stream, XboxIsoFileEntry entry)
        {
            stream.Position = entry.StartSector * 0x800L;
            var reader = new BinaryReader(stream);
            return reader.ReadBytes(entry.FileSize);
        }
    }

    public interface IXboxIsoStreamReader
    {
        IEnumerable<XboxIsoFileEntry> Read(Stream stream, long length);
        byte[] Extract(Stream stream, XboxIsoFileEntry entry);
    }
}