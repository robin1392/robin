using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Input = Quantum.Input;

public class LocalInput : MonoBehaviour {
    
  private void OnEnable() {
    QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
  }

  public void PollInput(CallbackPollInput callback) {
    Quantum.Input i = new Quantum.Input();
    
    var v = UnityEngine.Input.mousePosition;
    var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(v.x, v.y, 10));
    var a = new FPVector2(worldPoint.x.ToFP(), worldPoint.z.ToFP());
    i.Dest = a;
    callback.SetInput(i, DeterministicInputFlags.Repeatable);
  }
}
