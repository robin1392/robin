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

    public override void OnServerAuthenticate(INetworkPlayer conn)
    {
        conn.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage);
    }

    public override void OnClientAuthenticate(INetworkPlayer conn)
    {
        conn.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage);

        var client = GetComponent<RWNetworkClient>();
        
        conn.AuthenticationData = new AuthDataForConnection()
        {
            PlayerId = client.LocalUserId,
            PlayerNickName = client.LocalNickName,
            PlayerSessionId = client.PlayerSessionId,
        };

        var authRequestMessage = new AuthRequestMessage
        {
            PlayerId = client.LocalUserId,
            NickName = client.LocalNickName,
            PlayerSessionId = client.PlayerSessionId,
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
                PlayerSessionId = msg.PlayerSessionId
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
    public string PlayerSessionId;
}