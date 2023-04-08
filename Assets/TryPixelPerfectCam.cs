using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using /*UnityEngine.U2D;
*/UnityEngine.Experimental.Rendering.Universal;

public class TryPixelPerfectCam : MonoBehaviour
{
    private Camera _cam;
    private PixelPerfectCamera _pixelPerfectCamera;

    [SerializeField] private int _initialPPU, _targetPPU;
    [SerializeField] private float _zoomSpeed;

    private float t = 0f;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        _pixelPerfectCamera.assetsPPU = _initialPPU;

        StartCoroutine(PixelPerfectZoom());
    }
    void Update()
    {


        t += Time.deltaTime * 0.1f;
    }

    private IEnumerator PixelPerfectZoom()
    {
        _pixelPerfectCamera.enabled = false;

        while(true)
        {
            _cam.orthographicSize -= _zoomSpeed;

            //Debug.Log(_cam.orthographicSize * 270);
            if (270 / _cam.orthographicSize >= _targetPPU) break;

            yield return null;
        }
        Debug.Log("over tal");
        _pixelPerfectCamera.enabled = true;
        _pixelPerfectCamera.assetsPPU = _targetPPU;
    }
}
