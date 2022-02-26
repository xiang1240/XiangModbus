using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ModbusMaster.ByteUtils
{
    public class ByteSegementCollection : IList<ByteSegementBase>, IList
    {
        private List<ByteSegementBase> _collection;

        public ByteSegementCollection()
        {
            _collection = new List<ByteSegementBase>();
        }

        private Dictionary<string, ByteSegementBase> _dictionary;

        private Dictionary<string, ByteSegementBase> Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<string, ByteSegementBase>();
                }

                return _dictionary;
            }
        }

        public ByteSegementBase this[int index]
        {
            get
            {
                return _collection[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        object IList.this[int index]
        {
            get
            {
                return _collection[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ByteSegementBase this[string name]
        {
            get
            {
                ByteSegementBase value;
                Dictionary.TryGetValue(name, out value);

                return value;
            }
        }

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        public int Count
        {
            get
            {
                return _collection.Count;
            }
        }

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public void Add(ByteSegementBase value)
        {
            AddHelper(value);
        }

        public int Add(object value)
        {
            return AddHelper(Cast(value));
        }

        public void Clear()
        {
            _collection.Clear();
            Dictionary.Clear();
        }

        public bool Contains(object value)
        {
            return _collection.Contains(Cast(value));
        }

        public bool Contains(ByteSegementBase value)
        {
            return _collection.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (index < 0 || (index + _collection.Count) > array.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException();
            }

            try
            {
                int count = _collection.Count;
                for (int i = 0; i < count; i++)
                {
                    array.SetValue(_collection[i], index + i);
                }
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public void CopyTo(ByteSegementBase[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            _collection.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator<ByteSegementBase> IEnumerable<ByteSegementBase>.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            return _collection.IndexOf(value as ByteSegementBase);
        }

        public int IndexOf(ByteSegementBase value)
        {
            return _collection.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            InsertHelper(index, Cast(value));
        }

        public void Insert(int index, ByteSegementBase value)
        {
            InsertHelper(index, value);
        }

        public void Remove(object value)
        {
            Remove(value as ByteSegementBase);
        }

        public bool Remove(ByteSegementBase value)
        {
            value.Segements = null;

            _collection.Remove(value);

            if (!string.IsNullOrEmpty(value.Name))
            {
                Dictionary.Remove(value.Name);
            }

            return true;
        }

        public void RemoveAt(int index)
        {
            ByteSegementBase value = _collection[index];

            value.Segements = null;

            _collection.RemoveAt(index);

            if (!string.IsNullOrEmpty(value.Name))
            {
                Dictionary.Remove(value.Name);
            }
        }

        private int AddHelper(ByteSegementBase value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            value.Segements = this;

            int index = _collection.Count;
            _collection.Add(value);

            if (!string.IsNullOrEmpty(value.Name))
            {
                if (Dictionary.ContainsKey(value.Name))
                {
                    throw new ArgumentException();
                }

                Dictionary.Add(value.Name, value);
            }

            return index;
        }

        private void InsertHelper(int index, ByteSegementBase value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            value.Segements = this;

            _collection.Insert(index, value);

            if (!string.IsNullOrEmpty(value.Name))
            {
                if (Dictionary.ContainsKey(value.Name))
                {
                    throw new ArgumentException();
                }

                Dictionary.Add(value.Name, value);
            }
        }

        private ByteSegementBase Cast(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (!(value is ByteSegementBase))
            {
                throw new ArgumentException();
            }

            return (ByteSegementBase)value;
        }

        public int Length
        {
            get
            {
                int length = 0;
                foreach (ByteSegementBase segement in this)
                {
                    length += segement.Length;
                }

                return length;
            }
        }

        public int GetSegementOffset(ByteSegementBase segement)
        {
            int length = 0;
            foreach (ByteSegementBase segement1 in this)
            {
                if (segement1 == segement)
                {
                    break;
                }

                length += segement.Length;
            }

            return length;
        }

        public Byte[] ToByteArray()
        {
            byte[] result = new byte[Length];

            int offset = 0;
            foreach (ByteSegementBase segement in this)
            {
                byte[] bytes = segement.ToByteArray();
                int length = bytes.Length;
                Array.Copy(bytes, 0, result, offset, length);
                offset += length;
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (ByteSegementBase segement in this)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append(segement.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}
