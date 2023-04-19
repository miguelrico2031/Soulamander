using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OtherSpirit : MonoBehaviour
{
    [SerializeField] private float _speed, _rangeIncrease, _intensityIncrease;

    private Transform _spirit;

    private void Update()
    {
        if(!_spirit) return;

        transform.position = Vector2.MoveTowards(transform.position, _spirit.position, Time.deltaTime * _speed);

        if (Vector2.Distance(transform.position, _spirit.position) > 0.05f) return;

        var light = _spirit.GetComponentInChildren<Light2D>();
        light.intensity += _intensityIncrease;
        light.pointLightOuterRadius += _intensityIncrease;

        var scoutLight = FindObjectOfType<Scout>().GetComponentInChildren<Light2D>();
        scoutLight.intensity += _intensityIncrease;
        scoutLight.pointLightOuterRadius += _intensityIncrease;

        EndGame.Instance.SpiritMerge();

        Destroy(gameObject);
    }

    public void Free(Transform spirit)
    {
        transform.parent = null;
        _spirit = spirit;
    }
}
