using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform _cameraTranform;
    private Vector3 _lastCameraPosition;

    [SerializeField] private Vector2 _parallaxMultiplier;

    private void Start()
    {
        _cameraTranform = FindObjectOfType<CameraController>().transform;
        _lastCameraPosition = _cameraTranform.position;
    }

    private void LateUpdate()
    {
        Debug.Log("LateUpdate");
        Vector3 deltaMovement = _cameraTranform.position - _lastCameraPosition;
        transform.position = new Vector3(deltaMovement.x * _parallaxMultiplier.x, deltaMovement.y * _parallaxMultiplier.y, deltaMovement.z);
        _lastCameraPosition = _cameraTranform.position;
    }
}
