using System;
using Moq;
using Moqzilla;

namespace RhythmCodex
{
    /// <summary>
    ///     Base test fixture for all unit tests that use mocking.
    /// </summary>
    public abstract class BaseUnitTestFixture : BaseTestFixture
    {
        private readonly Lazy<Mocker> _mocker = new Lazy<Mocker>(() => new Mocker());

        protected Mocker Mocker => _mocker.Value;

        protected Mock<TMock> Mock<TMock>()
            where TMock : class
        {
            return Mocker.Mock<TMock>();
        }

        protected Mock<TMock> Mock<TMock>(Action<Mock<TMock>> func)
            where TMock : class
        {
            return Mocker.Mock(func);
        }
    }

    /// <summary>
    ///     Base unit test fixture which exposes a pre-mocked test subject only as its interface.
    ///     This will reveal missing/unused methods during testing.
    /// </summary>
    public abstract class BaseUnitTestFixture<TSubject, TInterface> : BaseUnitTestFixture
        where TSubject : class, TInterface
        where TInterface : class
    {
        private readonly Lazy<TInterface> _subject;

        protected BaseUnitTestFixture()
        {
            _subject = new Lazy<TInterface>(() => Mocker.Create<TSubject>());
        }

        protected TInterface Subject => _subject.Value;
    }

    /// <summary>
    ///     Base unit test fixture which exposes a pre-mocked test subject.
    /// </summary>
    /// <typeparam name="TSubject"></typeparam>
    public abstract class BaseUnitTestFixture<TSubject> : BaseUnitTestFixture<TSubject, TSubject>
        where TSubject : class
    {
    }
}