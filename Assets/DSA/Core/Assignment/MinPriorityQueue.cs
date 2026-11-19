using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    // Assignment Phase 3. Implement as a binary min-heap.

    /// <summary>
    /// A minimum-priority queue implemented as a binary min-heap.
    ///
    /// Items with lower integer priority values should be returned before
    /// items with higher priority values.
    ///
    /// This class is intentionally incomplete in the assignment starter.
    /// </summary>
    public sealed class MinPriorityQueue<T>
    {
        // The list stores the physical representation of the binary heap.
        // Your implementation is responsible for preserving the min-heap invariant.
        private readonly List<Entry> _heap = new List<Entry>();

        /// <summary>
        /// Number of items currently stored in the queue.
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// Adds an item with the supplied priority.
        /// Lower priority values are removed before higher priority values.
        /// </summary>
        public void Enqueue(T item, int priority)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var i = Count;
            _heap.Add(new Entry(item, priority));
            if (Count > 1)
            {
                SiftDown(i);
            }
        }

        /// <summary>
        /// Returns the minimum-priority item without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the queue is empty.
        /// </exception>
        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return _heap[0].Item;
        }

        /// <summary>
        /// Removes and returns the minimum-priority item.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the queue is empty.
        /// </exception>
        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            var item = _heap[0].Item;
            if (Count == 1)
            {
                _heap.RemoveAt(0);
                return item;
            }
            
            var maxIndex = Count - 1;
            (_heap[0], _heap[maxIndex]) = (_heap[maxIndex], _heap[0]);
            _heap.RemoveAt(maxIndex);
            SiftUp(0);
            return item;
        }

        /// <summary>
        /// Stores one item together with the priority used by the heap.
        /// </summary>
        private readonly struct Entry
        {
            public Entry(T item, int priority)
            {
                Item = item;
                Priority = priority;
            }

            public T Item { get; }
            public int Priority { get; }
        }
        
        private int ParentIndex(int index) => (index - 1) / 2;
        private int LeftIndex(int index) => index * 2 + 1;
        
        private void Swap(int a, int b)
        {
            (_heap[a], _heap[b]) = (_heap[b], _heap[a]);
        }

        private void SiftDown(int i)
        {
            var priority = _heap[i].Priority;
            var pi = ParentIndex(i);
            while (priority < _heap[pi].Priority)
            {
                Swap(i, pi);
                i = pi;
                pi = ParentIndex(i);
            }
        }

        private void SiftUp(int index)
        {
            var priority = _heap[index].Priority;

            while (true)
            {
                var leftIndex = LeftIndex(index);
                var rightIndex = leftIndex + 1;
                var swapIndex = leftIndex;
                if (rightIndex < Count)
                {
                    swapIndex = _heap[leftIndex].Priority < _heap[rightIndex].Priority ? leftIndex : rightIndex;
                }
                else if (leftIndex >= Count || priority < _heap[swapIndex].Priority)
                {
                    return;
                }
                
                Swap(swapIndex, index);
                index = swapIndex;
            }
        }

        public void Clear()
        {
            _heap.Clear();
        }
    }
}
