using Cysharp.Threading.Tasks;
using Mirage;
using Mirage.KCP;
using MirageTest;
using UnityEngine;

public class ServerStarter : MonoBehaviour
{
    private void Awake()
    {
        var fps = CommandLineArgs.GetInt("fps"); 
        if (fps.HasValue)
        {
            Application.targetFrameRate = fps.Value; 
            Debug.Log($"fps from CommandLineArgs - {fps.Value}");
        }
        else
        {
            Application.targetFrameRate = 30;
        }
            
        QualitySettings.vSyncCount = 0;
        
        var port = CommandLineArgs.GetInt("port"); 
        if (port.HasValue)
        {
            GetComponent<KcpTransport>().Port = (ushort)port.Value;
            Debug.Log($"port from CommandLineArgs - {port.Value}");
        }
        
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