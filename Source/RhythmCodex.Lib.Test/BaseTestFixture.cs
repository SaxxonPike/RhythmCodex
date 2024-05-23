﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using NUnit.Framework;
using RhythmCodex.Data;

namespace RhythmCodex;

/// <summary>
///     Contains features available to all tests.
/// </summary>
public abstract class BaseTestFixture
{
    private readonly Lazy<Fixture> _fixture = new(() =>
    {
        var fixture = new Fixture();
        new SupportMutableValueTypesCustomization().Customize(fixture);
        return fixture;
    });

    private Stopwatch _stopwatch;

    [SetUp]
    public void __Setup()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    [TearDown]
    public void __Teardown()
    {
        _stopwatch.Stop();
        TestContext.Out.WriteLine(
            $"{TestContext.CurrentContext.Test.FullName}: {_stopwatch.ElapsedMilliseconds}ms");
    }

    /// <summary>
    ///     Retrieves an embedded resource by name.
    /// </summary>
    protected byte[] GetEmbeddedResource(string name)
    {
        using var stream =
            typeof(TestDataBeacon).Assembly.GetManifestResourceStream($"RhythmCodex.Data.{name}");
        using var mem = new MemoryStream();
        if (stream == null)
            throw new IOException($"Embedded resource {name} was not found.");

        stream.CopyTo(mem);
        return mem.ToArray();
    }

    /// <summary>
    ///     Retrieves an embedded resource by name as an archive and extracts the first file from it.
    /// </summary>
    protected IDictionary<string, byte[]> GetArchiveResource(string name)
    {
        var output = new Dictionary<string, byte[]>();

        using var mem = new MemoryStream(GetEmbeddedResource(name));
        using var archive = new ZipArchive(mem, ZipArchiveMode.Read, false);

        foreach (var entry in archive.Entries)
        {
            using var entryStream = entry.Open();
            using var entryCopy = new MemoryStream();

            entryStream.CopyTo(entryCopy);
            entryCopy.Flush();
            output[entry.FullName] = entryCopy.ToArray();
        }

        return output;
    }

    /// <summary>
    ///     Gets an AutoFixture builder which can be used to customize a created object.
    /// </summary>
    protected ICustomizationComposer<T> Build<T>()
    {
        return _fixture.Value.Build<T>();
    }

    /// <summary>
    ///     Creates an object of the specified type with randomized properties.
    /// </summary>
    protected T Create<T>()
    {
        return _fixture.Value.Create<T>();
    }

    /// <summary>
    ///     Creates many objects of the specified type all with randomized properties.
    /// </summary>
    protected T[] CreateMany<T>()
    {
        return _fixture.Value.CreateMany<T>().ToArray();
    }

    /// <summary>
    ///     Creates a specified number of objects of the specified type all with randomized properties.
    /// </summary>
    protected T[] CreateMany<T>(int count)
    {
        return _fixture.Value.CreateMany<T>(count).ToArray();
    }

    /// <summary>
    ///     Chooses one item at random from the specified set.
    /// </summary>
    protected T OneOf<T>(IReadOnlyList<T> items)
    {
        return items[TestContext.CurrentContext.Random.Next(items.Count)];
    }

    /// <summary>
    ///     Chooses one item at random from the specified set.
    /// </summary>
    protected T OneOf<T>(IEnumerable<T> items)
    {
        return OneOf(items.ToArray());
    }
    
    /// <summary>
    ///     Chooses one item at random from the specified set.
    /// </summary>
    protected T[] ManyOf<T>(IReadOnlyList<T> items, int count)
    {
        var result = new T[count];
        for (var i = 0; i < count; i++)
            result[i] = items[TestContext.CurrentContext.Random.Next(items.Count)];
        return result;
    }
    
    /// <summary>
    ///     Chooses one item at random from the specified set.
    /// </summary>
    protected T[] ManyOf<T>(IEnumerable<T> items, int count)
    {
        return ManyOf(items.ToArray(), count);
    }
}