using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private Dialogue _dialogue;

    private SpriteRenderer _renderer;

    [SerializeField] private SpriteRenderer[] _otherRenderers;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public Dialogue GetDialogue()
    {
        _dialogue.ResetDialogue();

        return _dialogue;
    }

    public void SetMaterial(Material material)
    {

        if (_otherRenderers.Length == 0) _renderer.material = material;

        else foreach (var renderer in _otherRenderers) renderer.material = material;
    }
}
