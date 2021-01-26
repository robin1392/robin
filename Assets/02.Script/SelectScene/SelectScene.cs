using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Percent.Platform
{
    public class SelectScene : MonoBehaviour
    {
        /// <summary>
        /// 씬 이동 : 씬 선택
        /// </summary>
        public void MoveSelectScene()
        {
            SceneManager.LoadScene("01.Scenes/SelectScene");
        }

        /// <summary>
        /// 씬 이동 : IAP 샘플
        /// </summary>
        public void MoveInAppPurchaseScene()
        {
            SceneManager.LoadScene("01.Scenes/InAppPurchaseScene");
        }

        /// <summary>
        /// 씬 이동 : 스플래시
        /// </summary>
        public void MoveSplashScene()
        {
            SceneManager.LoadScene("01.Scenes/Splash");
        }
    }
}