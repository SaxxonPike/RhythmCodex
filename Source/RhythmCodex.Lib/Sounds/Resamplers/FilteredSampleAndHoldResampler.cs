using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Sounds.Resamplers
{
    [Service]
    public class FilteredSampleAndHoldResampler : IResampler
    {
        private readonly IFilterProvider _filterProvider;
        private readonly IResampler _baseResampler;

        public FilteredSampleAndHoldResampler(IFilterProvider filterProvider)
        {
            _filterProvider = filterProvider;
            _baseResampler = new SampleAndHoldResampler();
        }
        
        public string Name => "filteredsampleandhold";

        public int Priority => int.MinValue + 1;

        public IList<float> Resample(IList<float> data, float sourceRate, float targetRate)
        {
            var unfiltered = _baseResampler.Resample(data, sourceRate, targetRate);
            var filter = _filterProvider.Get(FilterType.LowPass).First();
            var context = filter.Create(targetRate, sourceRate / 2);
            return context.Filter(unfiltered);
        }
    }
}