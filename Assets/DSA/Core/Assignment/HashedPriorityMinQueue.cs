using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    public sealed class HashedPriorityMinQueue<T>
    {
        private struct PriorityIndex
        {
            public PriorityIndex(int index, float priority)
            {
                Index = index;
                Priority = priority;
            }
            
            public int Index;
            public readonly float Priority;

            public PriorityIndex NewIndex(int index) => new(index, 0);
        }

        private readonly struct Entry
        {
            private readonly HashedPriorityMinQueue<T> _heap;
            public Entry(HashedPriorityMinQueue<T> heap, T value)
            {
                _heap = heap;
                Value = value;
            }

            public T Value { get; }
            public float Priority => _heap._priorities[Value].Priority;
        }

        private readonly List<Entry> _heap = new();
        private readonly Dictionary<T, PriorityIndex> _priorities = new();

        public int Count => _heap.Count;

        public void Enqueue(T value, float priority)
        {
            var index = _heap.Count;
            _heap.Add(new Entry(this, value));
            _priorities.Add(value, new PriorityIndex(index, 0));
            SiftUp(_heap.Count - 1);
        }

        public T Dequeue()
        {
            if (_heap.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            var result = _heap[0];
            var last = _heap.Count - 1;

            if (last == 0)
            {
                _priorities.Remove(result.Value);
                _heap.RemoveAt(0);
                return result.Value;
            }

            _heap[0] = _heap[last];
            _priorities.Remove(result.Value);
            _priorities[_heap[0].Value] = _priorities[_heap[0].Value].NewIndex(0);
            _heap.RemoveAt(last);
            SiftDown(0);
            return result.Value;
        }

        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }
            
            return _heap[0].Value;
        }
        
        public bool ContainsKey(T key)
        {
            return _priorities.ContainsKey(key);
        }

        public float this[T entry]
        {
            get => _priorities[entry].Priority;
            set
            {
                var p = _priorities[entry];

                _priorities[entry] = new PriorityIndex(p.Index, 0);
                if (value > p.Priority)
                {
                    SiftDown(p.Index);
                }
                
                if (value < p.Priority)
                {
                    SiftUp(p.Index);
                }
            }
        }
        
        private void SiftUp(int index)
        {
            while (index > 0)
            {
                var parent = (index - 1) / 2;
                if (_heap[parent].Priority <= _heap[index].Priority)
                {
                    return;
                }

                Swap(parent, index);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                var left = 2 * index + 1;
                if (left >= _heap.Count)
                {
                    return;
                }

                var right = left + 1;
                var smallerChild = left;

                if (right < _heap.Count && _heap[right].Priority < _heap[left].Priority)
                {
                    smallerChild = right;
                }

                if (_heap[index].Priority <= _heap[smallerChild].Priority)
                {
                    return;
                }

                Swap(index, smallerChild);
                index = smallerChild;
            }
        }

        private void Swap(int a, int b)
        {
            (_heap[a], _heap[b]) = (_heap[b], _heap[a]);
            _priorities[_heap[a].Value] = _priorities[_heap[a].Value].NewIndex(a);
            _priorities[_heap[b].Value] = _priorities[_heap[b].Value].NewIndex(b);
        }
    }
}