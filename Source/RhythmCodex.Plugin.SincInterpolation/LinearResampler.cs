using System.Collections.Generic;
using RhythmCodex.IoC;

namespace RhythmCodex.Plugin.SincInterpolation
{
    [Service]
    public class LinearResampler : BaseResampler
    {
        protected override IInterpolation GetInterpolation(IList<float> x, IList<float> y) =>
            new LinearInterpolation(x, y);

        public override string Name => "linear";

        public override int Priority => 1;
    }
}