using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameplay : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Spirit")) return;

        var spiritDim = other.GetComponent<SpiritDim>();
        spiritDim.IsAtLevel1 = true;

        StartCoroutine(StartFading(spiritDim));

        Music.Instance.PlayDesertMusic();
        
        GetComponent<Collider2D>().enabled = false;
    }

    private IEnumerator StartFading(SpiritDim sd)
    {
        yield return new WaitForSeconds(4f);
        sd.IsFading = true;
        Destroy(gameObject);
    }
}
