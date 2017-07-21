using System;
using Moq;
using Moqzilla;
using Ploeh.AutoFixture;

namespace RhythmCodex
{
    public class BaseTestFixture
    {
        private readonly Lazy<Fixture> _fixture = new Lazy<Fixture>(() => new Fixture());
        private readonly Lazy<Mocker> _mocker = new Lazy<Mocker>(() => new Mocker());

        protected Fixture Fixture => _fixture.Value;
        protected Mocker Mocker => _mocker.Value;

        protected Mock<TMock> Mock<TMock>() 
            where TMock : class 
            => Mocker.Mock<TMock>();
    }

    public class BaseTestFixture<TSubject> : BaseTestFixture
        where TSubject : class
    {
        private readonly Lazy<TSubject> _subject;

        protected TSubject Subject => _subject.Value;
        
        public BaseTestFixture() 
            => _subject = new Lazy<TSubject>(() => Mocker.Create<TSubject>());
    }
}
