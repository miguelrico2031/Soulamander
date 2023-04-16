using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeField] private Phrase[] _phrases;
    public GameZone GameZone;
    private int _index = 0;

    public Phrase NextPhrase()
    {
        Phrase phrase = _phrases[_index++];
        phrase.IsOver = false;

        if (_index == _phrases.Length)
        {
            phrase.IsLast = true;
            _index = 0;
        }
        return phrase;
    }

    public void ResetDialogue()
    {
        _index = 0;
        foreach (var phrase in _phrases) phrase.ResetPhrase();
    }
}


[System.Serializable]
public class Phrase
{

    [TextArea(6, 10)]public string Text;
    public Sprite Emote;
    [HideInInspector] public bool IsLast = false, IsOver = false;
    private int _index = 0;

    public char NextChar()
    {
        char c = Text[_index++];
        if(_index == Text.Length)
        {
            IsOver = true;
            _index = 0;
        }
        return c;
    }

    public void ResetPhrase()
    {
        IsLast = false;
        IsOver = false;
        _index = 0;
    }
}

[System.Serializable]
public enum GameZone
{
    Desert, Moss, City
}

