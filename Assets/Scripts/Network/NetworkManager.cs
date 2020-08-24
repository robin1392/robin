using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
    
    
    #region net variable
    
    // web
    public WebNetworkCommon webNetCommon { get; private set; }
    public WebPacket webPacket { get; private set; }

    // socket
    
    
    
    #endregion
    
    
    
    #region unity base

    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitNetwork();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        DestroyNetwork();
        
        base.OnDestroy();
    }

    #endregion
    
    
    #region net add

    public void InitNetwork()
    {
        webNetCommon = this.gameObject.AddComponent<WebNetworkCommon>();
        webPacket = this.gameObject.AddComponent<WebPacket>();
    }

    
    public void DestroyNetwork()
    {
        GameObject.Destroy(webPacket);
        GameObject.Destroy(webNetCommon);
    }
    #endregion
    
    
}

