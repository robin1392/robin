using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RandomWarsResource
{
    public interface ITableData<K>
    {
        K PK();
        void Serialize(string[] s);
    }


    public class TableData<K, V> where V : ITableData<K>, new()
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
        
        public bool Init(string text)
        {
            _rows.Clear();
            
            int row = 0;
            List<int> exceptIndex = new List<int>();
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var l in lines)
            {
                var line = l;
                int replaceIndex = line.IndexOf("\"");
                while (replaceIndex != -1)
                {
                    var originText = line.Substring(replaceIndex, line.IndexOf("\"", replaceIndex + 1) - replaceIndex + ("\"").Length);
                    var replaceText = originText.Replace(",", "{$}");
                    replaceText = replaceText.Replace("\"", "");
                    line = line.Replace(originText, replaceText);
                    replaceIndex = line.IndexOf("\"");
                };

                List<string> cols = new List<string>(line.Split(','));
                if (row == 0)
                {
                    for (int i = cols.Count - 1; i >= 0; i--)
                    {
                        if (cols[i].StartsWith("~") == true)
                        {
                            exceptIndex.Add(i);
                        }
                    }
                }

                if (row++ < 2)
                {
                    continue;
                }

                foreach (var index in exceptIndex)
                {
                    cols.RemoveAt(index);
                }

                V col = new V();
                col.Serialize(cols.ToArray());

                K key = col.PK();
                if (_rows.ContainsKey(key) == true)
                {
                    return false;
                }

                _rows.Add(key, col);
            }
           
            return true;
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
