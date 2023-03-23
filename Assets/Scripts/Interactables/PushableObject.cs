using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayers;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _golemLayer;
    //[SerializeField] private float _gravityForce;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _groundCheckOffset;

    [HideInInspector] public bool HasHitWall;

    private bool _isGrounded;

    
    private void FixedUpdate()
    {
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
        
        if (HasHitWall)
        {
            if (RayCastHitGround())
            {
                _rb.isKinematic = true;
            }
        }
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
}
