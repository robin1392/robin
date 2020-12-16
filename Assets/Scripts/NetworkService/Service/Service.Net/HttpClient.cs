using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;


namespace Service.Net
{
    class HttpResponse
    {
        public int ProtocolId { get; set; }
        public string Json { get; set; }
        public byte[] msg { get; set; }
    }


    public class HttpSendResult : ISender
    {
        string _json;


        public void Dispose()
        {
        }


        public bool SendMessage(int protocolId, byte[] buffer)
        {
            return true;
        }


        public bool SendHttpPost(int protocolId, string method, string json)
        {
            return false;
        }

        public bool SendHttpResult(string json)
        {
            _json = json;
            return true;
        }

        public string HttpResult()
        {
            return _json;
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


        public bool SendHttpPost(int protocolId, string method, string json)
        {
            RequestPostAsync(protocolId, _baseUrl + "/" + method, json);
            return true;
        }

        public bool SendHttpResult(string json)
        {
            return false;
        }

        public string HttpResult()
        {
            return string.Empty;
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
                string tempJson = ackJson;
                tempJson = tempJson.Replace("\\u0022", "\"");
                tempJson = tempJson.Replace("\\n", "");
                tempJson = tempJson.Replace("\\r", "");
                tempJson = tempJson.Replace("\\\"{", "{");
                tempJson = tempJson.Replace("}\\\"", "}");
                tempJson = tempJson.Replace("\"{", "{");
                tempJson = tempJson.Replace("}\"", "}");
                tempJson = tempJson.Replace("\\\\\\\"", "\"");
                tempJson = tempJson.Replace("\\\"", "\"");


                byte[] ackBytes = Encoding.UTF8.GetBytes(tempJson); 
                _gameSession.PushExternalMessage(
                    this,
                    protocolId + 1, 
                    ackBytes,
                    ackBytes.Length);


                // Close the stream object
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse
                response.Close();
                allDone.Set();

            }), request);

            allDone.WaitOne();
        }
    }
}