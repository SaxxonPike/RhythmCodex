namespace RhythmCodex.Thirdparty.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/
    
    internal interface IInterpolate
    {
        double? Interpolate( double p);
    }
}