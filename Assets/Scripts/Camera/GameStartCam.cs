using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class GameStartCam : MonoBehaviour
{
    [SerializeField] private CameraController _camController;
    [SerializeField] private Transform _centerGolemTarget;
    [SerializeField] private ParticleSystem _dirtEffect;
    [SerializeField] private Light2D _sunLight, _globalLight;
    [SerializeField] private Animator _initialAnimator;

    private PixelPerfectCamera _ppCamera;

    private void Awake()
    {
        _ppCamera = GetComponent<PixelPerfectCamera>();
    }

    private void Start()
    {
        StartCoroutine(StartCinematic());
    }

    private IEnumerator StartCinematic()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos;
        endPos.x = _centerGolemTarget.position.x;
        float t = 0f;

        StartCoroutine(Zoom(64, 0.55f));

        while (Vector2.Distance(transform.position, endPos) > 0.02f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / 3f;
            yield return null;
        }

        yield return new WaitForSeconds(2f);


        _dirtEffect.Play();
        yield return new WaitUntil(() => !_dirtEffect.isPlaying);
        yield return new WaitForSeconds(1f);

        _dirtEffect.Play();
        while (_dirtEffect.isPlaying)
        {
            _sunLight.intensity += 0.008f;
            yield return new WaitForSeconds(_dirtEffect.main.duration / 20f);
        }
        yield return new WaitForSeconds(1f);

        _dirtEffect.Play();
        float startI = _sunLight.intensity;
        t = 0f;
        while (_sunLight.intensity < 1f)
        {
            _sunLight.intensity = Mathf.Lerp(startI, 1f, t / _dirtEffect.main.duration);
            _globalLight.intensity = Mathf .Lerp(0f, 0.8f, t / _dirtEffect.main.duration);
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        _initialAnimator.SetTrigger("Light");

    }

    public void OnLighted()
    {

    }

    private IEnumerator Zoom(int targetPPU, float zoomSpeed)
    {
        //int i = 0;
        //while (i < 5) // goofy bugfix
        //{
        //    yield return new WaitForSeconds(Time.deltaTime);
        //    i++;
        //}
        bool add = false;
        int orthograficToPPUConversionConstant = Mathf.RoundToInt(_ppCamera.assetsPPU * Camera.main.orthographicSize);
        //Debug.Log(_ppCamera.assetsPPU + " * " + _camera.orthographicSize + " = " + orthograficToPPUConversionConstant);
        if (targetPPU < _ppCamera.assetsPPU) add = true;
        _ppCamera.enabled = false;
        while (!add)
        {

            Camera.main.orthographicSize -= zoomSpeed * Time.deltaTime;

            if (orthograficToPPUConversionConstant / Camera.main.orthographicSize >= targetPPU) break;

            yield return null;
        }
        while (add)
        {

            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;

            if (orthograficToPPUConversionConstant / Camera.main.orthographicSize <= targetPPU) break;

            yield return null;
        }
        _ppCamera.enabled = true;
        _ppCamera.assetsPPU = targetPPU;
    }
}
