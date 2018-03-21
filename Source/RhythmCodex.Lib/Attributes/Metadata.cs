﻿using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Attributes
{
    public class Metadata : IMetadata
    {
        private readonly IDictionary<FlagData, bool> _flagDatas;
        private readonly IDictionary<NumericData, BigRational> _numericDatas;
        private readonly IDictionary<string, string> _stringDatas;

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

        public string this[string key]
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
            get => _numericDatas.ContainsKey(type) ? _numericDatas[type] : (BigRational?) null;
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
            get => _flagDatas.ContainsKey(type) ? _flagDatas[type] : (bool?) null;
            set
            {
                if (value == null)
                    _flagDatas.Remove(type);
                else
                    _flagDatas[type] = value.Value;
            }
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
}