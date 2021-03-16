using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using Service.Core;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


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


        public void SetAccessToken(string accessToken)
        {

        }


        public string GetAccessToken()
        {
            return string.Empty;
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
        private string _accessToken;
        private string _baseUrl;
        private GameSessionClient _gameSession;
        private static Queue<HttpResponse> _responseQueue;



        public HttpClient(string url, GameSessionClient gameSession)
        {
            _baseUrl = url;
            _responseQueue = new Queue<HttpResponse>();
            _gameSession = gameSession;

            //ServicePointManager.MaxServicePointIdleTime = 500;
            ServicePointManager.DefaultConnectionLimit = 100;
        }


        public void Dispose()
        {
            _gameSession = null;
            _responseQueue.Clear();
            _responseQueue = null;
        }


        public void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }


        public string GetAccessToken()
        {
            return _accessToken;
        }


        public bool SendMessage(int protocolId, byte[] buffer)
        {
            return false;
        }


        public bool SendHttpPost(int protocolId, string method, string json)
        {
            HttpPostAsync(protocolId, _baseUrl + "/" + method, json);
            //HttpPost(protocolId, _baseUrl + "/" + method, json);
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


        void HttpPost(int protocolId, string url, string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Timeout = 5000;
            request.KeepAlive = false;
            //request.AllowAutoRedirect = false;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            Logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}][HTTP REQ - {url}] : {json}");


            HttpWebResponse response = null;
            try
            {
                // 요청, 응답 받기
                response = (HttpWebResponse)request.GetResponse();
                if (request.HaveResponse == true)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // 응답 Stream 읽기
                        using (Stream streamResponse = response.GetResponseStream())
                        using (StreamReader streamRead = new StreamReader(streamResponse))
                        {
                            string resJson = streamRead.ReadToEnd();
                            resJson = Regex.Unescape(resJson);
                            resJson = resJson.Replace("\"{", "{");
                            resJson = resJson.Replace("}\"", "}");
                            resJson = resJson.Replace("\"[", "[");
                            resJson = resJson.Replace("]\"", "]");


                            byte[] ackBytes = Encoding.UTF8.GetBytes(resJson);
                            _gameSession.PushExternalMessage(
                                this,
                                protocolId + 1,
                                ackBytes,
                                ackBytes.Length);

                            Logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}][HTTP ACK - {url}] : {resJson}, size: {Encoding.Default.GetBytes(resJson).Length}");
                        }

                        response.Close();
                    }
                }
            }
            catch (WebException wex)
            {
                Logger.Error($"[{Thread.CurrentThread.ManagedThreadId}, Error: {wex.Message}]");

                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
                
                request.Abort();
            }
        }


        void HttpPostAsync(int protocolId, string url, string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Timeout = 300000;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }


            Logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}][HTTP REQ - {url}] : {json}");

            request.BeginGetResponse(new AsyncCallback((asynchronousResult) =>
            {
                try
                {
                    // End the operation
                    HttpWebResponse response = (HttpWebResponse)((HttpWebRequest)asynchronousResult.AsyncState).EndGetResponse(asynchronousResult);
                    if (request.HaveResponse == true)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // 응답 Stream 읽기
                            using (Stream streamResponse = response.GetResponseStream())
                            using (StreamReader streamRead = new StreamReader(streamResponse))
                            {
                                string resJson = streamRead.ReadToEnd();
                                resJson = Regex.Unescape(resJson);
                                resJson = resJson.Replace("\"{", "{");
                                resJson = resJson.Replace("}\"", "}");
                                resJson = resJson.Replace("\"[", "[");
                                resJson = resJson.Replace("]\"", "]");

                                byte[] ackBytes = Encoding.UTF8.GetBytes(resJson);
                                _gameSession.PushExternalMessage(
                                    this,
                                    protocolId + 1,
                                    ackBytes,
                                    ackBytes.Length);

                                Logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}][HTTP ACK - {url}] : {resJson}, size: {Encoding.Default.GetBytes(resJson).Length}");

                            }

                            response.Close();
                            response.Dispose();
                        }
                    }
                }
                catch (WebException ex)
                {
                    Logger.Error($"[HTTP WebException. {ex.Message}");

                    var hwr = (HttpWebResponse)ex.Response;
                    if (hwr != null)
                    {
                        var responseex = hwr.StatusCode;
                        int statcode = (int)responseex;
                        Logger.Error($"[HTTP status. {statcode}");

                        if (statcode == 404)
                        {
                            //Utility.Instance.log(logPath, "The file might not be availble yet at the moment. Please try again later or contact your system administrator.", true);
                        }
                        if (statcode == 401)
                        {
                            //Utility.Instance.log(logPath, "Username and Password do not match.", true);
                        }
                        if (statcode == 408)
                        {
                            //Utility.Instance.log(logPath, "The operation has timed out", true);
                        }
                    }
                    else
                    {
                        //    Utility.Instance.log(logPath, ex + ". Please contact your administrator.", true);//Or you can do a different thing here
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"[HTTP Exception. {ex.Message}");

                    //    Utility.Instance.log(logPath, ex + ". Please contact your administrator.", true);//Or you can do a different thing here
                }
            }), request);
        }

        async void HttpPostAsync2(int protocolId, string url, string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Timeout = 30000;
            request.KeepAlive = false;


            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            Logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}][HTTP REQ - {url}] : {json}");


            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            await task.ContinueWith(t => ReadStreamFromResponse(t.Result, protocolId, url));
        }

        private void ReadStreamFromResponse(WebResponse response, int protocolId, string url)
        {
            try
            {
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader sr = new StreamReader(responseStream))
                {
                    //Need to return this response 
                    string resJson = sr.ReadToEnd();
                    resJson = Regex.Unescape(resJson);
                    resJson = resJson.Replace("\"{", "{");
                    resJson = resJson.Replace("}\"", "}");
                    resJson = resJson.Replace("\"[", "[");
                    resJson = resJson.Replace("]\"", "]");

                    byte[] ackBytes = Encoding.UTF8.GetBytes(resJson);
                    _gameSession.PushExternalMessage(
                        this,
                        protocolId + 1,
                        ackBytes,
                        ackBytes.Length);

                    Logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}][HTTP ACK - {url}] : {resJson}, size: {Encoding.Default.GetBytes(resJson).Length}");
                }

                response.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }

    //void ResponseAsync(HttpWebRequest request, Action<HttpWebResponse> responseAction)
    //    {
    //        Action wrapperAction = () =>
    //        {
    //            request.BeginGetResponse(new AsyncCallback((iar) =>
    //            {
    //                var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
    //                responseAction(response);

    //                request.Abort();
    //            }), request);
    //        };
    //        wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
    //        {
    //            var action = (Action)iar.AsyncState;
    //            action.EndInvoke(iar);
    //        }), wrapperAction);
    //    }
    //}
}