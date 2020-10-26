using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace RandomWarsService.Network.Http
{
    class HttpRequest
    {
        public int ProtocolId { get; set; }
        public string Url { get; set; }
        public string Json { get; set; }
    }


    public class HttpClient
    {
        private string _baseUrl;
        private Queue<HttpRequest> _requestQueue;
        private IHttpReceiver _httpReceiver;


        public HttpClient(string url, IHttpReceiver httpReceiver)
        {
            _baseUrl = url;
            _requestQueue = new Queue<HttpRequest>();
            _httpReceiver = httpReceiver;
        }


        public void Send(int protocolId, string methodUrl, string json)
        {
            lock (_requestQueue)
            {
                _requestQueue.Enqueue(new HttpRequest
                {
                    ProtocolId = protocolId,
                    Url = _baseUrl + "/" + methodUrl,
                    Json = json,
                });
            }
        }


        public void Update()
        {
            lock (_requestQueue)
            {
                if (_requestQueue.Count == 0)
                {
                    return;
                }

                HttpRequest request = _requestQueue.Peek();
                if (RequestPost(request.ProtocolId, request.Url, request.Json) == true)
                {
                    _requestQueue.Dequeue();
                }
            }
        }


        bool RequestPost(int protocolId, string url, string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var ackJson = streamReader.ReadToEnd();
                _httpReceiver.ProcessResponse(protocolId + 1, ackJson);
            }

            return true;
        }

    }
}
