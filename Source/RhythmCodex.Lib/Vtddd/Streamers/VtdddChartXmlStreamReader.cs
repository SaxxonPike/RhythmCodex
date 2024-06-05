using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Streamers;

[Service]
public class VtdddChartXmlStreamReader : IVtdddChartXmlStreamReader
{
    public List<VtdddStep> Read(Stream stream)
    {
        // okay, this is a hack to bypass the "XML version 1.1 is invalid" error
        // var doc = XDocument.Load(stream);
        var reader = new StreamReader(stream);
        var text = reader.ReadToEnd().Replace("<?xml version='1.1'?>", "<?xml version='1.0'?>");
        var doc = XDocument.Parse(text);
        var stepNodes = doc.Root?.Elements();
        var steps = stepNodes?.SelectMany(DecodeStep).ToList() ?? [];
        return steps;
    }

    private static IEnumerable<VtdddStep> DecodeStep(XElement xml)
    {
        var panels = new List<(int Player, int Panel, bool Hold)>();

        switch (xml.Name.LocalName.ToLower(CultureInfo.InvariantCulture))
        {
            case "left":
                panels.Add((Player: 0, Panel: 0, Hold: false));
                break;
            case "down":
                panels.Add((Player: 0, Panel: 1, Hold: false));
                break;
            case "up":
                panels.Add((Player: 0, Panel: 2, Hold: false));
                break;
            case "right":
                panels.Add((Player: 0, Panel: 3, Hold: false));
                break;
            case "d_left_down":
                panels.Add((Player: 0, Panel: 0, Hold: false));
                panels.Add((Player: 0, Panel: 1, Hold: false));
                break;
            case "d_left_up":
                panels.Add((Player: 0, Panel: 0, Hold: false));
                panels.Add((Player: 0, Panel: 2, Hold: false));
                break;
            case "d_left_right":
                panels.Add((Player: 0, Panel: 0, Hold: false));
                panels.Add((Player: 0, Panel: 3, Hold: false));
                break;
            case "d_up_down":
                panels.Add((Player: 0, Panel: 1, Hold: false));
                panels.Add((Player: 0, Panel: 2, Hold: false));
                break;
            case "d_right_down":
                panels.Add((Player: 0, Panel: 1, Hold: false));
                panels.Add((Player: 0, Panel: 3, Hold: false));
                break;
            case "d_right_up":
                panels.Add((Player: 0, Panel: 2, Hold: false));
                panels.Add((Player: 0, Panel: 3, Hold: false));
                break;
            case "left_float":
                panels.Add((Player: 0, Panel: 0, Hold: true));
                break;
            case "down_float":
                panels.Add((Player: 0, Panel: 1, Hold: true));
                break;
            case "up_float":
                panels.Add((Player: 0, Panel: 2, Hold: true));
                break;
            case "right_float":
                panels.Add((Player: 0, Panel: 3, Hold: true));
                break;
            default:
                throw new Exception("report this");
        }

        foreach (var (player, panel, hold) in panels)
        {
            yield return new VtdddStep
            {
                Player = player,
                Panel = panel,
                Show = xml.GetInt("show"),
                Target = xml.GetInt("target"),
                End = xml.GetInt("end"),
                Hold = hold
            };
        }
    }
}