using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public GameObject obj_Light;

    private void OnEnable()
    {
        obj_Light.SetActive(true);

        StartCoroutine(LightOffCoroutine());
    }

    IEnumerator LightOffCoroutine()
    {
        yield return new WaitForSeconds(0.15f);
        
        obj_Light.SetActive(false);
    }
}
