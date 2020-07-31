using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG;
using DG.Tweening;

public class Loading : MonoBehaviour
{
    public Image image_Loading;
    public Text text_Loading;
    
    public void LoadMainScene()
    {
        StartCoroutine(LoadMainSceneCoroutine());
    }

    private IEnumerator LoadMainSceneCoroutine()
    {
        var async = SceneManager.LoadSceneAsync("Main");
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            image_Loading.fillAmount = async.progress;
            text_Loading.text = $"{async.progress * 100}%";
            yield return null;
        }

        image_Loading.fillAmount = 1f;
        var progress = 100;
        
        while (progress < 111)
        {
            progress += Random.Range(1, 5);
            text_Loading.text = $"{progress}%";
            yield return new WaitForSeconds(0.1f);
        }
        text_Loading.text = $"{111}%";
        image_Loading.DOColor(Color.white, 0.4f).SetLoops(2, LoopType.Yoyo);

        yield return new WaitForSeconds(1.5f);
        async.allowSceneActivation = true;
    }
}
