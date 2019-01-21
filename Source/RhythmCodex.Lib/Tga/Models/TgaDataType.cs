namespace RhythmCodex.Tga.Models
{
    // Reference: http://paulbourke.net/dataformats/tga/

    public enum TgaDataType
    {
        NoImageData = 0,
        UncompressedIndexed = 1,
        UncompressedRgb = 2,
        UncompressedMonochrome = 3,
        RleIndexed = 9,
        RleRgb = 10,
        CompressedMonochrome = 11,
        HuffmanIndexed = 32,
        HuffmanQuadtreeIndexed = 33
    }
}