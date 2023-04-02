using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private Dialogue _dialogue;

    public Dialogue GetDialogue()
    {
        _dialogue.ResetDialogue();

        return _dialogue;
    }
}
