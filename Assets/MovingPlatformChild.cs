using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MovingPlatformChild : MonoBehaviour
{
    [SerializeField] private LayerMask _golemLayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("1");
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.contacts[0].normal.y < 0f)
        {
            Debug.Log("2");
            collision.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        collision.transform.SetParent(null);
    }
}
