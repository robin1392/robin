using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RandomWarsProtocol;
using DG.Tweening;
using ED;
using Random = UnityEngine.Random;

public class UI_GetProduction : SingletonDestroy<UI_GetProduction>
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

    [SerializeField]
    private float power = 3f;
    private Ease ease = Ease.OutQuad;

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
                    endPos = rts_Gold.position;
                    StartCoroutine(EndMove(ITEM_TYPE.GOLD));
                    break;
                case ITEM_TYPE.DIAMOND:
                    list_Image[i].sprite = sprite_Diamond;
                    list_Image[i].SetNativeSize();
                    endPos = rts_Diamond.position;
                    StartCoroutine(EndMove(ITEM_TYPE.DIAMOND));
                    break;
                case ITEM_TYPE.TROPHY:
                    list_Image[i].sprite = sprite_Trophy;
                    list_Image[i].SetNativeSize();
                    break;
                case ITEM_TYPE.KEY:
                    list_Image[i].sprite = sprite_Key;
                    list_Image[i].SetNativeSize();
                    endPos = rts_Key.position;
                    StartCoroutine(EndMove(ITEM_TYPE.KEY));
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
                Move(i, startPos, endPos, i * 0.02f);
            }
        }
    }

    public void Move(int num, Vector2 startPos, Vector2 endPos, float delay)
    {
        list_Image[num].transform.position = startPos;
        list_Image[num].transform.DOMove(startPos, 0f).SetDelay(delay).OnComplete(() =>
        {
            list_Image[num].gameObject.SetActive(true);
            list_Image[num].transform.DOMove(startPos + Random.insideUnitCircle * power, 0.5f).SetEase(ease).OnComplete(() =>
            {
                list_Image[num].transform.DOMove(endPos, 0.4f).SetEase(ease).SetDelay(0.15f).OnComplete(() =>
                {
                    list_Image[num].gameObject.SetActive(false);
                });
            });
        });
        
    }

    private IEnumerator EndMove(ITEM_TYPE type)
    {
        yield return new WaitForSeconds(1f);

        switch (type)
        {
            case ITEM_TYPE.GOLD:
            {
                int oldGold = System.Int32.Parse(UI_Main.Get().text_Gold.text);
                int newGold = UserInfoManager.Get().GetUserInfo().gold;

                for (int i = 0; i < 5; i++)
                {
                    UI_Main.Get().text_Gold.text = Mathf.RoundToInt(Mathf.Lerp(oldGold, newGold, i / 5f)).ToString();
                    yield return new WaitForSeconds(0.15f);
                }

                UI_Main.Get().text_Gold.text = newGold.ToString();
            }
                break;
            case ITEM_TYPE.DIAMOND:
            {
                int oldDia = System.Int32.Parse(UI_Main.Get().text_Diamond.text);
                int newDia = UserInfoManager.Get().GetUserInfo().diamond;

                for (int i = 0; i < 5; i++)
                {
                    UI_Main.Get().text_Diamond.text = Mathf.RoundToInt(Mathf.Lerp(oldDia, newDia, i / 5f)).ToString();
                    yield return new WaitForSeconds(0.15f);
                }

                UI_Main.Get().text_Diamond.text = newDia.ToString();
            }
                break;
        }
        
    }
}
