using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;
    public UnityEvent OnDialogueEnd;

    [SerializeField] private float _typeAnimationSpeed;
    [SerializeField] private Image _emoteImg, _bgImg;
    private TextMeshProUGUI _text;
    private Dialogue _currentDialogue;
    private float _typeAnimationDelay;
    Phrase _currentPhrase;
    private bool _skip = false, _justStartedDialogue = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _text = _bgImg.GetComponentInChildren<TextMeshProUGUI>();
        _emoteImg.gameObject.SetActive(false);
        _bgImg.gameObject.SetActive(false);
        _currentDialogue = null;
        _currentPhrase = null;
        _typeAnimationDelay = 1f / _typeAnimationSpeed;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        _emoteImg.gameObject.SetActive(true);
        _bgImg.gameObject.SetActive(true);
        _currentDialogue = dialogue;
        _text.text = "";
        _currentPhrase = _currentDialogue.NextPhrase();
        _emoteImg.sprite = _currentPhrase.Emote;
        _justStartedDialogue = true;
        StartCoroutine(TypeAnimation());
    }

    private void EndDialogue()
    {
        _emoteImg.gameObject.SetActive(false);
        _bgImg.gameObject.SetActive(false);
        _currentDialogue = null;
        _currentPhrase = null;

        OnDialogueEnd.Invoke();
    }

    private void Update()
    {
        if (!_currentDialogue) return;

        if (Input.GetButtonDown("Interact") && !_justStartedDialogue)
        {
            if (!_currentPhrase.IsOver) _skip = true;

            else if(!_currentPhrase.IsLast)
            {
                _text.text = "";
                _currentPhrase = _currentDialogue.NextPhrase();
                _emoteImg.sprite = _currentPhrase.Emote;
                StartCoroutine(TypeAnimation());
            }

            else
            {
                EndDialogue();
            }
        }
        if (_justStartedDialogue) _justStartedDialogue = false;
    }

    private IEnumerator TypeAnimation()
    {
        while (!_currentPhrase.IsOver)
        {
            _text.text += _currentPhrase.NextChar();
            if(!_skip) yield return new WaitForSeconds(_typeAnimationDelay);
        }
        if(_skip)
        {
            _skip = false;
        }
    }
}
