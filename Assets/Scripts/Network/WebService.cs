using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomWarsService.Network.Http;
using Newtonsoft.Json;
using RandomWarsProtocol;

public class WebService
{
    private HttpSender _httpSender;
    private HttpService _httpService;
    private HttpReceiver _httpReceiver;


    public WebService(string url)
    {
        _httpReceiver = new HttpReceiver();
        _httpReceiver.AuthUserAck = OnAuthUserAck;


        _httpService = new HttpService(url, _httpReceiver);
        _httpSender = new HttpSender(_httpService);
    }


    public void Update()
    {
        _httpService.Update();
    }


    public void AuthUserReq(string userId)
    {
        _httpSender.UserAuthReq(userId);
    }


    void OnAuthUserAck(GameErrorCode error, MsgUserInfo userInfo, MsgUserDeck[] userDeck, MsgUserDice[] userDice)
    {
        UserInfoManager.Get().SetUserKey(userInfo.UserId);
        GameStateManager.Get().UserAuthOK();
    }
}
