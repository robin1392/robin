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
    private Queue<Image> queue = new Queue<Image>();

    public override void Awake()
    {
        base.Awake();

        foreach (var image in list_Image)
        {
            queue.Enqueue(image);
        }
    }

    public void Initialize(ITEM_TYPE type, Vector2 startPos, int count)
    {
        Vector2 endPos = Vector2.zero;
        for (int i = 0; i < count; i++)
        {
            if (queue.Count > 0)
            {
                var image = queue.Dequeue();

                switch (type)
                {
                    case ITEM_TYPE.NONE:
                        break;
                    case ITEM_TYPE.GOLD:
                        image.sprite = sprite_Gold;
                        image.SetNativeSize();
                        endPos = rts_Gold.position;
                        StartCoroutine(EndMove(ITEM_TYPE.GOLD));
                        break;
                    case ITEM_TYPE.DIAMOND:
                        image.sprite = sprite_Diamond;
                        image.SetNativeSize();
                        endPos = rts_Diamond.position;
                        StartCoroutine(EndMove(ITEM_TYPE.DIAMOND));
                        break;
                    case ITEM_TYPE.TROPHY:
                        image.sprite = sprite_Trophy;
                        image.SetNativeSize();
                        break;
                    case ITEM_TYPE.KEY:
                        image.sprite = sprite_Key;
                        image.SetNativeSize();
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

                Move(image, startPos, endPos, i * 0.02f);
            }
        }
    }

    public void Move(Image image, Vector2 startPos, Vector2 endPos, float delay)
    {
        image.transform.position = startPos;
        image.transform.DOMove(startPos, 0f).SetDelay(delay).OnComplete(() =>
        {
            image.gameObject.SetActive(true);
            image.transform.DOMove(startPos + Random.insideUnitCircle * power, 0.5f).SetEase(ease).OnComplete(() =>
            {
                image.transform.DOMove(endPos, 0.4f).SetEase(ease).SetDelay(0.15f).OnComplete(() =>
                {
                    image.gameObject.SetActive(false);
                    queue.Enqueue(image);
                });
            });
        });
    }

    public void RefreshProduct(ITEM_TYPE type)
    {
        StartCoroutine(EndMove(type));
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

                float t = 0;
                while (t < 0.6f)
                {
                    UI_Main.Get().text_Gold.text = Mathf.RoundToInt(Mathf.Lerp(oldGold, newGold, t / 0.6f)).ToString();
                    t += Time.deltaTime;
                    yield return null;
                }

                UI_Main.Get().text_Gold.text = newGold.ToString();
            }
                break;
            case ITEM_TYPE.DIAMOND:
            {
                int oldDia = System.Int32.Parse(UI_Main.Get().text_Diamond.text);
                int newDia = UserInfoManager.Get().GetUserInfo().diamond;

                float t = 0;
                while (t < 0.6f)
                {
                    UI_Main.Get().text_Diamond.text = Mathf.RoundToInt(Mathf.Lerp(oldDia, newDia, t / 0.6f)).ToString();
                    t += Time.deltaTime;
                    yield return null;
                }

                UI_Main.Get().text_Diamond.text = newDia.ToString();
            }
                break;
        }
    }
}
