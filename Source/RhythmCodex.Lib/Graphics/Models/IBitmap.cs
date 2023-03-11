namespace RhythmCodex.Graphics.Models
{
    public interface IBitmap
    {
        int Width { get; }
        int Height { get; }
        int[] Data { get; }
    }
}