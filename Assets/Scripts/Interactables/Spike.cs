using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spike : MonoBehaviour
{
    [SerializeField] private float _respawnDelay;
    [SerializeField] private Respawn _respawn;
    [SerializeField] private ParticleSystem _deathParticles;
    [SerializeField] private AudioClip _deathSound;

    private int _golemLayer;
    private SpiritUnion _spiritUnion;
    private ParticleSystem _deathParticlesInstance;

    void Awake()
    {
        _golemLayer = LayerMask.NameToLayer("Golem");
        _spiritUnion = FindAnyObjectByType<SpiritUnion>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer != _golemLayer) return;

        StartCoroutine(DieAndRespawn(collider.gameObject));
    }

    private IEnumerator DieAndRespawn(GameObject golemGO)
    {
        Golem golem = golemGO.GetComponent<Golem>();

        if (golem.IsRespawning) yield break;

        golem.IsRespawning = true;

        Vector2 _deathParticlesPos = golem.transform.position;
        _deathParticlesInstance = Instantiate(_deathParticles, _deathParticlesPos, _deathParticles.transform.rotation);
        GetComponent<AudioSource>().PlayOneShot(_deathSound);

        if (_respawn != null) golem.transform.position = _respawn.GetRespawnPoint();

        if (golem.State == GolemState.Enabled)
        {
            _spiritUnion.transform.parent.position = golem.transform.position;
            _spiritUnion.transform.parent.gameObject.SetActive(false);
        }

        golemGO.SetActive(false);

        yield return new WaitForSeconds(_respawnDelay);

        golem.IsRespawning = false;

        if (_respawn == null) StartCoroutine(ReloadScene());
        else
        {
            golemGO.SetActive(true);
            _spiritUnion.transform.parent.gameObject.SetActive(true);

            if (golem.State == GolemState.Enabled)
            {
                golem.State = GolemState.Enabled;
                _spiritUnion.State = SpiritState.Possessing;
            }
            else if (golem.State == GolemState.Available) golem.State = GolemState.Available;
        }
    }

    private IEnumerator ReloadScene()
    {      
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null;
    }
}
