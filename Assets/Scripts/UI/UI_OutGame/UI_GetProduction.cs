using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RandomWarsProtocol;
using DG.Tweening;
using Random = UnityEngine.Random;

public class UI_GetProduction : MonoBehaviour
{
    public Camera cam;
    [Header("Sprite")]
    public Sprite sprite_Key;
    public Sprite sprite_Gold;
    public Sprite sprite_Diamond;
    public Sprite sprite_Trophy;
    public Sprite sprite_Box;

    [Header("Target")]
    public RectTransform rts_Key;
    public RectTransform rts_Gold;
    public RectTransform rts_Diamond;
    
    [Space]
    public List<Image> list_Image;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Initialize(ITEM_TYPE.DIAMOND, Input.mousePosition, 10);
        }
    }
    
    public void Initialize(ITEM_TYPE type, Vector2 startPos, int count)
    {
        Vector2 endPos = Vector2.zero;
        for (int i = 0; i < list_Image.Count; i++)
        {
            switch (type)
            {
                case ITEM_TYPE.NONE:
                    break;
                case ITEM_TYPE.GOLD:
                    list_Image[i].sprite = sprite_Gold;
                    list_Image[i].SetNativeSize();
                    endPos = cam.ScreenToViewportPoint(rts_Gold.position);
                    break;
                case ITEM_TYPE.DIAMOND:
                    list_Image[i].sprite = sprite_Diamond;
                    list_Image[i].SetNativeSize();
                    endPos = rts_Diamond.position;
                    break;
                case ITEM_TYPE.TROPHY:
                    list_Image[i].sprite = sprite_Trophy;
                    list_Image[i].SetNativeSize();
                    break;
                case ITEM_TYPE.KEY:
                    list_Image[i].sprite = sprite_Key;
                    list_Image[i].SetNativeSize();
                    endPos = rts_Key.anchoredPosition;
                    break;
                case ITEM_TYPE.PASS:
                    break;
                case ITEM_TYPE.BOX:
                    break;
                case ITEM_TYPE.DICE:
                    break;
                case ITEM_TYPE.GUADIAN:
                    break;
            }
            
            if (i < count)
            {
                Move(i, startPos, endPos, i * 0.025f);
            }
        }
    }

    public void Move(int num, Vector2 startPos, Vector2 endPos, float delay)
    {
        list_Image[num].rectTransform.anchoredPosition = startPos;
        list_Image[num].rectTransform.DOAnchorPos(startPos, 0f).SetDelay(delay).OnComplete(() =>
        {
            list_Image[num].gameObject.SetActive(true);
            list_Image[num].rectTransform.DOAnchorPos(startPos + Random.insideUnitCircle * 250, 0.5f).OnComplete(() =>
            {
                list_Image[num].transform.DOMove(endPos, 0.5f).SetDelay(0.15f).OnComplete(() =>
                {
                    list_Image[num].gameObject.SetActive(false);
                });
            });
        });
    }
}
