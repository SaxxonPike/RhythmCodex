using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Games.Beatmania.Ps2.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Services;

/// <inheritdoc />
[Service]
public sealed class BeatmaniaPs2Service(
    IBeatmaniaPs2FormatDatabase formatDatabase,
    IBeatmaniaPs2DataFileInfoDecoder dataFileInfoDecoder,
    IBeatmaniaPs2SongInfoDecoder songInfoDecoder,
    IBeatmaniaPs2NewBgmStreamReader newBgmStreamReader,
    IBeatmaniaPs2NewChartStreamReader newChartStreamReader,
    IBeatmaniaPs2NewKeysoundStreamReader newKeysoundStreamReader,
    IBeatmaniaPs2OldBgmStreamReader oldBgmStreamReader,
    IBeatmaniaPs2OldChartStreamReader oldChartStreamReader,
    IBeatmaniaPs2OldKeysoundStreamReader oldKeysoundStreamReader,
    IBeatmaniaPs2KeysoundDecoder keysoundDecoder,
    IBeatmaniaPs2BgmDecoder bgmDecoder,
    IBeatmaniaPs2ChartConverter chartConverter,
    IBeatmaniaPs2OldChartDecoder oldChartDecoder,
    IBeatmaniaPs2NewChartDecoder newChartDecoder)
    : IBeatmaniaPs2Service
{
    /// <inheritdoc />
    public IEnumerable<BeatmaniaPs2ChartSet> Decode(Func<string, Stream> openFile, BeatmaniaPs2FormatType type)
    {
        if (formatDatabase.GetFormatByType(type) is not { } formatInfo)
            throw new RhythmCodexException($"Unknown format: {type}");

        //
        // Cache the contents of all executable/list files.
        //

        var exeFiles = formatInfo.MetaTables
            .Select(x => x.BinaryFileName!)
            .Distinct()
            .ToDictionary(x => x, x =>
            {
                using var fileStream = openFile(x);
                var bytes = new byte[fileStream.Length];
                fileStream.ReadExactly(bytes);
                return bytes;
            });

        //
        // Cache file tables.
        //

        var masterFileTable = formatInfo.MetaTables
            .SelectMany(x => dataFileInfoDecoder
                .Decode(exeFiles[x.BinaryFileName!], (int)x.FileTableOffset, type)
                .Index()
                .Select(y => (
                    Index: y.Index + x.BaseIndex,
                    Offset: y.Item.Offset + x.BlobOffset,
                    y.Item.Size,
                    File: x.BlobFileName!)))
            .ToDictionary(x => x.Index, x => (x.Offset, x.Size, x.File));

        //
        // Cache song tables.
        //

        var masterSongTable = formatInfo.MetaTables
            .Select(x => (x.BinaryFileName, x.SongTableOffset, x.BaseSongIndex))
            .Distinct()
            .SelectMany(x => songInfoDecoder
                .Decode(exeFiles[x.BinaryFileName!], (int)x.SongTableOffset, type)
                .Index()
                .Select(y => y with { Index = y.Index + x.BaseSongIndex }))
            .ToDictionary(x => x.Index, x => x.Item);

        //
        // Process songs.
        //

        foreach (var (songId, songInfo) in masterSongTable)
        {
            var chartSet = new BeatmaniaPs2ChartSet
            {
                SongId = songId,
                Name = songInfo.Name
            };

            //
            // Create lookup tables for the song data.
            //

            var bgmMap = songInfo.Difficulties
                .Select(x => x.Bgm)
                .Index()
                .Where(x => x.Item >= 0)
                .DistinctBy(x => x.Item)
                .ToDictionary(x => x.Item, x => (
                    SetId: x.Index,
                    Data: ReadBgm(new MemoryStream(ReadFromBlob(x.Item)), formatInfo.UseOldReaders)));

            var keyMap = songInfo.Difficulties
                .Select(x => x.Keysounds)
                .Index()
                .Where(x => x.Item >= 0)
                .DistinctBy(x => x.Item)
                .ToDictionary(x => x.Item, x => (
                    SetId: x.Index,
                    Data: ReadKeysounds(new MemoryStream(ReadFromBlob(x.Item)), formatInfo.UseOldReaders, songId)));

            var chartMap = songInfo.Difficulties
                .Select(x => (x.ChartId, x.Chart, x.Bgm, x.Keysounds, x.Level, x.Name))
                .Index()
                .Where(x => x.Item.ChartId >= 0)
                .ToDictionary(x => x.Index, x => (
                    ChartId: x.Item,
                    Chart: x.Item.Chart ?? ReadChart(new MemoryStream(ReadFromBlob(x.Item.ChartId)),
                        formatInfo.UseOldReaders),
                    x.Item.Name,
                    x.Item.Level,
                    Bgm: x.Item.Bgm < 0 ? bgmMap.First().Value.SetId : bgmMap[x.Item.Bgm].SetId,
                    Keysounds: x.Item.Keysounds < 0 ? keyMap.First().Value.SetId : keyMap[x.Item.Keysounds].SetId));

            //
            // Populate the chart set.
            //

            foreach (var (chartIndex, chartInfo) in chartMap)
            {
                //
                // Maps identify which keysound set and BGM to use for a chart.
                //

                chartSet.ChartMaps[chartIndex] = (
                    BgmId: chartInfo.Bgm,
                    KeysoundId: chartInfo.Keysounds
                );

                chartInfo.Chart[NumericData.Id] = chartIndex;
                chartInfo.Chart[NumericData.SourceIndex] = chartInfo.ChartId.ChartId;
                chartInfo.Chart[NumericData.PlayLevel] = chartInfo.Level;
                chartInfo.Chart[StringData.Description] = chartInfo.Name;
                chartInfo.Chart[StringData.Title] = songInfo.Name;
                chartSet.Charts[chartIndex] = chartInfo.Chart;
            }

            foreach (var (_, bgmInfo) in bgmMap.Where(x => x.Value.Data != null))
                chartSet.Bgms[bgmInfo.SetId] = bgmInfo.Data!;

            foreach (var (_, keyInfo) in keyMap)
                chartSet.Keysounds[keyInfo.SetId] = keyInfo.Data.Select(x => x.Value).ToList()!;

            yield return chartSet;
        }

        yield break;

        //
        // Reads a file from a blob.
        //

        byte[] ReadFromBlob(int fileId)
        {
            if (fileId < 0 || fileId >= masterFileTable.Count)
                return [];
            var info = masterFileTable[fileId];
            using var blobStream = openFile(info.File);
            var data = new byte[info.Size];
            blobStream.Position = info.Offset;
            blobStream.ReadExactly(data);
            return data;
        }

        //
        // Reads a BGM block.
        //

        Sound? ReadBgm(Stream stream, bool isOld)
        {
            var data = isOld
                ? oldBgmStreamReader.Read(stream)
                : newBgmStreamReader.Read(stream);

            var bgm = data != null ? bgmDecoder.Decode(data) : null;

            return bgm;
        }

        //
        // Reads a keysound block.
        //

        Dictionary<int, Sound?> ReadKeysounds(Stream stream, bool isOld, int songId)
        {
            var keysounds = (isOld
                ? oldKeysoundStreamReader.Read(stream)
                : newKeysoundStreamReader.Read(stream)).Keysounds;

            return keysounds.ToDictionary(x => x.Index, x =>
            {
                var decoded = keysoundDecoder.Decode(x);
                decoded?[NumericData.Id] = x.Index;
                return decoded;
            });
        }

        //
        // Reads a chart block.
        //

        Chart ReadChart(Stream stream, bool isOld)
        {
            var chartData = isOld
                ? oldChartStreamReader.Read(stream, stream.Length)
                : newChartStreamReader.Read(stream, stream.Length);

            var decodedData = isOld
                ? oldChartDecoder.Decode(chartData.Span)
                : newChartDecoder.Decode(chartData.Span);

            return chartConverter.Convert(decodedData);
        }
    }
}