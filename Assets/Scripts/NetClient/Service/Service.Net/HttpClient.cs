using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;


namespace Service.Net
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
        public byte[] msg { get; set; }
    }


    public class HttpSender : ISender
    {
        public byte[] msg { get; set; }


        public void Dispose()
        {
            msg = null;
        }


        public bool SendMessage(int protocolId, byte[] buffer)
        {
            msg = buffer;
            return true;
        }


        public bool SendMessage(int protocolId, string method, byte[] buffer)
        {
            return false;
        }

    }

    public class HttpClient : ISender
    {
        private string _baseUrl;
        private GameSessionClient _gameSession;
        private static Queue<HttpResponse> _responseQueue;
        private static ManualResetEvent allDone = new ManualResetEvent(false);



        public HttpClient(string url, GameSessionClient gameSession)
        {
            _baseUrl = url;
            _responseQueue = new Queue<HttpResponse>();
            _gameSession = gameSession;
        }


        public void Dispose()
        {
            _gameSession = null;
            _responseQueue.Clear();
            _responseQueue = null;
            allDone = null;
        }


        public bool SendMessage(int protocolId, byte[] buffer)
        {
            return false;
        }


        public bool SendMessage(int protocolId, string method, byte[] buffer)
        {
            RequestPostAsync(protocolId, _baseUrl + "/" + method, buffer);
            return true;
        }


        public void Update()
        {
            // lock (_responseQueue)
            // {
            //     if (_responseQueue.Count == 0)
            //     {
            //         return;
            //     }

            //     HttpResponse response = _responseQueue.Dequeue();
            //     _httpController.OnRecevice(response.ProtocolId, response.msg);
            // }
        }


        void RequestPostAsync(int protocolId, string url, string json)
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
                ackJson = ackJson.Replace("\\r", "");
                ackJson = ackJson.Replace("\"{", "{");
                ackJson = ackJson.Replace("}\"", "}");
                ackJson = ackJson.Replace("\\\\", "");

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


        void RequestPostAsync(int protocolId, string url, byte[] msg)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            request.ContentLength = msg.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(msg, 0, msg.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            using (dataStream = response.GetResponseStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    dataStream.CopyTo(memoryStream);
                    _gameSession.PushExternalMessage(this, protocolId + 1, memoryStream.ToArray(), (int)memoryStream.Length);
                }
            }
        }
    }
}