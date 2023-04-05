using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MovingPlatformChild : MonoBehaviour
{
    [SerializeField] private LayerMask _golemLayer;
    private FixedJoint2D _platformJoint;
    private void Awake()
    {
        _platformJoint = GetComponent<FixedJoint2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.contacts[0].normal.y < 0f)
        {
            collision.transform.SetParent(transform);
            //_platformJoint.connectedBody = collision.collider.attachedRigidbody;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        collision.transform.SetParent(null);
        //_platformJoint.connectedBody = null;
    }
}
