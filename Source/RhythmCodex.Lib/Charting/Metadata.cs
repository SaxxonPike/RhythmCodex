using System.Collections.Generic;
using Numerics;

namespace RhythmCodex.Charting
{
    public class Metadata
    {
        private readonly IDictionary<StringData, string> _stringDatas = new Dictionary<StringData, string>();
        private readonly IDictionary<NumericData, BigRational> _numericDatas = new Dictionary<NumericData, BigRational>();
        private readonly ISet<FlagData> _flagDatas = new HashSet<FlagData>();

        public string this[StringData type]
        {
            get => _stringDatas.ContainsKey(type) ? _stringDatas[type] : default(string);
            set
            {
                if (value == default(string))
                    _stringDatas.Remove(type);
                else
                    _stringDatas[type] = value;
            }
        }

        public BigRational this[NumericData type]
        {
            get => _numericDatas.ContainsKey(type) ? _numericDatas[type] : default(BigRational);
            set
            {
                if (value == default(BigRational))
                    _numericDatas.Remove(type);
                else
                    _numericDatas[type] = value;
            }
        }

        public bool this[FlagData type]
        {
            get => _flagDatas.Contains(type);
            set
            {
                if (!value)
                    _flagDatas.Remove(type);
                else
                    _flagDatas.Add(type);
            }
        }
    }
}
