using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIcore
{
    public class IndexList<T> : IList<T>
    {
        public IndexList(int capacity)
        {
            max_index = -1;

            if (capacity < 0)
                throw new ArgumentException("capacity must not be negative! ");

            if (capacity == 0)
                ts = null;

            ts = new T[capacity];
        }

        protected T[] ts;
        protected int max_index;
        protected const int CAPA_REDUNDANCY = 10;

        public T this[int index]
        {
            get
            {
                return ts == null || index < 0 || index >= ts.Length
                    ? throw new IndexOutOfRangeException(string.Format("index out of range: {0}", index))
                    : ts[index];
            }
            set
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException(string.Format("index out of range: {0}", index));
                }

                if (ts == null || index >= ts.Length)
                {
                    T[] tmp = new T[index + CAPA_REDUNDANCY];
                    if (ts != null && ts.Length > 0)
                    {
                        Array.Copy(ts, 0, tmp, 0, ts.Length);
                    }
                    ts = tmp;
                }

                ts[index] = value;

                if (index > max_index)
                {
                    max_index = index;
                }
            }
        }

        public int Length => max_index + 1;

        public int Count => Length;

        public bool IsReadOnly => false;

        /// <summary>
        /// always throw exception.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            if (ts != null)
            {
                ts = null;
            }
            max_index = -1;
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.Length; i++)
            {
                yield return this[i];
            }
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.Length; i++)
            {
                yield return this[i];
            }
        }
    }
}
