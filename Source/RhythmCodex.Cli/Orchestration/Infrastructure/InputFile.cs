using System;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    public sealed class InputFile : IDisposable
    {
        private readonly Func<Stream> _open;
        private readonly Func<string, Stream> _openRelated;
        private readonly Action<Stream> _dispose;
        private Stream _openedStream;

        internal InputFile(string name, Func<Stream> open, Func<string, Stream> openRelated, Action<Stream> dispose)
        {
            Name = name;
            _open = open;
            _openRelated = openRelated;
            _dispose = dispose;
        }

        public string Name { get; }

        public Stream Open()
        {
            if (_openedStream != null)
                throw new RhythmCodexException("Input file already opened.");
            _openedStream = _open();
            return _openedStream;
        }

        public Stream OpenRelated(string name)
        {
            return _openRelated(name);
        }

        public void Dispose()
        {
            if (_openedStream != null)
                _dispose?.Invoke(_openedStream);
            _openedStream = null;
        }
    }
}