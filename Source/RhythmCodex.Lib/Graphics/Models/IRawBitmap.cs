namespace RhythmCodex.Graphics.Models
{
    public interface IRawBitmap
    {
        int Width { get; }
        int Height { get; }
        int[] Data { get; }
    }
}