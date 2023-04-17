using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SpiritDim : MonoBehaviour
{
    public bool IsFading
    {
        get { return _isFading;  }
        set
        {
            if (!_isFading && value)
            {
                _sprite.localScale = _initialScale;
                _light.pointLightInnerRadius = _initialInnerRadius;
                _light.pointLightOuterRadius = _initialOuterRadius;
                _mainModule.startSizeMultiplier = _initialFTSize;
            }
            
            _isFading = value;
        }
    }
    public bool IsAtLevel1 = false;

    [SerializeField] private Transform _sprite;
    [SerializeField] private float _attenuationDuration, _deathDuration;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _dieSound;
    [SerializeField] private Light2D _light;
    [SerializeField] private ParticleSystem _fireTrail;

    private SpiritMovement _spiritMovement;
    private SpiritUnion _spiritUnion;
    private bool _isFading = true, _isDying = false;
    private Vector3 _scaleDecrement, _initialScale;
    private float _initialOuterRadius, _initialInnerRadius, _initialFTSize;
    private ParticleSystem.MainModule _mainModule;

    private void Awake()
    {
        
        _scaleDecrement = new Vector3(Time.fixedDeltaTime, Time.fixedDeltaTime, Time.fixedDeltaTime) / _attenuationDuration;
        _initialScale = _sprite.localScale;
        _initialInnerRadius = _light.pointLightInnerRadius;
        _initialOuterRadius = _light.pointLightOuterRadius;
        _mainModule = _fireTrail.main;
        _initialFTSize = _mainModule.startSizeMultiplier;

        _spiritMovement = GetComponent<SpiritMovement>();
        _spiritUnion = GetComponentInChildren<SpiritUnion>();

        IsFading = true;

    }

    private void FixedUpdate()
    {
        if (!IsFading || _isDying) return;

        if (_sprite.localScale.x > 0f)
        {
            _sprite.localScale -= _scaleDecrement;
            _light.pointLightInnerRadius -= _scaleDecrement.x * 1.1f;
            _light.pointLightOuterRadius -= _scaleDecrement.x * 1.1f;
            _mainModule.startSizeMultiplier -= _scaleDecrement.x * _initialFTSize;
        }

        else StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        if (_spiritUnion.State != SpiritState.Roaming)
        {
            IsFading = false;
            yield break;
        }

        _isDying = true;

        _spiritMovement.CanMove = false;
        _spiritUnion.CanInteract = false;
        _spiritUnion.CanSwap = false;

        _audioSource.PlayOneShot(_dieSound); 
        _light.enabled = false;
        _fireTrail.Stop();

        yield return new WaitForSeconds(_deathDuration);

        if (!IsAtLevel1) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else SceneManager.LoadScene("Desert1_NC");
    }
}
