using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    public RectTransform SafeAreaRect;
 
    private Rect lastSafeArea = Rect.zero;
    private Canvas canvas;
 
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    //
    // private void Update()
    // {
    //     if (lastSafeArea != Screen.safeArea)
    //     {
    //         lastSafeArea = Screen.safeArea;
    //         ApplySafeArea();
    //     }
    // }

    private void Start()
    {
        lastSafeArea = Screen.safeArea;
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        if (SafeAreaRect == null)
        {
            return;
        }
 
        var safeArea = Screen.safeArea;
 
        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;
 
        SafeAreaRect.anchorMin = anchorMin;
        SafeAreaRect.anchorMax = anchorMax;
    }
}
