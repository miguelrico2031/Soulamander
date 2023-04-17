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
    [SerializeField] private GameObject _spirit;
    [SerializeField] private float _zoomOutSpeed = 0.4f;

    private PixelPerfectCamera _ppCamera;

    private void Awake()
    {
        _ppCamera = GetComponent<PixelPerfectCamera>();
    }

    public void GameStart()
    {
        StartCoroutine(StartCinematic());
    }

    private IEnumerator StartCinematic()
    {
        yield return new WaitForSeconds(2.5f);

        Vector3 startPos = transform.position;
        Vector3 endPos = _centerGolemTarget.position;
        endPos.z = startPos.z;
        float t = 0f;

        StartCoroutine(Zoom(64, _zoomOutSpeed));

        while (Vector2.Distance(transform.position, endPos) > 0.02f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / 3f;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        var dirtSound = _dirtEffect.GetComponent<AudioSource>();
        _dirtEffect.Play();
        dirtSound.Play();
        yield return new WaitUntil(() => !_dirtEffect.isPlaying);
        yield return new WaitForSeconds(1f);

        _dirtEffect.Play();
        dirtSound.Play();
        while (_dirtEffect.isPlaying)
        {
            _sunLight.intensity += 0.008f;
            yield return new WaitForSeconds(_dirtEffect.main.duration / 20f);
        }
        yield return new WaitForSeconds(1f);

        _dirtEffect.Play();
        dirtSound.Play();
        float startI = _sunLight.intensity;
        t = 0f;
        while (_sunLight.intensity < 1f)
        {
            _sunLight.intensity = Mathf.Lerp(startI, 1.1f, t / _dirtEffect.main.duration);
            _globalLight.intensity = Mathf .Lerp(0f, 0.8f, t / _dirtEffect.main.duration);
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(3.5f);

        _initialAnimator.SetTrigger("Light");

        yield return new WaitForSeconds(6f);

        _initialAnimator.SetTrigger("Off");
        _spirit.SetActive(true);
        _spirit.GetComponent<SpiritDim>().IsFading = false;
        PauseGame.Instance.enabled = true;
        
        StartCoroutine(Zoom(32, 0.7f));

        startPos = transform.position;
        endPos = _spirit.transform.position;
        endPos.z = startPos.z;
        while (Vector2.Distance(transform.position, endPos) > 0.0f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime;
            yield return null;
        }

        _camController.enabled = true;
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
