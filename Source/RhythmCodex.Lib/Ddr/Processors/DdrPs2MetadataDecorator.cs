using System.Linq;
using RhythmCodex.Ddr.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Ddr.Processors
{
    [Service]
    public class DdrPs2MetadataDecorator : IDdrPs2MetadataDecorator
    {
        public void Decorate(ChartSet chartSet, DdrDatabaseEntry meta)
        {
            if (meta == null)
                return;

            foreach (var chart in chartSet.Charts.Where(c => c[NumericData.Id] != null))
            {
                var id = (int) chart[NumericData.Id].Value;
                switch (id)
                {
                    case 0x0113:
                    case 0x0114:
                    case 0x0116:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x0];
                        break;
                    case 0x0213:
                    case 0x0214:
                    case 0x0216:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x1];
                        break;
                    case 0x0313:
                    case 0x0314:
                    case 0x0316:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x2];
                        break;
                    case 0x0413:
                    case 0x0414:
                    case 0x0416:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x4];
                        break;
                    case 0x0613:
                    case 0x0614:
                    case 0x0616:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x3];
                        break;
                    case 0x0118:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x8];
                        break;
                    case 0x0218:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x9];
                        break;
                    case 0x0318:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0xA];
                        break;
                    case 0x0418:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0xC];
                        break;
                    case 0x0618:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0xB];
                        break;
                }
            }
        }
    }
}