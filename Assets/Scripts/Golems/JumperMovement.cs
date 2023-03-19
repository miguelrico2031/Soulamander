using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperMovement : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private float _minJumpForce, _maxJumpForce, _minJumpTime, _maxJumpTime, _gravityForce;
    [SerializeField] private Transform _groundCheck;

    private Rigidbody2D _rb;
    private float _horizontalInput, _nextHorizontalInput;
    private float _holdingJumpButtonTime = 0f;


    private bool _isGrounded = false;
    private bool _isHoldingJumpButton = false;
    private bool _isDelayingHorizontalInput = false;

    private float _horizontalDelayTimer = 0f;

    private Vector2 _gravity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gravity = Vector2.down * 9.81f * _gravityForce;
    }

    private void Update()
    {

        if (Input.GetButtonDown("Jump") && _isGrounded) _isHoldingJumpButton = true;

        if (Input.GetButtonUp("Jump") && _isGrounded) Jump();
        else
        {
            if (!_isDelayingHorizontalInput)
            {
                _horizontalInput = _nextHorizontalInput;
                _nextHorizontalInput = Input.GetAxisRaw("Horizontal");
            }

            if (_nextHorizontalInput == 0 && _horizontalInput != 0) _isDelayingHorizontalInput = true;

            if (_isDelayingHorizontalInput)
            {
                _horizontalDelayTimer += Time.deltaTime;
                if(_horizontalDelayTimer >= 0.1f)
                {
                    _isDelayingHorizontalInput = false;
                    _horizontalDelayTimer = 0f;
                }   
            }   
        }

        if (_isHoldingJumpButton && _holdingJumpButtonTime < _maxJumpTime) _holdingJumpButtonTime += Time.deltaTime;

    }

    private void FixedUpdate()
    {
        if (_isGrounded) return;
        _rb.velocity += _gravity * Time.fixedDeltaTime;
    }

    private void Jump()
    {
        _isGrounded = false;
        _isHoldingJumpButton = false;

        //(valor de x - a) / (b - a)
        float jumpForceFactor = _holdingJumpButtonTime <= _minJumpTime ? 0f : (_holdingJumpButtonTime - _minJumpTime) / (_maxJumpTime - _minJumpTime);

        float jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, jumpForceFactor);

        Vector2 jumpDirection = new Vector2(_horizontalInput / 2f, 1f).normalized;
        

        _rb.velocity = jumpDirection * jumpForce;


        _holdingJumpButtonTime = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject);
        if ((_groundLayers.value & (1 << collision.gameObject.layer)) <= 0) return;

        if(collision.contacts[0].normal.y >= 0f)
        {
            _isGrounded = true;
            _rb.velocity = new Vector2(0f, 0f);
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if ((_groundLayers.value & (1 << collision.gameObject.layer)) <= 0) return;
    //    if (collision.contacts.Length > 0f && collision.contacts[0].normal.y >= 0f)
    //    {
    //        ContactFilter2D filter = new ContactFilter2D();
    //        filter.SetLayerMask(_groundLayers);
    //        List<Collider2D> contacts = new List<Collider2D>();
    //        _rb.OverlapCollider(filter, contacts);
    //        foreach (var contact in contacts) if (contact.gameObject != this) return;

    //        _isGrounded = false;
    //        _rb.gravityScale = _defaultGravity;
    //    }
    //}
}
