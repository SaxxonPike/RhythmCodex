using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Processors
{
    [Service]
    public class Ddr573AudioNameFinder : IDdr573AudioNameFinder
    {
        private static readonly Dictionary<char, char> NameMap = new Dictionary<char, char>
        {
            {'A', '0'},
            {'B', 'A'},
            {'C', '1'},
            {'D', 'B'},
            {'E', '2'},
            {'F', 'C'},
            {'G', '3'},
            {'H', 'D'},
            {'I', '4'},
            {'J', 'E'},
            {'K', '5'},
            {'L', 'F'},
            {'M', '6'},
            {'N', 'G'},
            {'O', '7'},
            {'P', 'H'},
            {'Q', '8'},
            {'R', 'I'},
            {'S', '9'},
            {'T', 'J'},
            {'0', 'K'},
            {'1', 'L'},
            {'2', 'M'},
            {'3', 'N'},
            {'4', 'O'},
            {'5', 'P'},
            {'6', 'Q'},
            {'7', 'R'},
            {'8', 'S'},
            {'9', 'T'},
            {'U', 'U'},
            {'V', 'V'},
            {'W', 'W'},
            {'X', 'X'},
            {'Y', 'Y'},
            {'Z', 'Z'},
            {'_', '_'}
        };

        public string GetName(string sourceName)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));
            var prunedSourceName = Path.GetFileNameWithoutExtension(sourceName.ToUpperInvariant());
            if (prunedSourceName.Length != 8)
                return prunedSourceName;
            var output = string.Empty;

            switch (prunedSourceName[0])
            {
                case 'E':
                case 'M':
                case 'S':
                {
                    var nameToDecode = $"{prunedSourceName[6]}{prunedSourceName[5]}" +
                                       $"{prunedSourceName[4]}{prunedSourceName[7]}_{prunedSourceName[3]}";
                    if (nameToDecode.Any(c => !NameMap.ContainsKey(c)))
                        return null;
                    foreach (var c in nameToDecode)
                    {
                        var dc = NameMap[c];
                        if (output.Length < 4 || (dc >= '0' && dc <= '9'))
                            output += dc;
                    }
                    break;
                }

                default:
                    return null;
            }

            switch (prunedSourceName[0])
            {
                case 'E':
                    output = $"system-{output}";
                    break;
                case 'S':
                    output += "-preview";
                    break;
            }

            return output;
        }
    }
}