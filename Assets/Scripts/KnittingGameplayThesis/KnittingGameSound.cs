using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class KnittingGameSound : MonoBehaviour
{
public AudioClip enterStitch;
public AudioClip wrapYarn;
public AudioClip grabNeedle;
public AudioClip[] createStitch;
 private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void EnterStitch()
    {
        source.PlayOneShot(enterStitch);
    }

    public void WrapYarn()
    {
        source.PlayOneShot(wrapYarn);
    }

    public void GrabNeedle()
    {
        source.PlayOneShot(grabNeedle);
    }

    public void CreateStitch()
    {
        int random = Random.Range(0, createStitch.Length - 1);
        AudioClip clip = createStitch[random];
        source.PlayOneShot(clip);
    }

}
