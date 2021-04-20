using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

[RequireComponent(typeof(RawImage))]
public class RawImageFlow : MonoBehaviour
{
    public Vector2 speed;
    public RawImage image;
    public float factor = 1f;
    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    void Update()
    {
        var offset = speed * factor * Time.time;
        image.uvRect = new Rect(offset.x, offset.y, 1, 1);
    }
}
