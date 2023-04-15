using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clues : ScriptableObject
{
    public List<Clue> CluesList;

    public string GetClue(string sceneName)
    {
        foreach(Clue clue in CluesList)
        {
            if(clue.SceneName == sceneName)
            {
                return clue.ClueText;
            }
        }
        return null;
    }
}

[System.Serializable]
public class Clue
{
    public string SceneName;
    [TextArea(5, 10)]public string ClueText;
}

