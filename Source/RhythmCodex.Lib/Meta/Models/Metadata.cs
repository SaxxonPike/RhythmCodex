using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Meta.Models;

public class Metadata : IMetadata
{
    private Dictionary<FlagData, bool> _flagDatas;
    private Dictionary<NumericData, BigRational> _numericDatas;
    private Dictionary<string, string> _stringDatas;

    public Metadata()
    {
        _stringDatas = new Dictionary<string, string>();
        _numericDatas = new Dictionary<NumericData, BigRational>();
        _flagDatas = new Dictionary<FlagData, bool>();
    }

    private Metadata(
        IDictionary<string, string> stringDatas,
        IDictionary<NumericData, BigRational> numericDatas,
        IDictionary<FlagData, bool> flagDatas)
    {
        _stringDatas = stringDatas.ToDictionary(kv => kv.Key, kv => kv.Value);
        _numericDatas = numericDatas.ToDictionary(kv => kv.Key, kv => kv.Value);
        _flagDatas = flagDatas.ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public string? this[string key]
    {
        get
        {
            var actualKey = _stringDatas.Keys
                .FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            return actualKey == null ? null : _stringDatas[actualKey];
        }
        set
        {
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
        get => _numericDatas.ContainsKey(type) ? _numericDatas[type] : null;
        set
        {
            if (value == null)
                _numericDatas.Remove(type);
            else
                _numericDatas[type] = value.Value;
        }
    }

    public bool? this[FlagData type]
    {
        get => _flagDatas.ContainsKey(type) ? _flagDatas[type] : null;
        set
        {
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

    public bool MetadataEquals(Metadata other)
    {
        return _stringDatas.Count == other._stringDatas.Count && !_stringDatas.Except(other._stringDatas).Any() &&
               Enum.GetValues(typeof(NumericData)).Cast<NumericData>().All(v => this[v] == other[v]) &&
               Enum.GetValues(typeof(FlagData)).Cast<FlagData>().All(v => this[v] == other[v]);
    }

    public Metadata CloneMetadata()
    {
        return new Metadata(_stringDatas, _numericDatas, _flagDatas);
    }

    public void CloneMetadataFrom(Metadata other)
    {
        var metadata = other.CloneMetadata();
        _stringDatas = metadata._stringDatas;
        _numericDatas = metadata._numericDatas;
        _flagDatas = metadata._flagDatas;
    }

    public void CopyTo(IMetadata other)
    {
        foreach (var kv in _stringDatas)
            other[kv.Key] = kv.Value;
        foreach (var kv in _numericDatas)
            other[kv.Key] = kv.Value;
        foreach (var kv in _flagDatas)
            other[kv.Key] = kv.Value;
    }

    public override string ToString()
    {
        var output = new Dictionary<string, string>();
        foreach (var item in _numericDatas)
            output[item.Key.ToString()] = $"{(decimal) item.Value}";
        foreach (var item in _stringDatas)
            output[item.Key] = $"{item.Value}";
        foreach (var item in _flagDatas)
            output[item.Key.ToString()] = $"{item.Value}";

        return
            $"{GetType().Name}: {string.Join(", ", output.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"))}";
    }
}