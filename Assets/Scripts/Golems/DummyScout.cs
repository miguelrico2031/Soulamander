using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScout : MonoBehaviour
{
    private Animator _animator;
    private bool _move = false;
    private float t = 0f, _speed = 0f;
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void DummyStart(Scout scout)
    {
        _animator.SetBool("Enabled", true);
        _speed = scout.GetSpeed();  
        _animator.SetFloat("Speed", _speed / 1.5f);

        transform.position = scout.transform.position;
        transform.localScale = scout.transform.localScale;

        _move = true;

        scout.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!_move) return;

        transform.position =
            Vector2.LerpUnclamped(transform.position, transform.position - transform.right, t);

        t += Time.fixedDeltaTime * _speed / 30f;
    }
}
