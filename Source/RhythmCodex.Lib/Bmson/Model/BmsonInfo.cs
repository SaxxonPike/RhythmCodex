using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bmson.Model
{
    [Model]
    public class BmsonInfo : JsonWrapper
    {
        public string Title
        {
            get => Get<string>("title");
            set => Set("title", value);
        }

        public string Subtitle
        {
            get => Get<string>("subtitle");
            set => Set("subtitle", value);
        }

        public string Artist
        {
            get => Get<string>("artist");
            set => Set("artist", value);
        }

        public List<string> Subartists
        {
            get => Get<List<string>>("subartists");
            set => Set("subartists", value);
        }

        public string Genre
        {
            get => Get<string>("genre");
            set => Set("genre", value);
        }

        public string ModeHint
        {
            get => Get<string>("mode_hint");
            set => Set("mode_hint", value);
        }

        public string ChartName
        {
            get => Get<string>("chart_name");
            set => Set("chart_name", value);
        }

        public long Level
        {
            get => Get<long>("level");
            set => Set("level", value);
        }

        public double InitialBpm
        {
            get => Get<double>("init_bpm");
            set => Set("init_bpm", value);
        }

        public double JudgeRank
        {
            get => Get<double>("judge_rank");
            set => Set("judge_rank", value);
        }

        public double Total
        {
            get => Get<double>("total");
            set => Set("total", value);
        }

        public string BackgroundImage
        {
            get => Get<string>("back_image");
            set => Set("back_image", value);
        }

        public string EyecatchImage
        {
            get => Get<string>("eyecatch_image");
            set => Set("eyecatch_image", value);
        }

        public string BannerImage
        {
            get => Get<string>("banner_image");
            set => Set("banner_image", value);
        }

        public string PreviewMusic
        {
            get => Get<string>("preview_music");
            set => Set("preview_music", value);
        }

        public long Resolution
        {
            get => Get<long>("resolution");
            set => Set("resolution", value);
        }
    }
}