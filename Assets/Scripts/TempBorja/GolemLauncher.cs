using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class GolemLauncher : MonoBehaviour
{
    [SerializeField] float _launchVelocity;
    [SerializeField] float _maxAngle;
    [SerializeField] float _initialAngle;
    [SerializeField] float _rotationSpeed;
    [SerializeField] Transform _golemHolder;

    private Vector2 _direction;
    [SerializeField] private GameObject _golem;
    private float _angle;
    private void Awake()
    {
        _angle = _initialAngle;
    }

    public void GetGolem(GameObject golem)
    {
        _golem = golem;
    }
    private void Launch()
    {
        _golem.GetComponent<Rigidbody2D>().velocity = new Vector2(_launchVelocity * _direction.x, _launchVelocity * _direction.y);
        _golem = null;
    }
    
    private void Update()
    {
        if (_golem != null)
        {
            _golem.transform.position = _golemHolder.position;
            _golem.transform.rotation = _golemHolder.rotation;

            _angle = Mathf.Clamp(_angle - (int)Input.GetAxis("Horizontal") * _rotationSpeed * Time.deltaTime, -_maxAngle, _maxAngle);
            
            if (Input.GetButtonDown("Jump"))
            {
                Launch();
            }
        }

        _direction = transform.up;

        transform.rotation = Quaternion.Euler(0, 0, _angle);
    }
}
