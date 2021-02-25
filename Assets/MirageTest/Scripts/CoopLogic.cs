using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(NetworkServer))]
public class CoopLogic : NetworkBehaviour
{
    private NetworkServer _server;
    private ServerObjectManager _serverObjectManager;
    private readonly int _gamePlayerCount = 2;

    public GameObject actorPrefab;
    public Material[] materials;

    private bool _isGameStart;
    
    private void Awake()
    {
        // Application.targetFrameRate = 30;
        // QualitySettings.vSyncCount = 0;
        
        _server = GetComponent<NetworkServer>();
        _serverObjectManager = GetComponent<ServerObjectManager>();
    }

    private async void Start()
    {
        if (!_server.Active)
        {
            return;
        }

        while (true)
        {
            if (!CheckStart())
            {
                return;
            }
        
            await UpdateSpawn();
        }
    }

    private async UniTask UpdateSpawn()
    {
        SpawnActorsForPlayer();
        
        await UniTask.Delay(TimeSpan.FromSeconds(1000));
    }

    private void SpawnActorsForPlayer()
    {
        for (int i = 0; i < 12; ++i)
        {
            Spawn(new Vector3(i % 4,0,  ((i / 4) + 1)), 1, 1);
            Spawn(new Vector3(i % 4, 0, -((i / 4) + 1)), 2, 2);
        }
    }

    void Spawn(Vector3 position, int owner, int team)
    {
        var actor = Instantiate(actorPrefab, position, Quaternion.identity);
        var actorProxy = actor.GetComponent<ActorProxy>();
        actorProxy.owner = owner;
        actorProxy.team = team;
        actorProxy.SetTeamInternal(team);
        _serverObjectManager.Spawn(actor);
    }

    private bool CheckStart()
    {
        if (_isGameStart)
        {
            return true;
        }
        
        if (!_isGameStart)
        {
            var playerProxies =
                _serverObjectManager.SpawnedObjects.Where(kvp => kvp.Value.GetComponent<PlayerProxy>() != null);
            if (playerProxies.Count() >= _gamePlayerCount)
            {
                _isGameStart = true;
            }

            return true;
        }

        return false;
    }
}
