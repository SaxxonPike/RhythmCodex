using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Ddr.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Processors
{
    [Service]
    public class Ddr573ImageFileNameHasher : IDdr573ImageFileNameHasher
    {
        public int Calculate(string name)
        {
            const int t1 = 0x4c11db7;
            var v1 = 0;
            foreach (int c in name)
            {
                for (var i = 0; i < 6; i++)
                {
                    var a0 = (v1 << 1) | ((c >> i) & 1);
                    v1 = ((v1 >> 31) & t1) ^ a0;
                }
            }

            return v1;
        }

        public IDictionary<int, string> Reverse(params int[] hashes)
        {
            var knownValues = new[]
            {
                "data/fpga/fpga_mp3.bin",
                "data/mdb/mdb.bin",
                "data/tex/rembind.bin",
                "data/tex/subbind.bin",
                "data/chara/inst_s/inst_s.cmt",
                "data/chara/inst_d/inst_d.cmt",
                "data/tim/wfont/wfont_w.bin",
                "data/movie/common/cddana.sbs",
                "data/movie/common/scrob_25.sbs",
                "data/movie/common/scrbk_16.sbs",
                "data/movie/common/re2424.sbs",
                "data/movie/common/acxx28.sbs",
                "data/course/onimode.bin",
                "data/mp3/mp3_tab.bin",
                "data/course/onimode.ssq",
                "data/movie/howto/hwrlja.sbs",
                "data/movie/howto/hwfroa.sbs",
                "data/movie/howto/hwnora.sbs",
                "data/movie/howto/hwhaja.sbs",
                "data/mp3/enc/M81D7HHJ.DAT",
                "boot/checksum.dat",
                "data/mdb/ja_mdb.bin",
                "data/mcard/engl/pages.bin",
                "data/mcard/japa/pages.bin",
                "data/mcard/engl/pagel.bin",
                "data/mcard/japa/pagel.bin"
            };

            var knownSongPatterns = new[]
            {
                "data/course/{0}_nm.tim",
                "data/course/{0}_in.tim",
                "data/course/{0}_ta.tim",
                "data/course/{0}_th.tim",
                "data/course/{0}_bk.cmt",
                "data/mdb/{0}/{0}_nm.tim",
                "data/mdb/{0}/{0}_in.tim",
                "data/mdb/{0}/{0}_ta.tim",
                "data/mdb/{0}/{0}_th.tim",
                "data/mdb/{0}/{0}_ta.cmt",
                "data/mdb/{0}/{0}_bk.cmt",
                "data/mdb/{0}/all.csq"
            };

            var knownAnimPatterns = new[]
            {
                "data/anime/{0}/{0}.can",
                "data/anime/{0}/{0}.anm"
            };

            var knownSongNames = DdrConstants.KnownSongNames;
            var knownAnimeNames = DdrConstants.KnownAnimeNames;

            var allKnownValues = knownValues
                .Concat(knownSongPatterns.SelectMany(p =>
                    knownSongNames.Select(s =>
                        string.Format(p, s))))
                .Concat(knownAnimPatterns.SelectMany(p =>
                    knownAnimeNames.Select(s =>
                        string.Format(p, s))));

            var dict = new Dictionary<int, string>();
            foreach (var path in allKnownValues)
                dict[Calculate(path)] = path;

            return dict;
        }
    }
}