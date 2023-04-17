using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Golem")) return;

        if (Music.Instance) Music.Instance.PlayMossMusic();

        Destroy(gameObject);
    }
}
