using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mirage;
using Mirage.KCP;
using UnityEngine;

namespace _Scripts.RCore.Networking
{
    public class ClientStarter : MonoBehaviour
    {
        static readonly ILogger Logger = LogFactory.GetLogger<ClientStarter>();
        private NetworkClient _client;
        public int WaitForAutoConnectionMs = 2000;
        public bool AutoConnect = true;

        private void Awake()
        {
            _client = GetComponent<NetworkClient>();
        }

        async void Start()
        {
            _client.Authenticated.AddListener(OnAuthenticated);
            _client.Connected.AddListener(OnConnected);
            _client.Disconnected.AddListener(OnDisconnected);
        
            await Task.Delay(WaitForAutoConnectionMs);

            if (AutoConnect == false)
            {
                return;
            }

            string address = "192.168.10.57";
            ushort port = GetComponent<KcpTransport>().Port;
            Connect(address, port).Forget();
        }

        public async UniTask Connect(string address, ushort port)
        {
            GetComponent<KcpTransport>().Port = port;
            var builder = new UriBuilder
            {
                Host = address,
                Scheme = _client.Transport.Scheme.First(),
            };
            
            var retryCount = 5;
            var count = 0;
            while (count < retryCount)
            {
                count++;
                try
                {
                    Debug.Log("connect:" + builder.Uri);
                    Debug.Log(builder.Uri.IsDefaultPort);
                    await _client.ConnectAsync(builder.Uri);
                }
                catch (OperationCanceledException e)
                {
                    Logger.LogError(e.Message);
                    Logger.Log($"Retry {count}");
                    continue;
                }
                
                break;
            } 
        }

        private void OnDisconnected()
        {
            Logger.Log("OnDisconnected");
        }

        private void OnConnected(INetworkConnection arg0)
        {
            Logger.Log($"Client OnConnected {gameObject.GetInstanceID()}");
        }

        private void OnAuthenticated(INetworkConnection arg0)
        {
            Logger.Log($"Client OnAuthenticated {gameObject.GetInstanceID()}");
        }
    }
}