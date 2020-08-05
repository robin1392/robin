#if UNITY_EDITOR
#define ENABLE_LOG
#endif


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region photon
using Photon.Pun;
#endregion



public class PlayerUnitManager : SingletonPhoton<PlayerUnitManager>, IPunObservable
{
    
    #region system variable
    #endregion
    
    
    #region unity base

    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion
    
    #region init & destroy

    public void InitializeManager()
    {
        
    }

    public void DestroyManager()
    {
        
    }

    public void StartManager()
    {
        
    }
    
    
    #endregion
    
    
    #region photon base
    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
    #endregion
    
    
    #region photon send , recv
    public void SendPlayer(RpcTarget target , E_PTDefine ptID , params object[] param)
    {
        photonView.RPC("RecvPlayer", target , ptID , param);
    }

    [PunRPC]
    public void RecvPlayer(E_PTDefine ptID, params object[] param)
    {
        
    }
    #endregion
    
}
