using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Ddr.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Processors;

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

    public Dictionary<int, string> Reverse(params int[] hashes)
    {
        var knownValues = new[]
        {
            "s573/config.dat",
            "boot/checksum.dat",
            "data/fpga/fpga_mp3.bin",
            "data/mp3/mp3_tab.bin",
            "data/tim/wfont/wfont_w.bin",
            "data/mdb/mdb.bin",
            "data/mdb/ja_mdb.bin",
            "data/tex/rembind.bin",
            "data/tex/subbind.bin",
            "data/chara/inst_s/inst_s.cmt",
            "data/chara/inst_d/inst_d.cmt",
            "data/course/onimode.bin",
            "data/course/onimode.ssq",
            "data/mp3/enc/M81D7HHJ.DAT",
            "data/chara/inst/inst.ctx",
            "data/motion/inst/inst.cmm",
            "data/chara/chara.pos",
            "data/chara/inst_s/inst_s.pos",
            "data/chara/inst_d/inst_d.pos",
            "data/chara/chara.lst",
            "data/chara/inst/inst.tmd",
            "data/chara/inst/inst.lst",
            "data/chara/inst_s/inst_s.tmd",
            "data/chara/inst_s/inst_s.lst",
            "data/chara/inst_d/inst_d.tmd",
            "data/chara/inst_d/inst_d.lst",
            "data/mcard/engl/pages.bin",
            "data/mcard/japa/pages.bin",
            "data/mcard/engl/pagel.bin",
            "data/mcard/japa/pagel.bin",
            "fpga_mp3.bin",
            "ir_id.bin",
            "net_id.bin",
            "kfont8.bin",
            "ascii_size8.bin",
            "ascii_size16.bin",
            "ascii_size24.bin",
            "music_info.bin",
            "course_info.bin",
            "group_list.bin",
            "got11j1b.bin",
            "got11j0b.bin",
            "got11h1f.bin",
            "d_res_ns.dat",
            "d_ending.dat",
            "d_title.dat",
            "d_cautio.dat",
            "g_id.dat",
            "system.fcn",
            "msucheck.fcn",
            "warning.fcn",
            "entry.fcn",
            "playstyl.fcn",
            "shoprank.fcn",
            "testmode.fcn",
            "gamemode.fcn",
            "ending.fcn",
            "konamiid.fcn",
            "card_ent.fcn",
            "card_res.fcn",
            "internet.fcn",
            "scorernk.fcn",
            "senddata.fcn",
            "result.fcn",
            "mus_sel.fcn",
            "mode_sel.fcn",
            "caution.fcn",
            "hitchart.fcn",
            "share_bg.fcn",
            "puzzle.fcn",
            "m_image.fcn",
            "share.fcn",
            "bgm0128.fcn",
            "snapshot.img",
            "dl/n1/pathtab.bin",
            "data/kanji/eames.cmp",
            "data/spu/system.vas",
            "soft/s573/overlay/bin/dbugtest.olb",
            "soft/s573/overlay/bin/gtest.olb",
            "soft/s573/overlay/bin/play.olb",
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