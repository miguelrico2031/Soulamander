using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public bool HasHitWall;


    [SerializeField] private LayerMask _interactableLayers;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _golemLayer, _spiritLayer;
    //[SerializeField] private float _gravityForce;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _groundCheckOffset, _wallCheckOffset;

    private bool _touchingScout, _touchingSpirit;
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
        WallAndRammerCheck();
        /*
        GameObject rayHit = RayCastHitWall(_interactableLayers);
        if (rayHit != null)
        {
            if ((_groundLayer.value & (1 << rayHit.layer)) > 0)
            {
                HasHitWall = true;
                _rb.velocity = new Vector2(0f, 0f);
            }

            if ((_golemLayer.value & (1 << rayHit.layer)) > 0)
            {
                if (rayHit.TryGetComponent<Rammer>(out Rammer doNotUse))
                {
                    _rb.isKinematic = false;
                }
                else
                {
                    _rb.isKinematic = true;
                }
            }
        }
        */

        //if (HasHitWall && RayCastHitGround())
        //{
        //    _rb.isKinematic = true;
        //}

        if (!_touchingScout && !_touchingSpirit) _rb.isKinematic = HasHitWall && RayCastHitGround();
    }

    private bool RayCastHitGround()
    {
        RaycastHit2D[] raycastHit = new RaycastHit2D[3];

        raycastHit[0] = Physics2D.Raycast(_collider.bounds.center, Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer);

        raycastHit[1] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x + _collider.bounds.extents.x, _collider.bounds.center.y), Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer);

        raycastHit[2] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x, _collider.bounds.center.y), Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer);

        foreach (var hit in raycastHit)
        {
            if (hit.collider != null)
            {
                return true;
            }
        }
        return false;
    }
    private GameObject RayCastHitWall(LayerMask layer)
    {
        RaycastHit2D[] raycastHit = new RaycastHit2D[3];

        raycastHit[0] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y), Vector2.right, 2 * _collider.bounds.extents.x + 2 * _groundCheckOffset, layer);

        raycastHit[1] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y + _collider.bounds.extents.y), Vector2.right, 2 * _collider.bounds.extents.x + 2 * _groundCheckOffset, layer);

        raycastHit[2] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y - _collider.bounds.extents.y), Vector2.right, 2 * _collider.bounds.extents.x + 2 * _groundCheckOffset, layer);

        foreach (var hit in raycastHit)
        {
            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawCube(_collider.bounds.center, _collider.size * 0.9f + Vector2.right * _collider.size.x * _wallCheckOffset);
    //}

    private void WallAndRammerCheck()
    {
        HasHitWall = false;
        _touchingScout = false;
        _touchingSpirit = false;
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

            else if ((_golemLayer.value & (1 << col.gameObject.layer)) > 0)
            {    
                if (col.TryGetComponent<Rammer>(out Rammer doNotUse)) _rb.isKinematic = false;
                else
                {
                    _rb.isKinematic = true;
                    _touchingScout = true;
                }
            }

            else if ((_spiritLayer.value & (1 << col.gameObject.layer)) > 0)
            { 
                _rb.isKinematic = true;
                _touchingSpirit = true;
            }
        }
    }
}
