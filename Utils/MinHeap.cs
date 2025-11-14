using System;
using System.Collections.Generic;

namespace MunicipalIssueReporter.Utils
{
    // Min-heap by a key selector (lower key = higher priority here)
    public class MinHeap<T>
    {
        private readonly List<T> _data = new List<T>();
        private readonly Func<T, int> _prioritySelector;

        public MinHeap(Func<T, int> prioritySelector) => _prioritySelector = prioritySelector;

        public int Count => _data.Count;

        public void Add(T item)
        {
            _data.Add(item);
            HeapifyUp(_data.Count - 1);
        }

        public T Peek()
        {
            if (_data.Count == 0) throw new InvalidOperationException("Heap empty");
            return _data[0];
        }

        public T Pop()
        {
            if (_data.Count == 0) throw new InvalidOperationException("Heap empty");
            var root = _data[0];
            var last = _data[_data.Count - 1];
            _data.RemoveAt(_data.Count - 1);
            if (_data.Count > 0)
            {
                _data[0] = last;
                HeapifyDown(0);
            }
            return root;
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                var p = (i - 1) / 2;
                if (_prioritySelector(_data[i]) >= _prioritySelector(_data[p])) break;
                Swap(i, p);
                i = p;
            }
        }

        private void HeapifyDown(int i)
        {
            int smallest = i;
            int l = 2 * i + 1;
            int r = 2 * i + 2;
            if (l < _data.Count && _prioritySelector(_data[l]) < _prioritySelector(_data[smallest])) smallest = l;
            if (r < _data.Count && _prioritySelector(_data[r]) < _prioritySelector(_data[smallest])) smallest = r;
            if (smallest != i)
            {
                Swap(i, smallest);
                HeapifyDown(smallest);
            }
        }

        private void Swap(int i, int j)
        {
            var tmp = _data[i];
            _data[i] = _data[j];
            _data[j] = tmp;
        }

        public IEnumerable<T> ToSortedList()
        {
            var copy = new MinHeap<T>(_prioritySelector);
            foreach (var it in _data) copy._data.Add(it);
            // fix heights by heapifying copy root properly
            for (int k = (copy._data.Count - 1) / 2; k >= 0; k--) copy.HeapifyDown(k);
            var result = new List<T>();
            while (copy._data.Count > 0) result.Add(copy.Pop());
            return result;
        }
    }
}
