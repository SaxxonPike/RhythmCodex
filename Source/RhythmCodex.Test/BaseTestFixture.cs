using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using AutoFixture;
using AutoFixture.Dsl;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using RhythmCodex.Data;

namespace RhythmCodex;

/// <summary>
///     Contains features available to all tests.
/// </summary>
[Parallelizable(ParallelScope.All)]
[PublicAPI]
public abstract class BaseTestFixture
{
    private static readonly ConcurrentDictionary<string, Fixture> Fixtures = [];
    private static readonly ConcurrentDictionary<string, HashSet<Task>> AsyncTasks = [];

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
        if (AsyncTasks.Remove(TestContext.CurrentContext.Test.ID, out var tasks) && tasks.Count > 0)
        {
            Task.WaitAll(tasks);
            var taskExceptions = tasks.Where(t => t.IsFaulted).Select(t => t.Exception!).ToList();
            if (taskExceptions.Count > 0)
                throw new AggregateException(taskExceptions);
        }

        _stopwatch.Stop();
        Log.WriteLine(
            $"{TestContext.CurrentContext.Test.FullName}: {_stopwatch.ElapsedMilliseconds}ms");
        Fixtures.Remove(TestContext.CurrentContext.Test.ID, out _);
    }

    public static Randomizer Random =>
        TestContext.CurrentContext.Random;

    public static TextWriter Log =>
        TestContext.Progress;

    public bool IsCanceled =>
        TestContext.CurrentContext.CancellationToken.IsCancellationRequested;

    private HashSet<Task> GetAsyncTasks() =>
        AsyncTasks.GetOrAdd(TestContext.CurrentContext.Test.ID, _ => []);

    private Fixture GetFixture() =>
        Fixtures.GetOrAdd(TestContext.CurrentContext.Test.ID, _ =>
        {
            var fixture = new Fixture();
            new SupportMutableValueTypesCustomization().Customize(fixture);
            return fixture;
        });

    protected void RunAsync(Action action)
    {
        GetAsyncTasks().Add(Task.Run(action));
    }

    /// <summary>
    ///     Retrieves an embedded resource by name.
    /// </summary>
    protected static byte[] GetEmbeddedResource(string name)
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
    protected static Dictionary<string, byte[]> GetArchiveResource(string name)
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
        return GetFixture().Build<T>();
    }

    /// <summary>
    ///     Creates an object of the specified type with randomized properties.
    /// </summary>
    protected T Create<T>()
    {
        return GetFixture().Create<T>();
    }

    /// <summary>
    ///     Creates many objects of the specified type all with randomized properties.
    /// </summary>
    protected T[] CreateMany<T>()
    {
        return GetFixture().CreateMany<T>().ToArray();
    }

    /// <summary>
    ///     Creates a specified number of objects of the specified type all with randomized properties.
    /// </summary>
    protected T[] CreateMany<T>(int count)
    {
        return GetFixture().CreateMany<T>(count).ToArray();
    }

    /// <summary>
    ///     Chooses one item at random from the specified set.
    /// </summary>
    protected static T OneOf<T>(IReadOnlyList<T> items)
    {
        return items[TestContext.CurrentContext.Random.Next(items.Count)];
    }

    /// <summary>
    ///     Chooses one item at random from the specified set.
    /// </summary>
    protected static T OneOf<T>(IEnumerable<T> items)
    {
        return OneOf(items.ToArray());
    }

    /// <summary>
    ///     Chooses one item at random from the specified set.
    /// </summary>
    protected static T[] ManyOf<T>(IReadOnlyList<T> items, int count, bool unique = false)
    {
        var result = new T[count];
        for (var i = 0; i < count; i++)
        {
            var item = items[TestContext.CurrentContext.Random.Next(items.Count)];
            if (!unique || !result.Contains(item))
                result[i] = item;
        }

        return result;
    }

    /// <summary>
    ///     Chooses one item at random from the specified set.
    /// </summary>
    protected static T[] ManyOf<T>(IEnumerable<T> items, int count, bool unique = false)
    {
        return ManyOf(items.ToArray(), count, unique);
    }
}