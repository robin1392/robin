using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomWarsService.Network.Http;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;


public class HttpJsonSerializer : IJsonSerializer
{
    public string SerializeObject<T>(T jObject)
    {
        return JsonHelper.ToJson<T>(jObject);
    }

    public T DeserializeObject<T>(string json)
    {
        return JsonHelper.Deserialize<T>(json);
    }

    public T[] DeserializeObjectArray<T>(string json)
    {
        return JsonHelper.DeserializeArray<T>(json);
    }

}

public class WebService
{
    private HttpSender _httpSender;
    private HttpClient _httpClient;
    private HttpReceiver _httpReceiver;


    public WebService(string url)
    {
        IJsonSerializer jsonSerializer = new HttpJsonSerializer();

        _httpReceiver = new HttpReceiver(jsonSerializer);
        _httpReceiver.AuthUserAck = OnAuthUserAck;


        _httpClient = new HttpClient(url, _httpReceiver);
        _httpSender = new HttpSender(_httpClient, jsonSerializer);
    }


    public void Update()
    {
        _httpClient.Update();
    }


    public void AuthUserReq(string userId)
    {
        MsgUserAuthReq msg = new MsgUserAuthReq();
        msg.UserId = userId;
        _httpSender.UserAuthReq(msg);
    }


    void OnAuthUserAck(MsgUserAuthAck msg)
    {
        UserInfoManager.Get().SetUserKey(msg.UserInfo.UserId);
        GameStateManager.Get().UserAuthOK();
    }
}
