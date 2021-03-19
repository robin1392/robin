using System;
using System.Collections;
using System.Linq;
using Aws.GameLift.Server;
using Mirage;
using Mirage.Logging;
using MirageTest.Aws;
using MirageTest.Scripts;
using UnityEngine;

public class RWAthenticator : NetworkAuthenticator
{
    static readonly ILogger Logger = LogFactory.GetLogger<RWAthenticator>();
    public string LocalUserId;
    public string LocalNickName;
    public string LocalPlayerSessionId;

    public struct AuthRequestMessage
    {
        public string PlayerId;
        public string NickName;
        public string PlayerSessionId;
    }

    public struct AuthResponseMessage
    {
        public byte Code;
        public string Message;
    }

    private void Awake()
    {
        if (GetComponent<RWNetworkClient>() != null)
        {
            var userInfo = UserInfoManager.Get().GetUserInfo();
            LocalUserId = userInfo.userID;
            LocalNickName = userInfo.userNickName;
        }
    }

    public override void OnServerAuthenticate(INetworkPlayer conn)
    {
        // wait for AuthRequestMessage from client
        conn.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage);
    }

    public override void OnClientAuthenticate(INetworkPlayer conn)
    {
        conn.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage);
        
        conn.AuthenticationData = new AuthDataForConnection()
        {
            PlayerId = LocalUserId,
            PlayerNickName = LocalNickName,
        };

        var authRequestMessage = new AuthRequestMessage
        {
            PlayerId = LocalUserId,
            NickName = LocalNickName,
            PlayerSessionId = LocalPlayerSessionId,
        };

        conn.Send(authRequestMessage);
    }

    public void OnAuthRequestMessage(INetworkPlayer conn, AuthRequestMessage msg)
    {
        var server = GetComponent<RWNetworkServer>(); 
        var authedPlayerInfo = server.MatchData.PlayerInfos.FirstOrDefault(p => p.UserId == msg.PlayerId);
        if (authedPlayerInfo != null)
        {
            var authResponseMessage = new AuthResponseMessage
            {
                Code = 100,
                Message = "Success"
            };

            conn.Send(authResponseMessage);

            // Invoke the event to complete a successful authentication
            base.OnServerAuthenticate(conn);

            conn.AuthenticationData = new AuthDataForConnection()
            {
                PlayerId = msg.PlayerId,
                PlayerNickName = msg.NickName,
            };
            
#if UNITY_EDITOR || UNITY_STANDALONE
            GameLiftServerAPI.AcceptPlayerSession(msg.PlayerSessionId);
#endif
            
        }
        else
        {
            // create and send msg to client so it knows to disconnect
            var authResponseMessage = new AuthResponseMessage
            {
                Code = 200,
                Message = $"Invalid Credentials"
            };

            conn.Send(authResponseMessage);

            // disconnect the client after 1 second so that response message gets delivered
            StartCoroutine(DelayedDisconnect(conn, 1));
        }
    }

    public IEnumerator DelayedDisconnect(INetworkPlayer conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        conn.Connection.Disconnect();
    }

    public void OnAuthResponseMessage(INetworkPlayer conn, AuthResponseMessage msg)
    {
        if (msg.Code == 100)
        {
            if (Logger.logEnabled) Logger.LogFormat(LogType.Log, "Authentication Response: {0}", msg.Message);

            // Invoke the event to complete a successful authentication
            base.OnClientAuthenticate(conn);
        }
        else
        {
            Logger.LogFormat(LogType.Error, "Authentication Response: {0}", msg.Message);
            // disconnect the client
            conn.Connection.Disconnect();
        }
    }
}

public class AuthDataForConnection
{
    public string PlayerId;
    public string PlayerNickName;
}