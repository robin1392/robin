using System;
using System.Collections;
using Mirage;
using MirageTest.Scripts;
using UnityEngine;

public class RWAthenticator : NetworkAuthenticator
{
    static readonly ILogger Logger = LogFactory.GetLogger<RWAthenticator>();
    public string LocalId;
    public string LocalName;

    public struct AuthRequestMessage
    {
        public string PlayerId;
        public string NickName;
    }

    public struct AuthResponseMessage
    {
        public byte Code;
        public string Message;
    }

    private void Awake()
    {
        OnClientAuthenticated += OnClientAuthenticatedCallback; 
    }

    private void OnClientAuthenticatedCallback(INetworkConnection obj)
    {
        // GetComponent<RWNetworkClient>().Authenticated.Invoke(obj);
    }

    public override void OnServerAuthenticate(INetworkConnection conn)
    {
        // wait for AuthRequestMessage from client
        conn.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage);
    }

    public override void OnClientAuthenticate(INetworkConnection conn)
    {
        conn.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage);

        Debug.Log("Auth" + LocalId);
        conn.AuthenticationData = new AuthDataForConnection()
        {
            PlayerId = LocalId,
            PlayerNickName = LocalName,
        };

        var authRequestMessage = new AuthRequestMessage
        {
            PlayerId = LocalId,
            NickName = LocalName,
        };

        conn.Send(authRequestMessage);
    }

    public void OnAuthRequestMessage(INetworkConnection conn, AuthRequestMessage msg)
    {
        if (true)
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

    public IEnumerator DelayedDisconnect(INetworkConnection conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        conn.Disconnect();
    }

    public void OnAuthResponseMessage(INetworkConnection conn, AuthResponseMessage msg)
    {
        if (msg.Code == 100)
        {
            if (Logger.LogEnabled()) Logger.LogFormat(LogType.Log, "Authentication Response: {0}", msg.Message);

            // Invoke the event to complete a successful authentication
            base.OnClientAuthenticate(conn);
        }
        else
        {
            Logger.LogFormat(LogType.Error, "Authentication Response: {0}", msg.Message);
            // disconnect the client
            conn.Disconnect();
        }
    }
}

public class AuthDataForConnection
{
    public string PlayerId;
    public string PlayerNickName;
}