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
    private RawImage _image;
    public float factor = 1f;
    private void Awake()
    {
        _image = GetComponent<RawImage>();
    }

    void Update()
    {
        var offset = speed * factor * Time.time;
        _image.uvRect = new Rect(offset.x, offset.y, 1, 1);
    }
}
