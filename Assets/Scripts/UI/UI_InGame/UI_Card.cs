using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ED
{
    public class UI_Card : MonoBehaviour
    {
        public Animator ani;
        public Image image_Card;
        public Text text_Name;
        public Text text_Type;
        public Text text_Discription;
        public bool isAnimationRunning;
        private static readonly int Off1 = Animator.StringToHash("Off");
        private static readonly int On = Animator.StringToHash("On");

        public void Initialize(Data_Dice pData)
        {
            isAnimationRunning = false;
            image_Card.sprite = pData.card;
            text_Name.text = pData.name;
            text_Type.text = pData.castType.ToString();
            ani.SetTrigger(On);
        }

        public void Off()
        {
            if (ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.65f)
            {
                ani.SetTrigger(Off1);
                isAnimationRunning = true;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}