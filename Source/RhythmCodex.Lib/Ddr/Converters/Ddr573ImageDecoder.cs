using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters;

[Service]
public class Ddr573ImageDecoder : IDdr573ImageDecoder
{
    private readonly IDdr573ImageDirectoryDecoder _ddr573ImageDirectoryDecoder;
    private readonly IBemaniLzDecoder _bemaniLzDecoder;
    private readonly ILogger _logger;
    private readonly IDdr573DatabaseDecrypter _ddr573DatabaseDecrypter;

    public Ddr573ImageDecoder(
        IDdr573ImageDirectoryDecoder ddr573ImageDirectoryDecoder, 
        IBemaniLzDecoder bemaniLzDecoder, 
        ILogger logger,
        IDdr573DatabaseDecrypter ddr573DatabaseDecrypter)
    {
        _ddr573ImageDirectoryDecoder = ddr573ImageDirectoryDecoder;
        _bemaniLzDecoder = bemaniLzDecoder;
        _logger = logger;
        _ddr573DatabaseDecrypter = ddr573DatabaseDecrypter;
    }
        
    public IList<Ddr573File> Decode(Ddr573Image image, string dbKey)
    {
        return DecodeInternal(image, dbKey).ToList();
    }

    private IEnumerable<Ddr573File> DecodeInternal(Ddr573Image image, string dbKey)
    {
        if (!image.Modules.Any())
            throw new RhythmCodexException("There must be at least one module in the image.");
        if (!image.Modules.ContainsKey(0))
            throw new RhythmCodexException("A module with index 0 must be present in the image.");
            
        var readers = image.Modules
            .ToDictionary(kv => kv.Key, kv => new BinaryReader(new ReadOnlyMemoryStream(kv.Value)));
            
        try
        {
            foreach (var entry in _ddr573ImageDirectoryDecoder.Decode(image))
            {
                if (!readers.ContainsKey(entry.Module))
                    throw new RhythmCodexException($"Module {entry.Module} was requested, but not found.");

                var canDecompress = true;
                var moduleReader = readers[entry.Module];
                moduleReader.BaseStream.Position = entry.Offset;
                Memory<byte> data = moduleReader.ReadBytes(entry.Length);

                switch (entry.EncryptionType)
                {
                    case 1:
                    {
                        if (dbKey != null)
                            data = _ddr573DatabaseDecrypter.Decrypt(data.Span, _ddr573DatabaseDecrypter.ConvertKey(dbKey));
                        else
                            canDecompress = false;
                        break;
                    }
                }

                if (canDecompress)
                {
                    switch (entry.CompressionType)
                    {
                        case 1:
                        {
                            using var compressedStream = new ReadOnlyMemoryStream(data);
                            try
                            {
                                data = _bemaniLzDecoder.Decode(compressedStream);
                            }
                            catch (Exception)
                            {
                                _logger.Warning($"Entry Id={entry.Id:X8} Module={entry.Module:X4} Offset={entry.Offset:X7} could not be decompressed. It will be extracted as-is.");
                            }

                            break;
                        }
                    }
                }
                    
                yield return new Ddr573File
                {
                    Data = data,
                    Id = entry.Id,
                    Module = entry.Module,
                    Offset = entry.Offset,
                    EncryptionType = entry.EncryptionType,
                    Reserved1 = entry.Reserved1,
                    Reserved2 = entry.Reserved2
                };
            }
        }
        finally
        {
            foreach (var reader in readers)
                reader.Value?.Dispose();
        }
            
    }
}