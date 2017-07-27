using System;
using System.Collections.Generic;
using System.Linq;
using Numerics;

namespace RhythmCodex.Charting
{
    public class Metadata
    {
        private readonly IDictionary<StringData, string> _stringDatas;
        private readonly IDictionary<NumericData, BigRational> _numericDatas;
        private readonly IDictionary<FlagData, bool> _flagDatas;

        public Metadata()
        {
            _stringDatas = new Dictionary<StringData, string>();
            _numericDatas = new Dictionary<NumericData, BigRational>();
            _flagDatas = new Dictionary<FlagData, bool>();
        }

        private Metadata(
            IDictionary<StringData, string> stringDatas, 
            IDictionary<NumericData, BigRational> numericDatas, 
            IDictionary<FlagData, bool> flagDatas)
        {
            _stringDatas = stringDatas.ToDictionary(kv => kv.Key, kv => kv.Value);
            _numericDatas = numericDatas.ToDictionary(kv => kv.Key, kv => kv.Value);
            _flagDatas = flagDatas.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
        
        public bool MetadataEquals(Metadata other)
        {
            return Enum.GetValues(typeof(StringData)).Cast<StringData>().All(v => this[v] == other[v]) &&
                Enum.GetValues(typeof(NumericData)).Cast<NumericData>().All(v => this[v] == other[v]) &&
                Enum.GetValues(typeof(FlagData)).Cast<FlagData>().All(v => this[v] == other[v]);
        }

        public Metadata CloneMetadata()
        {
            return new Metadata(_stringDatas, _numericDatas, _flagDatas);
        }
        
        public string this[StringData type]
        {
            get => _stringDatas.ContainsKey(type) ? _stringDatas[type] : null;
            set
            {
                if (value == null)
                    _stringDatas.Remove(type);
                else
                    _stringDatas[type] = value;
            }
        }

        public BigRational? this[NumericData type]
        {
            get => _numericDatas.ContainsKey(type) ? _numericDatas[type] : (BigRational?)null;
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
            get => _flagDatas.ContainsKey(type) ? _flagDatas[type] : (bool?)null;
            set
            {
                if (value == null)
                    _flagDatas.Remove(type);
                else
                    _flagDatas[type] = value.Value;
            }
        }

        public override string ToString()
        {
            var output = new Dictionary<string, string>();
            foreach (var item in _numericDatas)
                output[item.Key.ToString()] = $"{(decimal)item.Value}";
            foreach (var item in _stringDatas)
                output[item.Key.ToString()] = $"{item.Value}";
            foreach (var item in _flagDatas)
                output[item.Key.ToString()] = $"{item.Value}";

            return $"{GetType().Name}: {string.Join(", ", output.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"))}";
        }
    }
}
