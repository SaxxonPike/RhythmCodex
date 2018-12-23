using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Infrastructure.Converters
{
    public sealed class MemoryMap<T> : IReadOnlyList<T>
    {
        private class MemoryMapEnumerator : IEnumerator<T>
        {
            private readonly IEnumerable<Memory<T>> _memories;
            private int _offset;

            public MemoryMapEnumerator(IEnumerable<Memory<T>> memories)
            {
                _memories = memories;
            }
            
            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public T Current { get; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
        
        private readonly IEnumerable<Memory<T>> _memories;

        public MemoryMap()
        {
            _memories = Enumerable.Empty<Memory<T>>();
            Count = 0;
        }

        public MemoryMap(Memory<T> memory)
        {
            _memories = new[] {memory};
            Count = memory.Length;
        }
        
        public MemoryMap(IEnumerable<Memory<T>> memories)
        {
            _memories = memories;
            Count = _memories.Sum(m => m.Length);
        }

        private IEnumerable<T> EnumerateMemory()
        {
            foreach (var memory in _memories)
            {
                var span = memory.Span;
                for (var i = 0; i < memory.Length; i++)
                    yield return span[i];
            }
        }

        private T GetItem(int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException();

            foreach (var memory in _memories)
            {
                if (memory.Length < index)
                    return memory.Span[index];
                index -= memory.Length;
            }
            
            throw new IndexOutOfRangeException();
        }

        public IEnumerator<T> GetEnumerator() => EnumerateMemory().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count { get; }
        public T this[int index] => GetItem(index);

        public T[] ToArray()
        {
            var buffer = new T[Count];
            var index = 0;
            foreach (var memory in _memories)
                memory.CopyTo(buffer.AsMemory(index, memory.Length));
            return buffer;
        }

        public Span<T> ToSpan() => ToArray().AsSpan();
        public Memory<T> ToMemory() => ToArray().AsMemory();

        public MemoryMap<T> Slice(int offset, int length)
        {
            var output = new List<Memory<T>>();
            var offset2 = offset;
            var eligibleMemories = _memories.SkipWhile(memory =>
            {
                if (memory.Length < offset2)
                    return false;
                offset2 -= memory.Length;
                return true;
            });
            
            foreach (var memory in eligibleMemories)
            {
                var sliceEnd = offset + length;
                if (memory.Length >= sliceEnd)
                {
                    output.Add(memory.Slice(offset, length));
                    return new MemoryMap<T>(output);
                }

                var newSlice = memory.Slice(offset);
                output.Add(newSlice);
                offset = 0;
                length -= newSlice.Length;
            }
            
            return new MemoryMap<T>();
        }

        public MemoryMap<T> Concat(params Memory<T>[] memories) => 
            new MemoryMap<T>(_memories.Concat(memories));
    }
}