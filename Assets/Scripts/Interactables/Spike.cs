using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private float _respawnDelay;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private ParticleSystem _deathParticles;

    private int _golemLayer;
    private GameObject _spirit;
    private ParticleSystem _deathParticlesInstance;

    void Awake()
    {
        _golemLayer = LayerMask.NameToLayer("Golem");
        _spirit = FindAnyObjectByType<SpiritMovement>().gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer != _golemLayer) return;

        StartCoroutine(DieAndRespawn(collider.gameObject));
    }

    private IEnumerator DieAndRespawn(GameObject golemGO)
    {
        Golem golem = golemGO.GetComponent<Golem>();
        _deathParticlesInstance = Instantiate(_deathParticles, golem.transform.position, _deathParticles.transform.rotation);
        golem.transform.position = _respawnPoint.position;
        _spirit.transform.position = _respawnPoint.position;
        golemGO.SetActive(false);
        _spirit.SetActive(false);
        yield return new WaitForSeconds(_respawnDelay);
        _spirit.SetActive(true);
        golemGO.SetActive(true);
        if(golem.State == GolemState.Enabled) golem.State = GolemState.Enabled;
        else if (golem.State == GolemState.Available) golem.State = GolemState.Available;

    }
}