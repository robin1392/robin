using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Effect_SpawnLine : MonoBehaviour
{
    public LineRenderer lr;
    public float time = 0.2f;

    private void OnEnable()
    {
        StartCoroutine(nameof(LineRendererCoroutine));
    }

    private IEnumerator LineRendererCoroutine()
    {
        float t = 0;
        while (t < time)
        {
            lr.widthMultiplier = (0.2f - t);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
