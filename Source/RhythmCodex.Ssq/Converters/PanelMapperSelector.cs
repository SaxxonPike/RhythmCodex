using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class PanelMapperSelector : IPanelMapperSelector
    {
        private readonly IEnumerable<IPanelMapper> _panelMappers;

        public PanelMapperSelector(IEnumerable<IPanelMapper> panelMappers)
        {
            _panelMappers = panelMappers;
        }
        
        public IPanelMapper Select(IEnumerable<Step> steps)
        {
            var stepList = steps.AsList();
            return _panelMappers
                .First(pm => stepList.Select(s => pm.Map(s.Panels)).All(m => m != null));
        }
    }
}
