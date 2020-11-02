using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Linq;

namespace RandomWarsService.Network.Http
{
    class HttpRequest
    {
        public int ProtocolId { get; set; }
        public string Url { get; set; }
        public string Json { get; set; }
    }

    class HttpResponse
    {
        public int ProtocolId { get; set; }
        public string Json { get; set; }
    }


    public class HttpClient
    {
        private string _baseUrl;
        private Queue<HttpResponse> _responseQueue;
        private IHttpReceiver _httpReceiver;


        public HttpClient(string url, IHttpReceiver httpReceiver)
        {
            _baseUrl = url;
            _responseQueue = new Queue<HttpResponse>();
            _httpReceiver = httpReceiver;
        }


        public void Send(int protocolId, string methodUrl, string json)
        {
            HttpRequestPostAsync(protocolId, _baseUrl + "/" + methodUrl, json);
        }


        public void Update()
        {
            lock (_responseQueue)
            {
                if (_responseQueue.Count == 0)
                {
                    return;
                }

                HttpResponse response = _responseQueue.Dequeue();
                _httpReceiver.Process(response.ProtocolId, response.Json);
            }
        }


        void HttpRequestPostAsync(int protocolId, string url, string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpResponseAsync(request, (response) =>
            {
                var ackJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
                ackJson = ackJson.Replace("\\u0022", "\"");
                ackJson = ackJson.Replace("\\n", "");
                ackJson = ackJson.Replace("\"{", "{");
                ackJson = ackJson.Replace("}\"", "}");

                lock (_responseQueue)
                {
                    _responseQueue.Enqueue(new HttpResponse
                    {
                        ProtocolId = protocolId + 1,
                        Json = ackJson
                    });
                }
            });
        }

        void HttpResponseAsync(HttpWebRequest request, Action<HttpWebResponse> responseAction)
        {
            Action wrapperAction = () =>
            {
                request.BeginGetResponse(new AsyncCallback((iar) =>
                {
                    var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                    responseAction(response);
                }), request);
            };
            wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
            {
                var action = (Action)iar.AsyncState;
                action.EndInvoke(iar);
            }), wrapperAction);
        }


        void RequestPost(int protocolId, string url, string json)
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
                ackJson = ackJson.Replace("\\u0022", "\"");
                ackJson = ackJson.Replace("\\n", "");
                ackJson = ackJson.Replace("\"{", "{");
                ackJson = ackJson.Replace("}\"", "}");
                _httpReceiver.Process(protocolId + 1, ackJson);
            }
        }

    }
}
