using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToMoss : MonoBehaviour
{
    [SerializeField] private ParticleSystem _breakEffect;

    private void Awake()
    {
        Music.Instance.PlayDesertMusic();
    }

    public void OnEffector()
    {
        _breakEffect.Play();

        if(Music.Instance) Music.Instance.StopMusic();
    }
}
