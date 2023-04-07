using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffector : MonoBehaviour
{
    private CameraController _camera;
    private bool _isActive;

    [Header("Instance Settings")]
    [SerializeField] private bool _onlyActivateOnce;
    [SerializeField] private bool _changeToFollowPlayerX;
    [SerializeField] private bool _changeToFollowPlayerY;
    [SerializeField] private float _newCameraSize;
    [SerializeField] private Transform _newCameraBounds;
    [SerializeField] private float _timerBeforeReturnPreviousValues; // si timer == 0 se considera que no hay timer (el cambio se mantiene para siempre)
    [SerializeField] private Transform _newCamStaticPos;

    [Header("Unity Setup")]
    [SerializeField] private LayerMask _golemLayer;    

    private void Start()
    {
        _isActive = true;
        _camera = GameObject.FindObjectOfType<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.gameObject.GetComponent<Golem>().State != GolemState.Enabled) return;
        ActivateEffector(0);
    }

    public void ActivateEffector(float returnValues)
    {
        if (!_isActive) return;
        if (_onlyActivateOnce) _isActive = true;

        _camera.OnEffector(_changeToFollowPlayerX, _changeToFollowPlayerY, _newCamStaticPos.position, _newCameraSize, _newCameraBounds, _timerBeforeReturnPreviousValues + returnValues);
    }
}
