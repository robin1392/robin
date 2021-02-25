using Cysharp.Threading.Tasks;
using Mirage;
using Mirage.KCP;
using UnityEngine;

public class ServerStarter : MonoBehaviour
{
    private void Awake()
    {
        var server = GetComponent<NetworkServer>();
        server.Started.AddListener(OnStarted);
        server.Authenticated.AddListener(OnAuthenticated);
        server.Connected.AddListener(OnConnected);
    }

    void Start()
    {
        StartServer().Forget();
    }
        
    async UniTask StartServer()
    {
        var server = GetComponent<NetworkServer>();
        await server.ListenAsync();
    }

    private void OnConnected(INetworkConnection arg0)
    {
        Debug.Log("Server OnConnected");
    }

    private void OnAuthenticated(INetworkConnection arg0)
    {
        Debug.Log("Server OnAuthenticated");
    }

    private void OnStarted()
    {
        Debug.Log("Server OnStarted");
    }
}