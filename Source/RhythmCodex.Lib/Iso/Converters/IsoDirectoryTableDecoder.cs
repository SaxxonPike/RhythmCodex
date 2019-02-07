using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Cd.Model;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;
using RhythmCodex.Iso.Streamers;

namespace RhythmCodex.Iso.Converters
{
    [Service]
    public class IsoDirectoryTableDecoder : IIsoDirectoryTableDecoder
    {
        private readonly IIsoSectorStreamFactory _isoSectorStreamFactory;
        private readonly IIsoDirectoryRecordDecoder _isoDirectoryRecordDecoder;

        public IsoDirectoryTableDecoder(IIsoSectorStreamFactory isoSectorStreamFactory, IIsoDirectoryRecordDecoder isoDirectoryRecordDecoder)
        {
            _isoSectorStreamFactory = isoSectorStreamFactory;
            _isoDirectoryRecordDecoder = isoDirectoryRecordDecoder;
        }
        
        public IList<IsoDirectoryRecord> Decode(IEnumerable<ICdSector> sectors)
        {
            return DecodeInternal(sectors).ToList();
        }

        private IEnumerable<IsoDirectoryRecord> DecodeInternal(IEnumerable<ICdSector> sectors)
        {
            using (var stream = _isoSectorStreamFactory.Open(sectors))
            {
                while (true)
                {
                    var record = _isoDirectoryRecordDecoder.Decode(stream, false);
                    if (record != null)
                        yield return record;
                    else
                        yield break;
                }
            }
        }
    }
}