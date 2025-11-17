using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Metadatas.Models;

public class Metadata : IMetadata
{
    private Dictionary<FlagData, bool>? _flagDatas;
    private Dictionary<NumericData, BigRational>? _numericDatas;
    private Dictionary<string, string>? _stringDatas;

    public Metadata()
    {
    }

    private Metadata(
        IDictionary<string, string>? stringDatas,
        IDictionary<NumericData, BigRational>? numericDatas,
        IDictionary<FlagData, bool>? flagDatas)
    {
        _stringDatas = stringDatas?.ToDictionary(kv => kv.Key, kv => kv.Value);
        _numericDatas = numericDatas?.ToDictionary(kv => kv.Key, kv => kv.Value);
        _flagDatas = flagDatas?.ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public string? this[string key]
    {
        get
        {
            return _stringDatas?
                .Where(kv => kv.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                .Select(kv => kv.Value)
                .DefaultIfEmpty(null)
                .First();
        }
        set
        {
            _stringDatas ??= new Dictionary<string, string>();
            var actualKey = _stringDatas.Keys
                                .FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase))
                            ?? key;
            if (value == null)
                _stringDatas.Remove(actualKey);
            else
                _stringDatas[actualKey] = value;
        }
    }

    public BigRational? this[NumericData type]
    {
        get => _numericDatas?.TryGetValue(type, out var val) ?? false ? val : null;
        set
        {
            _numericDatas ??= new Dictionary<NumericData, BigRational>();
            if (value == null)
                _numericDatas.Remove(type);
            else
                _numericDatas[type] = value.Value;
        }
    }

    public bool? this[FlagData type]
    {
        get => _flagDatas?.TryGetValue(type, out var val) ?? false ? val : null;
        set
        {
            _flagDatas ??= new Dictionary<FlagData, bool>();
            if (value == null)
                _flagDatas.Remove(type);
            else
                _flagDatas[type] = value.Value;
        }
    }

    public string? this[StringData type]
    {
        get => this[type.ToString()];
        set => this[type.ToString()] = value;
    }

    public bool IsMetadataEmpty =>
        _stringDatas is not { Count: > 0 } &&
        _flagDatas is not { Count: > 0 } &&
        _numericDatas is not { Count: > 0 };

    public bool MetadataEquals(Metadata? other)
    {
        if (other == null || other.IsMetadataEmpty)
            return IsMetadataEmpty;

        var myStrings = _stringDatas ?? [];
        var myFlags = _flagDatas ?? [];
        var myNumbers = _numericDatas ?? [];

        var otherStrings = other._stringDatas ?? [];
        var otherFlags = other._flagDatas ?? [];
        var otherNumbers = other._numericDatas ?? [];

        return myStrings.Count == otherStrings.Count &&
               myFlags.Count == otherFlags.Count &&
               myNumbers.Count == otherNumbers.Count &&
               !myStrings
                   .Select(kv => (kv.Key, kv.Value))
                   .Except(otherStrings.Select(kv => (kv.Key, kv.Value)))
                   .Any() &&
               !myFlags
                   .Select(kv => (kv.Key, kv.Value))
                   .Except(otherFlags.Select(kv => (kv.Key, kv.Value)))
                   .Any() &&
               !myNumbers
                   .Select(kv => (kv.Key, kv.Value))
                   .Except(otherNumbers.Select(kv => (kv.Key, kv.Value)))
                   .Any();
    }

    public Metadata CloneMetadata() =>
        new(_stringDatas, _numericDatas, _flagDatas);

    public void CloneMetadataFrom(Metadata? other)
    {
        if (other == null)
            return;

        var metadata = other.CloneMetadata();
        _stringDatas = metadata._stringDatas;
        _numericDatas = metadata._numericDatas;
        _flagDatas = metadata._flagDatas;
    }

    public void CopyMetadataTo(IMetadata? other)
    {
        if (other == null)
            return;

        foreach (var kv in _stringDatas ?? [])
            other[kv.Key] = kv.Value;
        foreach (var kv in _numericDatas ?? [])
            other[kv.Key] = kv.Value;
        foreach (var kv in _flagDatas ?? [])
            other[kv.Key] = kv.Value;
    }

    public override string ToString()
    {
        var output = new Dictionary<string, string>();
        foreach (var item in _numericDatas ?? [])
            output[item.Key.ToString()] = $"{(decimal)item.Value}";
        foreach (var item in _stringDatas ?? [])
            output[item.Key] = $"{item.Value}";
        foreach (var item in _flagDatas ?? [])
            output[item.Key.ToString()] = $"{item.Value}";

        return
            $"{GetType().Name}: {string.Join(", ", output.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"))}";
    }
}