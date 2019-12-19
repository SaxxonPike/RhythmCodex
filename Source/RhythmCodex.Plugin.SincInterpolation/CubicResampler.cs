using System.Collections.Generic;
using RhythmCodex.IoC;

namespace RhythmCodex.Plugin.SincInterpolation
{
    [Service]
    public class CubicResampler : BaseResampler
    {
        protected override IInterpolation GetInterpolation(IList<float> x, IList<float> y) =>
            new CubicSplineInterpolation(x, y);

        public override string Name => "cubic";

        public override int Priority => 10;
    }
}