#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : SingletonDestroy<FieldManager>
{
    
    #region system variable
    public Transform ts_TopPlayer;
    public Transform ts_BottomPlayer;


    [Header("Spawn Position List")]
    public List<Transform> listTopPosition;
    public List<Transform> listBottomPosition;
    #endregion
    
    
    #region unity base

   
    
    public override void OnDestroy()
    {
        DestroyManager();
        
        base.OnDestroy();
    }

    #endregion
    
    #region init & destroy
    

    public void DestroyManager()
    {
        if(listTopPosition != null)
            listTopPosition.Clear();
        listTopPosition = null;

        if (listBottomPosition != null) 
            listBottomPosition.Clear();
        listBottomPosition = null;
    }
    #endregion
    
    #region get set

    public Vector3 GetTopListPos(int index)
    {
        if (listTopPosition.Count <= index)
            return ts_TopPlayer.position;
        
        return listTopPosition[index].position;
    }

    public Vector3 GetBottomListPos(int index)
    {
        if (listBottomPosition.Count <= index)
            return ts_BottomPlayer.position;

        return listBottomPosition[index].position;
    }
    
    public Transform GetTopListTs(int index)
    {
        if (listTopPosition.Count <= index)
            return ts_TopPlayer;
        
        return listTopPosition[index];
    }

    public Transform GetBottomListTs(int index)
    {
        if (listBottomPosition.Count <= index)
            return ts_BottomPlayer;

        return listBottomPosition[index];
    }

    public Vector3 GetPlayerPos(bool isBottomPlayer)
    {
        Vector3 pos = isBottomPlayer ? ts_BottomPlayer.position : ts_TopPlayer.position;
        return pos;
    }

    public Transform GetPlayerTrs(bool player)
    {
        return player?ts_BottomPlayer:ts_TopPlayer;
    }
    #endregion
    
    
}
