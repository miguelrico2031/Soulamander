using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public bool HasHitWall;


    [SerializeField] private LayerMask _interactableLayers;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _golemLayer;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _wallCheckOffset;

    private bool _isGrounded;
    private Collider2D[] _wallHitColliders;
    private ContactFilter2D _wallCheckCF;
    private int _wallHitsSize;

    private void Awake()
    {
        _wallHitColliders = new Collider2D[6];
        _wallCheckCF = new ContactFilter2D
        {
            layerMask = _interactableLayers
        };
    }

    private void FixedUpdate()
    {
        WallCheck();
    }
 
    private void WallCheck()
    {
        HasHitWall = false;

        _wallHitColliders = new Collider2D[6];
        _wallHitsSize =
            Physics2D.OverlapBox(_collider.bounds.center, _collider.size * 0.9f + Vector2.right * _collider.size.x * _wallCheckOffset, 0f, _wallCheckCF, _wallHitColliders);

        if (_wallHitsSize == 0) return;

        foreach(var col in _wallHitColliders)
        {
            if (!col) continue;
            if (col.gameObject == gameObject || col.transform.parent == transform) continue;

            if ((_groundLayer.value & (1 << col.gameObject.layer)) > 0)
            {
                HasHitWall = true;
                _rb.velocity = new Vector2(0f, _rb.velocity.y);
            }

        }
    }
}
