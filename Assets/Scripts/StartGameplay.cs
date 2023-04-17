using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameplay : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Spirit")) return;

        var spiritDim = other.GetComponent<SpiritDim>();
        spiritDim.IsFading = true;
        spiritDim.IsAtLevel1 = true;

        Music.Instance.PlayDesertMusic();
        Destroy(gameObject);
    }
}
