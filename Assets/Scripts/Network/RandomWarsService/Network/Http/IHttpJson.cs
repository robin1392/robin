using System;
using System.Collections.Generic;
using System.Text;

namespace RandomWarsService.Network.Http
{
    public interface IJsonSerializer
    {
        string SerializeObject<T>(T jObject);

        T DeserializeObject<T>(string json);

        T[] DeserializeObjectArray<T>(string json);

    }


    public class JsonObject
    {
        public Dictionary<string, object> Document { get; set;  }

        public JsonObject()
        {
            Document = new Dictionary<string, object>();
        }


        public void Add(string field, object value)
        {
            Document.Add(field, value);
        }

        public void Delete(string field)
        {
            Document.Remove(field);
        }

    }
}
