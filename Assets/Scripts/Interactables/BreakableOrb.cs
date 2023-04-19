using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableOrb : MonoBehaviour
{
    private ParticleSystem _particles;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _trigger;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trigger = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Golem")) return;

        _trigger.enabled = false;
        _spriteRenderer.enabled = false;
        _particles.Play();
        _audioSource.Play();

        var spirit = FindObjectOfType<SpiritMovement>().transform;

        foreach (var other in GetComponentsInChildren<OtherSpirit>()) other.Free(spirit);
    }
}
