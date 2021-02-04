using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

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
        private IHttpReceiver _httpReceiver;
        private static Queue<HttpResponse> _responseQueue;
        private static ManualResetEvent allDone = new ManualResetEvent(false);

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


            request.BeginGetResponse(new AsyncCallback((asynchronousResult) =>
            {
                // End the operation
                HttpWebResponse response = (HttpWebResponse)((HttpWebRequest)asynchronousResult.AsyncState).EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);

                string ackJson = streamRead.ReadToEnd();
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


                // Close the stream object
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse
                response.Close();
                allDone.Set();

            }), request);

            allDone.WaitOne();
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
