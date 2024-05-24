using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

[Service]
public class SmMetadataChanger : ISmMetadataChanger
{
    private static void AddOrReplace(ICollection<Command> commands, string key, params string[] values)
    {
        var command = commands.FirstOrDefault(c => c.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
        if (command != null)
            command.Values = values.ToList();
        else
            commands.Add(new Command { Name = key, Values = values.ToList() });
    }

    private static void EditWhere(IEnumerable<Command> commands, Func<Command, bool> predicate, Action<Command> replace)
    {
        foreach (var command in commands.Where(predicate))
            replace(command);
    }

    public void SetTitle(ICollection<Command> commands, string value) =>
        AddOrReplace(commands, ChartTag.TitleTag, value);

    public void SetSubtitle(ICollection<Command> commands, string value) =>
        AddOrReplace(commands, ChartTag.SubTitleTag, value);

    public void SetArtist(ICollection<Command> commands, string value) =>
        AddOrReplace(commands, ChartTag.ArtistTag, value);

    public void SetSubartist(ICollection<Command> commands, string value) =>
        AddOrReplace(commands, ChartTag.CreditTag, value);

    public void SetDifficulty(ICollection<Command> commands, string mode, string difficulty, string value) =>
        EditWhere(commands,
            c => c.Name.Equals(ChartTag.NotesTag, StringComparison.OrdinalIgnoreCase) &&
                 mode.Equals(c.Values.FirstOrDefault(), StringComparison.OrdinalIgnoreCase) &&
                 difficulty.Equals(c.Values.Skip(2).FirstOrDefault(), StringComparison.OrdinalIgnoreCase),
            c => c.Values[3] = value);

    public void SetBpm(ICollection<Command> commands, string minBpm, string maxBpm) =>
        AddOrReplace(commands, ChartTag.DisplayBpmTag, minBpm, maxBpm);

    public void SetBannerImage(ICollection<Command> commands, string fileName) =>
        AddOrReplace(commands, ChartTag.BannerTag, fileName);

    public void SetBackgroundImage(ICollection<Command> commands, string fileName) =>
        AddOrReplace(commands, ChartTag.BackgroundTag, fileName);
}