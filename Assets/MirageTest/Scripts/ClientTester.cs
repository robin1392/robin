using System.Collections;
using System.Collections.Generic;
using _Scripts.RCore.Networking;
using Mirage.KCP;
using MirageTest.Scripts;
using UnityEngine;

public class ClientTester : MonoBehaviour
{
    public GameObject Prefab; 
    void Awake()
    {
        var portStart = 7770;
        for (int i = 0; i < 20; ++i)
        {
            var port = (ushort) (portStart + i);
            var client1 = Instantiate(Prefab);
            client1.GetComponent<KcpTransport>().Port = port;
            client1.GetComponent<RWAthenticator>().LocalId = i.ToString();
            client1.GetComponent<RWAthenticator>().LocalName = i.ToString();
            client1.GetComponent<ClientStarter>().WaitForAutoConnectionMs = 1000 * (i + 1);
            if (i == 19)
            {
                client1.GetComponent<RWNetworkClient>().enableActor = true;
            }
            client1.SetActive(true);

            var client2 = Instantiate(Prefab);
            client2.GetComponent<KcpTransport>().Port = port;
            var id = (i + 1).ToString();
            client2.GetComponent<RWAthenticator>().LocalId = id.ToString();
            client2.GetComponent<RWAthenticator>().LocalName = id.ToString();
            client2.GetComponent<ClientStarter>().WaitForAutoConnectionMs = 1000 * (i + 1) + 500;
            client2.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
