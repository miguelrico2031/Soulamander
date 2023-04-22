using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform _cameraTranform;
    private Vector3 _lastCameraPosition;

    private bool _activated;

    [SerializeField] private Vector2 _parallaxMultiplier;
    [SerializeField] private float _startDelay;

    private void Start()
    {
        _cameraTranform = FindObjectOfType<CameraController>().transform;
        _lastCameraPosition = _cameraTranform.position;
        StartCoroutine(OnStart());
    }

    IEnumerator OnStart()
    {
        yield return new WaitForSeconds(_startDelay);

        _activated = true;
    }

    private void LateUpdate()
    {
        if (!_activated)
        {
            Vector3 deltaMovement = _cameraTranform.position - _lastCameraPosition;
            transform.position = new Vector3(transform.position.x + deltaMovement.x, transform.position.y + deltaMovement.y, transform.position.z);
            if (deltaMovement != Vector3.zero) _lastCameraPosition = _cameraTranform.position;
        }
        else
        {
            Vector3 deltaMovement = _cameraTranform.position - _lastCameraPosition;
            transform.position = new Vector3(transform.position.x + deltaMovement.x * _parallaxMultiplier.x, transform.position.y + deltaMovement.y * _parallaxMultiplier.y, transform.position.z);
            if (deltaMovement != Vector3.zero) _lastCameraPosition = _cameraTranform.position;
        }        
    }
}
