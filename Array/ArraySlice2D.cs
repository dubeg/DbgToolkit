using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbgToolkit.Array {
    public static class ArraySliceExt {
        public static ArraySlice2D<T> Slice<T>(this T[,] arr, int firstDimIdx) {
            return new ArraySlice2D<T>(arr, firstDimIdx);
        }

        public static ArraySlice2D<T> Slice<T>(this T[,] arr, int firstDimIdx, int length) {
            return new ArraySlice2D<T>(arr, firstDimIdx, length);
        }
    }

    public class ArraySlice2D<T> : IEnumerator<T>, IEnumerable<T> {
        private readonly T[,] _arr;
        private readonly int _firstDimIdx;
        private readonly int _length;
        private readonly int _lowerBound;
        private readonly int _upperBound;
        private int _position;
        public int Length { get { return _length; } }

        public ArraySlice2D(T[,] arr, int firstDimIdx, int? length = null) {
            _arr = arr;
            _firstDimIdx = firstDimIdx;
            _lowerBound = arr.GetLowerBound(1);
            _upperBound = arr.GetUpperBound(1);
            _length = arr.GetLength(1);
            if (length.HasValue) {
                if (length <= 0) throw new ArgumentException("Parameter Length must be higher than zero.", nameof(length));
                if (length > _length) throw new ArgumentException("Parameter value is higher than the actual dimension length.", nameof(length));
                _length = length.Value;
            }
        }

        public T this[int index] {
            get { return _arr[_firstDimIdx, index]; }
            set { _arr[_firstDimIdx, index] = value; }
        }

        public T[] ToArray() {
            var arr = new T[_length];
            for (int i = 0; i < _length; i += 1)
                arr[i] = _arr[_firstDimIdx, _lowerBound + i];
            return arr;
        }

        public void Dispose() { }

        public bool MoveNext() {
            _position += 1;
            return _position <= _upperBound;
        }

        public void Reset() {
            _position = _lowerBound;
        }

        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public T Current => _arr[_firstDimIdx, _position];

        object IEnumerator.Current => Current;
    }
}
