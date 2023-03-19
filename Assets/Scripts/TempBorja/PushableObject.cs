using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private float _gravityForce;
    [SerializeField] private Rigidbody2D _rb;

    private bool _isGrounded;
    public bool HasHitWall;

    private void FixedUpdate()
    {
        if (!_isGrounded) _rb.velocity += _gravityForce * 9.81f * Vector2.down * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_groundLayers.value & (1 << collision.gameObject.layer)) <= 0) return;
        
        if (collision.contacts[0].normal.y > 0f)
        {
            _isGrounded = true;
            _rb.velocity = new Vector2(0f, 0f);
            _rb.SetRotation(Vector2.SignedAngle(transform.up, collision.contacts[0].normal));
        }
        else if (collision.contacts[0].normal.x != 0f)
        {
            HasHitWall = true;
            _rb.velocity = new Vector2(0f, 0f);
        }
    }
}
