using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : MonoBehaviour
{
    [SerializeField] private float _spiritSuckDuration, _rotationSpeed, _vortexRotationMultiplier, _soundFadeDuration;

    [SerializeField] private Transform _vortex;

    private AudioSource _audioSource;
    private int _spiritLayer, _golemLayer;
    private Golem _golemBeingSucked;
    private SpiritUnion _spiritUnion;
    private float _suckTime = 0f;

    private void Awake()
    {
        _spiritLayer = LayerMask.NameToLayer("Spirit");
        _golemLayer = LayerMask.NameToLayer("Golem");
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _spiritUnion = GameObject.FindAnyObjectByType<SpiritUnion>();
        _audioSource.volume = 0f;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        if (_vortex) _vortex.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime* _vortexRotationMultiplier);

        if (!_golemBeingSucked) return;

        _suckTime += Time.deltaTime;

        if(_suckTime >= _spiritSuckDuration)
        {
            if(_golemBeingSucked.State == GolemState.Enabled) _spiritUnion.VacuumSpirit(transform);
            _golemBeingSucked = null;
        }

        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.layer == _spiritLayer) _spiritUnion.VacuumSpirit(transform);
        
        else if(collider.gameObject.layer == _golemLayer)
        {
           Golem golem = collider.GetComponent<Golem>();

            if (golem.State != GolemState.Enabled) return;

            _golemBeingSucked = golem;
            _suckTime = 0f;
            _spiritUnion.CanSwap = false;
            _spiritUnion.SuckSpirit(true);
            StartCoroutine(SoundFade(true));
        }
    }


    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer != _golemLayer) return;

        Golem golem = collider.GetComponent<Golem>();

        if(golem == _golemBeingSucked)
        {
            _golemBeingSucked = null;
            _spiritUnion.CanSwap = true;
            _spiritUnion.SuckSpirit(false);
            StartCoroutine(SoundFade(false));
        }
    }

    private IEnumerator SoundFade(bool fadeIn)
    {
         _audioSource.volume = fadeIn ? 0f : 1f;
        float timer = 0f;
        while(timer < _soundFadeDuration)
        {
            timer += Time.deltaTime;

            if(fadeIn) _audioSource.volume += Time.deltaTime / _soundFadeDuration;
            else _audioSource.volume -= Time.deltaTime / _soundFadeDuration;

            yield return null;
        }
        
    }
}
