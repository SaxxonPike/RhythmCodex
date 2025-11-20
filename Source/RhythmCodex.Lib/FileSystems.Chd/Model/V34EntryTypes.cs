namespace RhythmCodex.FileSystems.Chd.Model;

public enum V34EntryTypes
{
    V34_MAP_ENTRY_TYPE_INVALID = 0, // invalid type
    V34_MAP_ENTRY_TYPE_COMPRESSED = 1, // standard compression
    V34_MAP_ENTRY_TYPE_UNCOMPRESSED = 2, // uncompressed data
    V34_MAP_ENTRY_TYPE_MINI = 3, // mini: use offset as raw data
    V34_MAP_ENTRY_TYPE_SELF_HUNK = 4, // same as another hunk in this file
    V34_MAP_ENTRY_TYPE_PARENT_HUNK = 5, // same as a hunk in the parent file
    V34_MAP_ENTRY_TYPE_2ND_COMPRESSED = 6 // compressed with secondary algorithm (usually FLAC CDDA)
}