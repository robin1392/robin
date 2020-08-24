using System.Collections;
using System.Collections.Generic;
using System.Text;
using ErrorDefine;
using UnityEngine;
using UnityEngine.Networking;


//
public delegate void RecvCallback(PacketDefine.WebProtocol packID , string content , NetCallBack cbSuccess , NetCallBackFail cbFail );

public delegate void NetCallBack();

public delegate void NetCallBackFail(ErrorDefine.ErrorCode errorCode = ErrorCode.ErrorCode_None);

public class SendQueue
{
    public PacketDefine.WebProtocol packetDef;
    public string packetData;

    public RecvCallback recvCB;
    public RecvCallback recvFailCB;

    public NetCallBack cb_Success;
    public NetCallBackFail cb_Fail;
}

public class WebNetworkCommon : Singleton<WebNetworkCommon>
{
    
    #region variable
    Dictionary<string, string> _headers = new Dictionary<string, string>();
    
    // send message
    private Queue<SendQueue> _sendQueue = new Queue<SendQueue>();
    
    //
    private bool _bIsSending = false;

    private string _urlWebAdress;
    public string urlWebAdress
    {
        get => _urlWebAdress;
        private set => _urlWebAdress = value;
    }

    private int _sendFailCount = 0;
    #endregion
    
    
    #region unity base

    public override void Awake()
    {
        base.Awake();
        
        _bIsSending = false;
        _sendQueue.Clear();
        _sendFailCount = 0;
        
        _headers.Add("content-type", "application/json; charset=utf-8");
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateRequest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override void OnDestroy()
    {
        _sendQueue.Clear();
        
        
        base.OnDestroy();
    }

    #endregion

    #region url setting

    public void SetWebUrl(string url)
    {
        _urlWebAdress = url;
    }

    #endregion
    
    
    #region queue dequeue
    
    public void SendPacket(SendQueue sendData)
    {
        EnQueuePacket(sendData);
    }

    public void EnQueuePacket(SendQueue sendQue)
    {
        _sendQueue.Enqueue(sendQue);
    }


    public SendQueue DeQueuePacket()
    {
        SendQueue sendItem = _sendQueue.Dequeue();
        return sendItem;
    }

    public SendQueue DeQueuePeek()
    {
        SendQueue sendItem = _sendQueue.Peek();
        return sendItem;
    }

    public void ClearQueue()
    {
        _sendQueue.Clear();
    }
    
    #endregion
    
    
    #region request web

    IEnumerator UpdateRequest()
    {
        //
        // 패킷 잇을때만 큐에서 뽑아서 보내는걸로
        while ( Get() != null)
        {
            if (_bIsSending == false)
            {
                if (_sendQueue.Count > 0)
                {
                    StartCoroutine(WaitForRequest(DeQueuePeek()));
                }
            }
            yield return null;
        }
    }

    private IEnumerator WaitForRequest(SendQueue packData)
    {
        //
        if (packData == null)
        {
            _bIsSending = false;
            yield break;
        }

        if (_bIsSending == true)
        {
            yield break;
        }

        _bIsSending = true;

        string url = string.Empty;
        url = _urlWebAdress;

        byte[] body = Encoding.UTF8.GetBytes(packData.packetData);
        WWW www = new WWW(url , body , _headers);
        
        yield return www;
        
        //UnityWebRequest www = new UnityWebRequest(url , "POST");
        //www.uploadHandler = (UploadHandler)new UploadHandlerRaw(body);
        //www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //www.SetRequestHeader("Content-Type", "application/json");
        //yield return www.SendWebRequest();
        
        // 데이터 없을경우 에러
        if (www.bytes == null)
        {
            yield break;
        }

        string decodedString = Encoding.UTF8.GetString(www.bytes);
        if (www.error == null)
        {
            if (packData.recvCB == null)
            {
                // 
                yield break;
            }


            packData.recvCB(packData.packetDef, packData.packetData, packData.cb_Success, packData.cb_Fail);

            DeQueuePacket();
            _sendFailCount = 0;
            _bIsSending = false;
            
        }
        else
        {
            // 기본 10 번정도 재 시도 했는데도 실패했으면(시간상...20초)..실패 
            if (_sendFailCount >= 10)
            {
                // 추후..로그인으로 보내던지...

                if (packData.recvFailCB != null)
                    packData.recvFailCB(packData.packetDef, packData.packetData, packData.cb_Success, packData.cb_Fail);
                
                ClearQueue();
                _sendFailCount = 0;
                _bIsSending = false;
                yield break;
            }

            if (www.text.Equals(""))
            {
                string[] errStr = www.error.Split(':');
                if (errStr.Length > 3)
                {
                    if (errStr[3].Equals(" ETIMEDOUT (Connection timed out)"))
                    {
                        // 네트워크 연결상태 않좋음 재 시도 -- 패킷 재전송
                        _sendFailCount++;
                    }
                    else //if(str[0].Equals("Could not resolve host"))
                    {   
                        _sendFailCount++;
                    }
                }
                else
                {
                    // 연결 실패
                    if (errStr.Length <= 1)
                    {
                        _sendFailCount++;
                    }       // end if (str.Length <= 1)
                    else
                    {
                        if (errStr[1].Equals(" Timed out"))
                        {
                            // 패킷 재 전송
                            _sendFailCount++;
                        }       // if (str[1].Equals(" Timed out"))
                        else
                        {
                            Debug.Log("패킷 검사 로그 여기까지 오면 연결 실패도 아니고 타임아웃도 아니고...패킷이 버려지고 있다....");
                            foreach(string strPacketError in errStr)
                            {
                                Debug.Log("strPacketError -> "+ strPacketError);
                            }

                            
                            // 패킷 모두 버려
                            ClearQueue();
                            
                            // 알수없는 에러 -- 로그인으로 보내버려
                            //
                            
                            
                        }
                    }
                }
            }
            
            // 일단 실패 떳으면..2초 기다렸다가 보내봐
            {
                float elapsedTime = 0f;
                float duration = 2.0f;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
            _bIsSending = false;
        }
    }
    
    #endregion
    
    
    
    
}
