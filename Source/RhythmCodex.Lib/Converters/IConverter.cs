using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhythmCodex.Converters
{
    public interface IConverter<in TFrom, out TTo>
    {
        TTo Convert(TFrom data);
    }
}
