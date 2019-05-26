namespace RhythmCodex.Chd.Model
{
    public enum V5CompressionType
    {
        // codec #0
        // these types are live when running
        COMPRESSION_TYPE_0 = 0,

        // codec #1
        COMPRESSION_TYPE_1 = 1,

        // codec #2
        COMPRESSION_TYPE_2 = 2,

        // codec #3
        COMPRESSION_TYPE_3 = 3,

        // no compression; implicit length = hunkbytes
        COMPRESSION_NONE = 4,

        // same as another block in this chd
        COMPRESSION_SELF = 5,

        // same as a hunk's worth of units in the parent chd
        COMPRESSION_PARENT = 6,

        // start of small RLE run (4-bit length)
        // these additional pseudo-types are used for compressed encodings:
        COMPRESSION_RLE_SMALL,

        // start of large RLE run (8-bit length)
        COMPRESSION_RLE_LARGE,

        // same as the last COMPRESSION_SELF block
        COMPRESSION_SELF_0,

        // same as the last COMPRESSION_SELF block + 1
        COMPRESSION_SELF_1,

        // same block in the parent
        COMPRESSION_PARENT_SELF,

        // same as the last COMPRESSION_PARENT block
        COMPRESSION_PARENT_0,

        // same as the last COMPRESSION_PARENT block + 1
        COMPRESSION_PARENT_1
    }
}