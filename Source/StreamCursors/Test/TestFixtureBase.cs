using NUnit.Framework.Internal;

namespace Saxxon.StreamCursors;

[Parallelizable(ParallelScope.All)]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class TestFixtureBase
{
    protected static Randomizer Random => TestContext.CurrentContext.Random;
}