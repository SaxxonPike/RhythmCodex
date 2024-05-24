using System.IO;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Streamers;

public interface IDdr573ImageStreamReader
{
    Ddr573Image Read(Stream gameDatStream, int gameDatLength);
    Ddr573Image Read(Stream gameDatStream, int gameDatLength, Stream cardDatStream, int cardDatLength);
}