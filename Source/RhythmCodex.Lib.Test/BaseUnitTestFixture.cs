using System;
using Moq;
using Moqzilla;

namespace RhythmCodex
{
    public class BaseUnitTestFixture : BaseTestFixture
    {
        private readonly Lazy<Mocker> _mocker = new Lazy<Mocker>(() => new Mocker());

        protected Mocker Mocker => _mocker.Value;

        protected Mock<TMock> Mock<TMock>() 
            where TMock : class 
            => Mocker.Mock<TMock>();

        protected Mock<TMock> Mock<TMock>(Action<Mock<TMock>> func)
            where TMock : class
            => Mocker.Mock(func);
    }

    public class BaseUnitTestFixture<TSubject> : BaseUnitTestFixture
        where TSubject : class
    {
        private readonly Lazy<TSubject> _subject;

        protected TSubject Subject => _subject.Value;
        
        public BaseUnitTestFixture() 
            => _subject = new Lazy<TSubject>(() => Mocker.Create<TSubject>());
    }
}
