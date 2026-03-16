using System.Collections.Generic;
using System.IO;
using System.Text;
using RhythmCodex.FileSystems.Cue.Model;
using RhythmCodex.FileSystems.Cue.Processors;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Cue.Streamers;

[Service]
public sealed class CueStreamReader(ICueTokenProcessor cueTokenProcessor) : ICueStreamReader
{
    public CueFile ReadCue(Stream stream)
    {
        var reader = new StreamReader(stream);
        var sb = new StringBuilder();
        string? currentTrack = null;
        (string Name, string Type)? currentFile = null;

        //
        // This will store the CUE lines associated with each file and track.
        //

        var root = new List<List<string>>();
        var tracks =
            new Dictionary<string, (string Type, string FileName, string FileType, List<List<string>> Lines)>();

        //
        // Process each line.
        //

        while (reader.ReadLine() is { } line)
        {
            //
            // Extract tokens from the CUE line.
            //

            var tokens = cueTokenProcessor.ProcessTokens(line, sb);

            if (tokens.Count < 1)
                continue;

            //
            // Process the CUE line.
            //

            switch (tokens[0].ToLowerInvariant())
            {
                //
                // FILE line. This specifies the file from which
                // the following data will be read from.
                //

                case "file":
                {
                    if (tokens.Count < 3)
                        throw new RhythmCodexException(
                            "FILE must have at least <filename> and <type>.");

                    currentFile = (Name: tokens[1], Type: tokens[2]);
                    break;
                }

                //
                // TRACK line. This specifies the track on the disc
                // to which the following info will be associated.
                //

                case "track":
                {
                    if (tokens.Count < 3)
                        throw new RhythmCodexException(
                            "TRACK must have at least 3 tokens.");

                    if (currentFile == null)
                        throw new RhythmCodexException(
                            "TRACK is not permitted without a FILE.");

                    currentTrack = tokens[1];

                    if (tracks.TryGetValue(currentTrack, out var info) && tokens[2] != info.Type)
                        throw new RhythmCodexException(
                            $"TRACK \"{currentTrack}\" is \"{info.Type}\". It cannot also be \"{tokens[2]}\".");

                    var (fileName, fileType) = currentFile.Value;
                    if (!tracks.ContainsKey(currentTrack))
                        tracks.Add(currentTrack, (Type: tokens[2], FileName: fileName, FileType: fileType, Lines: []));

                    break;
                }

                //
                // These lines require both FILE and TRACK but are otherwise
                // not processed here. This is for commands with 3 tokens.
                //

                case "index":
                {
                    if (tokens.Count < 3)
                        throw new RhythmCodexException(
                            $"{tokens[0].ToUpper()} must have at least 3 tokens.");

                    if (currentFile == null || currentTrack == null)
                        throw new RhythmCodexException(
                            $"{tokens[0].ToUpper()} is not permitted without a FILE and TRACK.");

                    tracks[currentTrack].Lines.Add(tokens);

                    break;
                }

                //
                // Like above, but only 2 tokens are required.
                //

                case "pregap":
                case "postgap":
                case "flags":
                case "isrc":
                {
                    if (tokens.Count < 2)
                        throw new RhythmCodexException(
                            $"{tokens[0].ToUpper()} must have at least 2 tokens.");

                    if (currentFile == null || currentTrack == null)
                        throw new RhythmCodexException(
                            $"{tokens[0].ToUpper()} is not permitted without a FILE and TRACK.");

                    tracks[currentTrack].Lines.Add(tokens);

                    break;
                }


                //
                // These lines apply to the root.
                //

                case "catalog":
                case "cdtextfile":
                {
                    root.Add(tokens);
                    break;
                }

                //
                // Other lines can appear anywhere and are associated to
                // either the root or the track, depending on if the line
                // is under a TRACK. We will assume this is the case for
                // all lines that are not processed.
                //

                default:
                {
                    if (currentTrack == null)
                        root.Add(tokens);
                    else
                        tracks[currentTrack].Lines.Add(tokens);

                    break;
                }
            }
        }

        //
        // Once all the lines are read in, transform the data
        // to the internal format.
        //

        var result = new CueFile
        {
            ExtraLines = root
        };

        foreach (var (trackNumber, trackData) in tracks)
        {
            var track = new CueTrack
            {
                Number = int.Parse(trackNumber),
                FileName = trackData.FileName,
                FileType = trackData.FileType,
                ExtraLines = [],
                Type = trackData.Type,
                StoredBytesPerSector = trackData.Type.ToLowerInvariant() switch
                {
                    "mode1/2048" => 2048,
                    "cdg" => 2448,
                    "mode2/2336" => 2336,
                    _ => 2352
                }
            };

            //
            // Parse lines that belong to the track.
            //

            foreach (var tokens in trackData.Lines)
            {
                switch (tokens[0].ToLowerInvariant())
                {
                    case "index":
                    {
                        if (cueTokenProcessor.ConvertMsfTimeToSector(tokens[2]) is { } msf)
                            track.Indices.Add(int.Parse(tokens[1]), msf);
                        break;
                    }
                    case "pregap":
                    {
                        if (cueTokenProcessor.ConvertMsfTimeToSector(tokens[1]) is { } msf)
                            track.Pregap = msf;
                        break;
                    }
                    case "postgap":
                    {
                        if (cueTokenProcessor.ConvertMsfTimeToSector(tokens[1]) is { } msf)
                            track.Postgap = msf;
                        break;
                    }
                    default:
                    {
                        track.ExtraLines.Add(tokens);
                        break;
                    }
                }
            }

            result.Tracks.Add(track);
        }

        return result;
    }
}