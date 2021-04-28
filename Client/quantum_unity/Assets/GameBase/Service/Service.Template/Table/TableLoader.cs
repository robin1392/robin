using System.Collections.Generic;
using System.IO;
using System.Net;


namespace Service.Template.Table
{
    public interface ITableLoader<K, V>
    {
        bool Run(string path, ref Dictionary<K, V> outValues);
    }


    public class TableLoaderLocalCSV<K, V> : ITableLoader<K, V>
        where V : ITableData<K>, new()
    {
        public bool Run(string path, ref Dictionary<K, V> outValues)
        {
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    V col = new V();
                    col.Serialize(line.Split(','));

                    K key = col.PK();
                    if (outValues.ContainsKey(key) == true)
                    {
                        return false;
                    }

                    outValues.Add(key, col);
                }
            }

            return true;
        }
    }


    public class TableLoaderRemoteCSV<K, V> : ITableLoader<K, V>
        where V : ITableData<K>, new()
    {
        public bool Run(string path, ref Dictionary<K, V> outValues)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            request.ContentType = "application/json";
            request.Method = "GET";

            List<int> exceptIndex = new List<int>();
            var response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                int row = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
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
                    if (outValues.ContainsKey(key) == true)
                    {
                        return false;
                    }

                    outValues.Add(key, col);
                }
            }

            return true;
        }
    }

}