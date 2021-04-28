using System.Collections;
using Quantum;
using UnityEngine;

public unsafe class BotSDKDebugger : QuantumCallbacks, IBotDebug
{
  public EntityRef EntityRef => _prefabView != null ? _prefabView.EntityRef : EntityRef.None;

  public Frame Frame => QuantumRunner.Default.Game.Frames.Predicted;

  private EntityView _prefabView;
  private HFSMData _hfsmData;
  private BTAgent _btAgent;

  private IEnumerator Start()
  {
    if (QuantumRunner.Default == null || QuantumRunner.Default.Game == null)
    {
      yield break;
    }

    _prefabView = GetComponent<EntityView>();

    while (_prefabView.EntityRef == default)
    {
      yield return null;
    }

    TrySetupHFSMDebugger();
    TrySetupBTDebugger();
  }

  private bool TrySetupHFSMDebugger()
  {
    var frame = QuantumRunner.Default.Game.Frames.Predicted;
    if (frame.Has<HFSMAgent>(_prefabView.EntityRef) == false)
    {
      return false;
    }

    _hfsmData = frame.Get<HFSMAgent>(_prefabView.EntityRef).Data;
    return true;
  }

  private bool TrySetupBTDebugger()
  {
    var frame = QuantumRunner.Default.Game.Frames.Predicted;
    if (frame.Has<BTAgent>(_prefabView.EntityRef) == false)
    {
      return false;
    }

    _btAgent = frame.Get<BTAgent>(_prefabView.EntityRef);
    return true;
  }

  public string GetHFSMRootName()
  {
    if(_hfsmData.Equals(default) == true && TrySetupHFSMDebugger() == false)
    {
      return "";
    }
    
    return _hfsmData.Root.Id != default ? UnityDB.FindAsset<HFSMRootAsset>(_hfsmData.Root.Id).name : "";
  }

  public string GetBTRootName()
  {
    if (_btAgent.Equals(default) == true && TrySetupBTDebugger() == false)
    {
      return "";
    }

    return _btAgent.Tree.Id != default ? UnityDB.FindAsset<BTNodeAsset>(_btAgent.Tree.Id).name : "";
  }
}
