using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _gravityForce;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _groundCheckOffset;

    [HideInInspector] public bool HasHitWall;

    private bool _isGrounded;   

    private void FixedUpdate()
    {
        if (_isGrounded)
        {
            if (!RayCastHitGround())
            {
                _isGrounded = false;
            }            
        }
        if (!_isGrounded) _rb.velocity = new Vector2(_rb.velocity.x, (_rb.velocity.y + _gravityForce * -9.81f * Time.fixedDeltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_groundLayer.value & (1 << collision.gameObject.layer)) <= 0) return;

        if (collision.contacts[0].normal.y > 0f)
        {
            _isGrounded = true;
            _rb.velocity = new Vector2(0f, 0f);
        }
        if (collision.contacts[0].normal.x != 0f)
        {
            HasHitWall = true;
            _rb.velocity = new Vector2(0f, 0f);
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
}
