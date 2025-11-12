using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Sm;
using RhythmCodex.Charts.Sm.Model;
using RhythmCodex.Games.Ddr.Models;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Games.Ddr.Processors;

[Service]
public class DdrMetadataDecorator : IDdrMetadataDecorator
{
    public void Decorate(ChartSet? chartSet, DdrDatabaseEntry? meta, MetadataDecoratorFileExtensions extensions)
    {
        if (meta == null || chartSet == null)
            return;
                
        if (chartSet.Metadata == null)
            chartSet.Metadata = new Metadata();

        chartSet.Metadata[ChartTag.TitleTag] = meta.LongName ?? meta.ShortName ?? meta.Id;
        chartSet.Metadata[ChartTag.MusicTag] = $"{meta.Id}.{extensions.Audio}";
        chartSet.Metadata[ChartTag.OffsetTag] = $"{(decimal)(-chartSet.Charts.First()[NumericData.LinearOffset] ?? 0)}";
        chartSet.Metadata[ChartTag.DisplayBpmTag] = $"{meta.MinBpm}:{meta.MaxBpm}";
        chartSet.Metadata[ChartTag.BannerTag] = $"{meta.Id}_th.{extensions.Graphics}";
        chartSet.Metadata[ChartTag.BackgroundTag] = $"{meta.Id}_bk.{extensions.Graphics}";
            
        foreach (var chart in chartSet.Charts.Where(c => c[NumericData.Id] != null))
        {
            var id = (int)(chart[NumericData.Id] ?? 0);

            if (meta.Difficulties == null || meta.Difficulties.Length == 0)
                continue;

            if (meta.Difficulties.Length < 8)
            {
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
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x0];
                        break;
                    case 0x0218:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x1];
                        break;
                    case 0x0318:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x2];
                        break;
                    case 0x0418:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x4];
                        break;
                    case 0x0618:
                        chart[NumericData.PlayLevel] = meta.Difficulties[0x3];
                        break;
                }
            }
            else
            {
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