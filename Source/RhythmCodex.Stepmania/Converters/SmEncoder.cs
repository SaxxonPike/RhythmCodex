using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public class SmEncoder
    {
        private readonly INoteEncoder _noteEncoder;

        public SmEncoder(INoteEncoder noteEncoder)
        {
            _noteEncoder = noteEncoder;
        }
        
        public IEnumerable<Command> Encode(IEnumerable<IChart> charts)
        {
            
        }
    }
}
