using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToMoss : MonoBehaviour
{
    [SerializeField] private ParticleSystem _breakEffect;

    private void Awake()
    {
        //Music.Instance.PlayDesertMusic();
    }

    public void OnEffector()
    {
        _breakEffect.Play();
        _breakEffect.GetComponent<AudioSource>().Play();

        if(Music.Instance) Music.Instance.StopMusic();
    }
}
