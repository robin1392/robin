using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;


namespace RandomWarsResource
{
    public interface ITableLoader<K, V>
    {
        bool Run(string sourcePath, string fileName, string targetPath, ref Dictionary<K, V> outValues);
    }


    public class TableLoaderLocalCSV<K, V> : ITableLoader<K, V>
        where V : ITableData<K>, new()
    {
        public bool Run(string sourcePath, string fileName, string targetPath, ref Dictionary<K, V> outValues)
        {
            Debug.Log($"file: {fileName}");
            var path = sourcePath + "/" + fileName;

            int row = 0;
            List<int> exceptIndex = new List<int>();
            var lines = System.IO.File.ReadAllLines(path);
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
                if (outValues.ContainsKey(key) == true)
                {
                    return false;
                }

                outValues.Add(key, col);
            }
           
            return true;
        }
    }


    public class TableLoaderRemoteCSV<K, V> : ITableLoader<K, V>
        where V : ITableData<K>, new()
    {
        public bool Run(string sourcePath, string fileName, string targetPath, ref Dictionary<K, V> outValues)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sourcePath + "/" + fileName);
            request.ContentType = "application/json";
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();
            var resStream = response.GetResponseStream();

            StreamReader reader;
            if (targetPath.Length != 0)
            {
                if (Directory.Exists(targetPath) == false)
                {
                    Directory.CreateDirectory(targetPath);
                }

                using (var fileStream = new FileStream(targetPath + "/" + fileName, FileMode.Create, FileAccess.Write))
                {
                    resStream.CopyTo(fileStream);
                }

                reader = new StreamReader(targetPath + "/" + fileName);
            }
            else
            {
                reader = new StreamReader(resStream);
            }


            List<int> exceptIndex = new List<int>();
            int row = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
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
                if (outValues.ContainsKey(key) == true)
                {
                    return false;
                }

                outValues.Add(key, col);
            }

            reader.Close();
            return true;
        }
    }

}