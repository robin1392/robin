using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    public AudioClip clip;

    public void Play()
    {
        SoundManager.instance.Play(clip);
    }
}
