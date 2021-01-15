using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace ED
{
    public class UI_MainMenuButton : MonoBehaviour
    {
        public Image image_BG;
        public RectTransform rts_Icon;
        public Text text_Name;
        public Image image_Arrow_Left;
        public Image image_Arrow_Right;

        [Header("Sprite BG")]
        public Sprite[] arrSprite_BG;
            
        private readonly float duration = 0.25f;

        public void Up()
        {
            //image_BG.DOColor(Color.white, 0.5f);
            image_BG.sprite = arrSprite_BG[1];
            rts_Icon.DOAnchorPosY(71f, duration);
            rts_Icon.DOScale(1f, duration);
            text_Name.DOColor(Color.white, duration);
            text_Name.rectTransform.DOAnchorPosY(-70f, duration);
            if (image_Arrow_Left != null)
            {
                image_Arrow_Left.rectTransform.DOAnchorPosX(-170, 0.3f);
                image_Arrow_Left.DOFade(1f, 0.3f);
            }
            if (image_Arrow_Right != null)
            {
                image_Arrow_Right.rectTransform.DOAnchorPosX(170, 0.3f);
                image_Arrow_Right.DOFade(1f, 0.3f);
            }
        }

        public void Down()
        {
            //image_BG.DOColor(Color.gray, 0.5f);
            image_BG.sprite = arrSprite_BG[0];
            rts_Icon.DOAnchorPosY(0f, duration);
            rts_Icon.DOScale(0.5f, duration);
            text_Name.DOColor(Color.clear, duration);
            text_Name.rectTransform.DOAnchorPosY(-170f, duration);
            if (image_Arrow_Left != null)
            {
                image_Arrow_Left.rectTransform.DOAnchorPosX(0, 0.3f);
                image_Arrow_Left.DOFade(0, 0.3f);
            }
            if (image_Arrow_Right != null)
            {
                image_Arrow_Right.rectTransform.DOAnchorPosX(0, 0.3f);
                image_Arrow_Right.DOFade(0, 0.3f);
            }
        }
    }
}
