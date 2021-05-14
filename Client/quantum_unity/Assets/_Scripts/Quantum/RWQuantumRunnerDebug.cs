using Quantum;
using System;
using System.Collections;
using System.Linq;
using ED;
using Photon.Deterministic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RWQuantumRunnerDebug : QuantumCallbacks
{
    public RecordingFlags RecordingFlags = RecordingFlags.Default;

    public InstantReplaySettings InstantReplayConfig = InstantReplaySettings.Default;

    public float SimulationSpeedMultiplier = 1.0f;
    public RuntimeConfig Config;
    public RuntimePlayer[] Players;

    bool _isReload;

    public void StartWithFrame(int frameNumber = 0, byte[] frameData = null)
    {
        _isReload = frameNumber > 0 && frameData != null;

        Debug.Log("### Starting quantum in local debug mode ###");

        var mapdata = FindObjectOfType<MapData>();
        if (mapdata)
        {
            // set map to this maps asset
            Config.Map = mapdata.Asset.Settings;

            var playerCount = Math.Max(1, Players.Length);

            // create start game parameter
            var param = new QuantumRunner.StartParameters
            {
                RuntimeConfig = Config,
                DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
                ReplayProvider = null,
                GameMode = Photon.Deterministic.DeterministicGameMode.Local,
                InitialFrame = 0,
                RunnerId = "LOCALDEBUG",
                PlayerCount = playerCount,
                LocalPlayerCount = playerCount,
                RecordingFlags = RecordingFlags,
                InstantReplayConfig = InstantReplayConfig,
            };

            param.InitialFrame = frameNumber;
            param.FrameData = frameData;

            // start with debug config
            var runner = QuantumRunner.StartGame("LOCALDEBUG", param);
        }
        else
        {
            throw new Exception("No MapData object found, can't debug start scene");
        }
    }

    public override void OnGameStart(QuantumGame game)
    {
        if (game.Session.IsPaused) return;

        if (_isReload == false)
        {
            var tableManager = TableManager.Get();
            var quantumTableDataContainer = new QuantumCodeTableDataContainer()
            {
                VsMode = tableManager.Vsmode,
                DiceInfo = tableManager.DiceInfo,
                GuardianInfo = tableManager.GuardianInfo,
                CoopMode = tableManager.CoopMode,
            };

            game.Frames.Verified.Context.SetData(quantumTableDataContainer);
            game.Frames.Verified.Context.SetFieldPositions(FieldManager.Get().ToFieldPositions());

            var index = 0;
            foreach (var lp in game.GetLocalPlayers())
            {
                game.SendPlayerData(lp, Players[index]);
                index++;
            }
        }
    }

    public override void OnGameResync(QuantumGame game)
    {
        Debug.Log("OnGameResync");
    }


    // Update is called once per frame
    public void OnGUI()
    {
        if (QuantumRunner.Default != null && QuantumRunner.Default.Id == "LOCALDEBUG")
        {
            if (GUI.Button(new Rect(Screen.width - 150, 10, 140, 25), "Save And Reload"))
            {
                StartCoroutine(SaveAndReload());
            }
        }
    }

    public void Update()
    {
        if (QuantumRunner.Default != null && QuantumRunner.Default.Session != null)
        {
            // Set the session ticking to manual to inject custom delta time.
            QuantumRunner.Default.OverrideUpdateSession = SimulationSpeedMultiplier != 1.0f;
            if (QuantumRunner.Default.OverrideUpdateSession)
            {
                var deltaTime = QuantumRunner.Default.DeltaTime;
                if (deltaTime == null)
                {
                    // DeltaTime can be null if we selected Quantum internal stopwatch. Use unscaled Unity time instead.
                    deltaTime = Time.unscaledDeltaTime;
                }

                QuantumRunner.Default.Session.Update(deltaTime * SimulationSpeedMultiplier);
                UnityDB.Update();
            }
        }
    }

    IEnumerator SaveAndReload()
    {
        var frameNumber = QuantumRunner.Default.Game.Frames.Verified.Number;
        var frameData = QuantumRunner.Default.Game.Frames.Verified.Serialize(DeterministicFrameSerializeMode.Blit);

        Log.Info($"Serialized Frame size: {frameData.Length} bytes");

        QuantumRunner.ShutdownAll();

        while (QuantumRunner.ActiveRunners.Any())
        {
            yield return null;
        }

        StartWithFrame(frameNumber, frameData);
    }
}