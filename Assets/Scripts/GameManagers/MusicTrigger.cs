using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public bool IsCity = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Golem")) return;

        if (Music.Instance)
        {
            if (!IsCity) Music.Instance.PlayMossMusic();
            else Music.Instance.PlayCityMusic();
        }

        Destroy(gameObject);
    }
}
