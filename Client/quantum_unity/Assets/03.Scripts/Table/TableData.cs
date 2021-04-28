using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RandomWarsResource
{
    public interface ITableData<K>
    {
        K PK();
        void Serialize(string[] s);
    }


    public class TableData<K, V>
    {
        Dictionary<K, V> _rows;

        public Dictionary<K, V> KeyValues
        {
            get { return _rows; }
        }

        public List<K> Keys 
        { 
            get
            {
                return _rows.Keys.ToList();
            }
        }

        public List<V> Values
        {
            get
            {
                return _rows.Values.ToList();
            }
        }


        public TableData()
        {
            _rows = new Dictionary<K, V>();
        }


        public bool Init(ITableLoader<K, V> loader, string sourcePath, string fileName, string targetPath = "")
        {
            return loader.Run(sourcePath, fileName, targetPath, ref _rows);
        }


        public bool GetData(K key, out V data)
        {
            if (_rows.TryGetValue(key, out data) == false)
            {
                return false;
            }

            return true;
        }


        public bool GetData(Predicate<V> match, out V values)
        {
            values = Array.Find(_rows.Values.ToArray(), match);
            if (values == null)
            {
                return false;
            }

            return true;
        }


        public bool GetData(Predicate<V> match, out V[] values)
        {
            values = Array.FindAll(_rows.Values.ToArray(), match);
            if (values == null)
            {
                return false;
            }

            return true;
        }
    }
}
