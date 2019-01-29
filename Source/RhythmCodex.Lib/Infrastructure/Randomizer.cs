using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Infrastructure
{
    [Service]
    public class Randomizer : IRandomizer
    {
        private readonly Random _random;

        public Randomizer() => 
            _random = new Random();

        public int GetInt(int max) => 
            _random.Next(max);
    }
}