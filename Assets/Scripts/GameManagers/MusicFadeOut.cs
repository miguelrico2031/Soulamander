using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFadeOut : MonoBehaviour
{
    private bool _done;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_done && collision.gameObject.layer == LayerMask.NameToLayer("Golem"))
        {
            Music.Instance.FadeOutMusic();
            _done = true;
        }
    }
}
