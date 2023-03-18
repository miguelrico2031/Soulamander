using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    private bool _isPressingMove = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        _isPressingMove = _moveInput.x != 0f || _moveInput.y != 0f;
    }

    private void FixedUpdate()
    {
        if(_isPressingMove) _rb.MovePosition(_rb.position + _moveInput.normalized * _moveSpeed * Time.fixedDeltaTime);
    }
}
