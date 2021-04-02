using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Service.Core;

namespace Service.Net
{
    class HttpResponse
    {
        public int ProtocolId { get; set; }
        public string Json { get; set; }
        public byte[] msg { get; set; }
    }


    public class HttpClient : ISender
    {
        private string _accessToken;
        private GameSessionClient _gameSession;



        public HttpClient(GameSessionClient gameSession)
        {
            _gameSession = gameSession;

            //ServicePointManager.MaxServicePointIdleTime = 500;
            ServicePointManager.DefaultConnectionLimit = 100;
        }


        public void Dispose()
        {
            _gameSession = null;
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


        public bool SendHttpPost(int protocolId, string url, string json)
        {
            HttpPostAsync(protocolId, url, json);
            return true;
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
    }
}