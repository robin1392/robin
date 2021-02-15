using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_IngameMessage : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Text text;

    public void Initialize(string text, float showtime = 3f)
    {
        this.text.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.text.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.GetChild(0));
        StartCoroutine(ShowCoroutine(showtime));
    }

    IEnumerator ShowCoroutine(float showtime)
    {
        yield return new WaitForSeconds(showtime);

        _canvasGroup.DOFade(0f, 0.2f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
