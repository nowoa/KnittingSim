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
public AudioClip BGM;
 public AudioSource source;
 private float targetVolume;
 public AudioSource _BGM_source;
 private bool _atTargetVolume;
 public float volumeChangeSpeed;

    private void Update()
    {
        if (!_atTargetVolume)
        {
            _BGM_source.volume = _BGM_source.volume > targetVolume
                ? _BGM_source.volume - (volumeChangeSpeed * Time.deltaTime)
                : _BGM_source.volume + (volumeChangeSpeed * Time.deltaTime);

            _BGM_source.volume = Mathf.Clamp01(_BGM_source.volume);
            if (_BGM_source.volume == targetVolume)
            {
                _atTargetVolume = true;
            }
        }
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

    public void FadeInBGM()
    {
        _atTargetVolume = false;
        targetVolume = 1;
    }
    
    public void FadeOutBGM()
    {
        _atTargetVolume = false;
        targetVolume = 0;
    }

}
