using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VacuumResizer : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _trigger;
    [SerializeField] private ParticleSystemForceField _spiritFF, _vortexFF;
    [SerializeField] private Transform _vortexParticles;
    [SerializeField] private float _scale = 1f;

    private bool _started = false;
    private float _angle, _lastScale = 1f;

    private float _initialTriggerRadius, _initialSpiritFFRadius, _initialVortexFFRadius, _initialVortexParticlesRadius;

    void Start()
    {
        if (Application.IsPlaying(gameObject) || _started) return;

        _started = true;
        _initialTriggerRadius = _trigger.radius;
        _initialSpiritFFRadius = _spiritFF.endRange;
        _initialVortexFFRadius = _vortexFF.endRange;
        _initialVortexParticlesRadius = 1.6f;

        _angle = 360f / _vortexParticles.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.IsPlaying(gameObject)) return;
        if (_scale == _lastScale) return;

        //_trigger.radius = _initialTriggerRadius * _scale;
        //_spiritFF.endRange = _initialSpiritFFRadius * _scale;
        //_vortexFF.endRange = _initialVortexFFRadius * _scale;

        _angle = 360f / _vortexParticles.childCount;

        for (int i = 0; i < _vortexParticles.childCount; i++)
        {
            Vector2 newPos = new Vector2(Mathf.Cos(Mathf.Deg2Rad * i * _angle), Mathf.Sin(Mathf.Deg2Rad * i * _angle));
            _vortexParticles.GetChild(i).localPosition = newPos * _initialVortexParticlesRadius * _scale;
        }


        _lastScale = _scale;
    }
}
