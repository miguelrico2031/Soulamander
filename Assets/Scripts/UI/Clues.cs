using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Clues : ScriptableObject
{
    public List<Clue> CluesList;
    [SerializeField] private UnityEngine.Localization.Locale _spanish;

    public string GetClue(string sceneName)
    {
        foreach(Clue clue in CluesList)
        {
            if(clue.SceneName == sceneName)
            {
                
                if (LocalizationSettings.SelectedLocale.Identifier.ToString() == "Spanish(es)") return clue.ClueText;
                else return clue.ClueTextEnglish;
            }
        }
        return null;
    }
}

[System.Serializable]
public class Clue
{
    public string SceneName;
    [TextArea(5, 10)]public string ClueText, ClueTextEnglish;
}

