using System;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    public sealed class InputFile : IDisposable
    {
        private readonly Func<Stream> _open;
        private readonly Action<Stream> _dispose;
        private Stream _openedStream;

        internal InputFile(string name, Func<Stream> open, Action<Stream> dispose)
        {
            Name = name;
            _open = open;
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

        public void Dispose()
        {
            if (_openedStream != null)
                _dispose?.Invoke(_openedStream);
            _openedStream = null;
        }
    }
}