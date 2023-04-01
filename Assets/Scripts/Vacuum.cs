using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : MonoBehaviour
{

    private int _spiritLayer, _golemLayer;

    private void Awake()
    {
        _spiritLayer = LayerMask.NameToLayer("Spirit");
        _golemLayer = LayerMask.NameToLayer("Golem");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == _spiritLayer)
        {
            collision.transform.GetComponentInChildren<SpiritUnion>().VacuumSpirit(transform);
        }
        else if(collision.gameObject.layer == _golemLayer)
        {

        }
    }
}
