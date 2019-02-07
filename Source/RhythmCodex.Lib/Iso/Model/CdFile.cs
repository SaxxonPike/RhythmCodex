using System;
using System.IO;

namespace RhythmCodex.Iso.Model
{
    public class CdFile : ICdFile
    {
        private readonly Func<Stream> _openFunc;

        public CdFile(Func<Stream> openFunc)
        {
            _openFunc = openFunc;
        }
        
        public string Name { get; set; }
        public Stream Open() => _openFunc();
    }
}