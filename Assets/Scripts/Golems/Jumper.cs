using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : Golem
{
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private float _minJumpForce, _maxJumpForce, _minJumpTime, _maxJumpTime, _gravityForce;


    private Vector2 _gravity;

    private float _horizontalInput, _nextHorizontalInput;
    private bool _isDelayingHorizontalInput = false;
    private float _horizontalDelayTimer = 0f;

    private bool _isHoldingJumpButton = false;
    private float _holdingJumpButtonTime = 0f;

    private bool _isGrounded = false;
    


    protected override void Awake()
    {
        base.Awake();

        _gravity = _gravityForce * 9.81f * Vector2.down;
    }

    private void Update()
    {
        if (State == GolemState.Available && _isGrounded)
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
        }
        if (State != GolemState.Enabled) return;

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
        if (State != GolemState.Enabled && State != GolemState.Available) return;

        if (!_isGrounded) _rb.velocity += _gravity * Time.fixedDeltaTime;
    }

    private void Jump()
    {
       
        _isGrounded = false;
        _isHoldingJumpButton = false;

        _rb.SetRotation(0f);

        //(valor de x - a) / (b - a)
        float jumpForceFactor = _holdingJumpButtonTime <= _minJumpTime ? 0f : (_holdingJumpButtonTime - _minJumpTime) / (_maxJumpTime - _minJumpTime);

        float jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, jumpForceFactor);

        Vector2 jumpDirection = new Vector2(_horizontalInput / 2f, 1f).normalized;
        

        _rb.velocity = jumpDirection * jumpForce;


        _holdingJumpButtonTime = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State != GolemState.Enabled && State != GolemState.Available) return;

        if ((_groundLayers.value & (1 << collision.gameObject.layer)) <= 0) return;

        if(collision.contacts[0].normal.y >= 0f)
        {
            _isGrounded = true;
            _rb.velocity = new Vector2(0f, 0f);
            _rb.SetRotation(Vector2.SignedAngle(transform.up, collision.contacts[0].normal));
        }
    }
}
