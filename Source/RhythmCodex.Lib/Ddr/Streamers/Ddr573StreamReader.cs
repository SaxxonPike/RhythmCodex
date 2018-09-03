using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archives;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Streamers
{
    // Directory offsets:
    // Old DDR: 0x100000
    // New DDR: 0xFE4000
    // Disney's Rave: 0x64F000*
    // DCT: 0x100800*
    // * uses a strange 0x1000 interleave block of 256 bytes at the end for some reason?
    
    public class Ddr573StreamReader : IDdr573StreamReader
    {
        public Ddr573Image Read(Stream gameDatStream, int gameDatLength)
        {
            var gameDatReader = new BinaryReader(gameDatStream);

            return new Ddr573Image
            {
                Modules = new Dictionary<int, byte[]>
                {
                    {0, gameDatReader.ReadBytes(gameDatLength)}
                }
            };
        }

        public Ddr573Image Read(Stream gameDatStream, int gameDatLength, Stream cardDatStream, int cardDatLength)
        {
            var gameDatReader = new BinaryReader(gameDatStream);
            var cardDatReader = new BinaryReader(cardDatStream);

            return new Ddr573Image
            {
                Modules = new Dictionary<int, byte[]>
                {
                    {0, gameDatReader.ReadBytes(gameDatLength)},
                    {1, cardDatReader.ReadBytes(cardDatLength)}
                }
            };
        }
    }
}