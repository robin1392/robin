using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class IntParameterRandomSetter : StateMachineBehaviour
{
    public string intParameterName;
    public int stateCount;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(intParameterName, Random.Range(0, stateCount));
    }
}
