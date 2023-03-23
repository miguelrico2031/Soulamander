using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    //[SerializeField] private float _gravityForce;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _groundCheckOffset;
    [SerializeField] private GameObject _topCollider;
    [SerializeField] private Transform _topColliderPos;

    [HideInInspector] public bool HasHitWall;

    private bool _isGrounded;
    /*
    private void Awake()
    {
        _topColliderPos = _topCollider.transform;
    }
    private void Update()
    {
        _topCollider.transform.position = _topColliderPos.position;
    }
    */
    private void FixedUpdate()
    {
        //if (!RayCastHitGround()) _rb.velocity = new Vector2(_rb.velocity.x, (_rb.velocity.y + _gravityForce * -9.81f * Time.fixedDeltaTime));
        if (RayCastHitWall(_groundLayer))
        {
            HasHitWall = true;
            _rb.velocity = new Vector2(0f, 0f);
        }
        if (HasHitWall)
        {
            if (RayCastHitGround())
            {
                _rb.isKinematic = true;
            }
        }
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_groundLayer.value & (1 << collision.gameObject.layer)) <= 0) return;

        if (collision.contacts[0].normal.y > 0f)
        {
            
            _isGrounded = true;
            _rb.velocity = new Vector2(0f, 0f);
        }
        if (Mathf.Abs(collision.contacts[0].normal.x) == 1f)
        {
            HasHitWall = true;
            Debug.Log("choque");
            _rb.velocity = new Vector2(0f, 0f);
        }
    }
    */

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
    private bool RayCastHitWall(LayerMask layer)
    {
        RaycastHit2D[] raycastHit = new RaycastHit2D[3];

        raycastHit[0] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y), Vector2.right, 2 * _collider.bounds.extents.x + 2 * _groundCheckOffset, layer);
        Debug.DrawRay(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y), Vector2.right * (2 * _collider.bounds.extents.x + 2 * _groundCheckOffset));
        raycastHit[1] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y + _collider.bounds.extents.y), Vector2.right, 2 * _collider.bounds.extents.x + 2 * _groundCheckOffset, layer);
        Debug.DrawRay(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y + _collider.bounds.extents.y), Vector2.right * (2 * _collider.bounds.extents.x + 2 * _groundCheckOffset));
        raycastHit[2] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y - _collider.bounds.extents.y), Vector2.right, 2 * _collider.bounds.extents.x + 2 * _groundCheckOffset, layer);
        Debug.DrawRay(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x - _groundCheckOffset, _collider.bounds.center.y - _collider.bounds.extents.y), Vector2.right * (2 * _collider.bounds.extents.x + 2 * _groundCheckOffset));
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
