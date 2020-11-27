using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Service.Template.Resource
{
    public class ResourceData<K, V>
    {
        Dictionary<K, V> _dicValues;

        
        public bool Load(string path, string url)
        {
            if (path != null)
            {
                return LoadFromFile(path);
            }

            if (url != null)
            {
                return LoadFromBucket(url);
            }

            return false;
        }


        bool LoadFromFile(string filePath)
        {
            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText(filePath));
                _dicValues = jObject.ToObject<Dictionary<K, V>>();
            }
            catch(Exception e)
            {
                return false;
            }
            
            return true;
        }


        bool LoadFromBucket(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";


                var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var json = streamReader.ReadToEnd();
                    JObject jObject = JObject.Parse(json);
                    _dicValues = jObject.ToObject<Dictionary<K, V>>();
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        public bool GetData(K key, out V data)
        {
             if (_dicValues.TryGetValue(key, out data) == false)
            {
                return false;
            }

            return true;
        }


        public V[] GetData(Predicate<V> match)
        {
            return Array.FindAll(_dicValues.Values.ToArray(), match);
        }
    }
}
