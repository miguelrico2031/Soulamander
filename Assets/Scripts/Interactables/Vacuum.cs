using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : MonoBehaviour
{
    [SerializeField] private float _spiritSuckDuration;
    private int _spiritLayer, _golemLayer;
    private Golem _golemBeingSucked;
    private SpiritUnion _spiritUnion;
    private float _suckTime = 0f;

    private void Awake()
    {
        _spiritLayer = LayerMask.NameToLayer("Spirit");
        _golemLayer = LayerMask.NameToLayer("Golem");
    }

    private void Start()
    {
        _spiritUnion = GameObject.FindAnyObjectByType<SpiritUnion>();
    }

    private void Update()
    {
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
        }
    }
}
