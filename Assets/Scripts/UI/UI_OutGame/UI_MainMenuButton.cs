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

        public void Up()
        {
            image_BG.DOColor(Color.white, 0.5f);
            rts_Icon.DOAnchorPosY(71f, 0.25f);
            text_Name.DOColor(Color.white, 0.25f);
            text_Name.rectTransform.DOAnchorPosY(-70f, 0.25f);
            if (image_Arrow_Left != null)
            {
                image_Arrow_Left.rectTransform.DOAnchorPosX(-140, 0.3f);
                image_Arrow_Left.DOFade(1f, 0.3f);
            }
            if (image_Arrow_Right != null)
            {
                image_Arrow_Right.rectTransform.DOAnchorPosX(140, 0.3f);
                image_Arrow_Right.DOFade(1f, 0.3f);
            }
        }

        public void Down()
        {
            image_BG.DOColor(Color.gray, 0.5f);
            rts_Icon.DOAnchorPosY(0f, 0.25f);
            text_Name.DOColor(Color.clear, 0.25f);
            text_Name.rectTransform.DOAnchorPosY(-170f, 0.25f);
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
