using System;
using System.Collections.Generic;
using System.Linq;
using Numerics;

namespace RhythmCodex.Charting
{
    public class Metadata
    {
        private readonly IDictionary<StringData, string> _stringDatas = new Dictionary<StringData, string>();
        private readonly IDictionary<NumericData, BigRational> _numericDatas = new Dictionary<NumericData, BigRational>();
        private readonly IDictionary<FlagData, bool> _flagDatas = new Dictionary<FlagData, bool>();

        public bool MetadataEquals(Metadata other)
        {
            return Enum.GetValues(typeof(StringData)).Cast<StringData>().All(v => this[v] == other[v]) &&
                Enum.GetValues(typeof(NumericData)).Cast<NumericData>().All(v => this[v] == other[v]) &&
                Enum.GetValues(typeof(FlagData)).Cast<FlagData>().All(v => this[v] == other[v]);
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
            get => _numericDatas.ContainsKey(type) ? _numericDatas[type] : 0;
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
    }
}
