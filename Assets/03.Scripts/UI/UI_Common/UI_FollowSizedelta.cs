using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UI_FollowSizedelta : MonoBehaviour
{
    public RectTransform rts_Target;

    private RectTransform rts;

    private void Awake()
    {
        rts = (RectTransform) transform;
    }

    private void LateUpdate()
    {
        rts.sizeDelta = rts_Target.sizeDelta;
    }
}
