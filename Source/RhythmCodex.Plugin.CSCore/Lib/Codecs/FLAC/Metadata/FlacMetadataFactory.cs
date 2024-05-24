using System;
using System.Collections.Generic;
using System.IO;
using CSCore.Codecs.FLAC;
using RhythmCodex.Plugin.CSCore.Lib.Utils;

namespace RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC.Metadata;

/// <summary>
/// Flac metadata factory.
/// </summary>
public sealed class FlacMetadataFactory
{
    private readonly Dictionary<int, Type> _registeredmetadataTypes = new();

    /// <summary>
    /// Gets the default factory instance.
    /// </summary>
    public static FlacMetadataFactory Instance { get; } = new();

    private FlacMetadataFactory()
    {
        RegistermetadataType<FlacMetadataStreamInfo>(FlacMetaDataType.StreamInfo);
        RegistermetadataType<FlacMetadataSeekTable>(FlacMetaDataType.Seektable);
    }

    /// <summary>
    /// Registers a new <see cref="FlacMetaDataType"/>.
    /// </summary>
    /// <param name="metadataType">The <see cref="FlacMetaDataType"/>.</param>
    /// <typeparam name="T">The <see cref="FlacMetadata"/> object assigned to the <paramref name="metadataType"/>.</typeparam>
    public void RegistermetadataType<T>(FlacMetaDataType metadataType) where T : FlacMetadata
    {
        RegistermetadataType<T>((int) metadataType);
    }

    /// <summary>
    /// Registers a new <see cref="FlacMetaDataType"/>.
    /// </summary>
    /// <param name="metadataType">The metadata type as an integer.</param>
    /// <typeparam name="T">The <see cref="FlacMetadata"/> object assigned to the <paramref name="metadataType"/>.</typeparam>
    public void RegistermetadataType<T>(int metadataType) where T : FlacMetadata
    {
        if (_registeredmetadataTypes.ContainsKey(metadataType))
        {
            _registeredmetadataTypes.Remove(metadataType);
        }

        _registeredmetadataTypes.Add(metadataType, typeof(T));
    }

    /// <summary>
    /// Reads and returns a single <see cref="FlacMetadata"/> from the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream which contains the <see cref="FlacMetadata"/>.</param>
    /// <returns>Returns the read <see cref="FlacMetadata"/>.</returns>
    /// <exception cref="FlacException">Could not read metadata.</exception>
    public unsafe FlacMetadata ParseMetadata(Stream stream)
    {
        bool isLastBlock;
        FlacMetaDataType type;
        int length;

        var b = new byte[4];
        if (stream.Read(b, 0, 4) <= 0)
            throw new FlacException(new EndOfStreamException("Could not read metadata."), FlacLayer.Metadata);

        fixed (byte* headerBytes = b)
        {
            var bitReader = new FlacBitReader(headerBytes, 0);

            isLastBlock = bitReader.ReadBits(1) == 1;
            type = (FlacMetaDataType)bitReader.ReadBits(7);
            length = (int)bitReader.ReadBits(24);
        }

        var streamStartPosition = stream.Position;
        if (type < 0 || (int)type > 6)
            return null;

        var data = CreateFlacMetadataInstance(type);
        data.Initialize(stream, length, isLastBlock);

        stream.Seek(length - (stream.Position - streamStartPosition), SeekOrigin.Current);
        return data;
    }

    private FlacMetadata CreateFlacMetadataInstance(FlacMetaDataType flacMetadataType)
    {
        var flacMetadataTypeAsInt = (int) flacMetadataType;

        if (!_registeredmetadataTypes.TryGetValue(flacMetadataTypeAsInt, out var type))
        {
            return new DefaultFlacMetadata(flacMetadataType);
        }

        return (FlacMetadata) Activator.CreateInstance(type);
    }
}