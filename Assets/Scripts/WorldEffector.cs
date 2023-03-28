using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEffector : MonoBehaviour
{
    [Header("Instance Settings")]
    [SerializeField] private List<GameObject> _objects;
    [SerializeField] bool _enablesObject;

    [Header("Unity Setup")]
    [SerializeField] private LayerMask _golemLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.gameObject.GetComponent<Golem>().State != GolemState.Enabled) return;

        foreach (GameObject gobject in _objects)
        {
            gobject.SetActive(_enablesObject);
        }
    }
}
