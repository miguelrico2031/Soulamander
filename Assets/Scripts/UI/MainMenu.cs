using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameStartCam _gsc;
    [SerializeField] private GameObject _credits, _mainMenu;

    private void Start()
    {
        PauseGame.Instance.enabled = false;
        EventSystem.current.SetSelectedGameObject(_mainMenu.transform.Find("Play").gameObject);

        Music.Instance.PlayMenuMusic();
    }

    public void PlayGame()
    {
        _gsc.GameStart();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (PauseGame.Instance.enabled) return;

        if (Input.GetButtonDown("Cancel"))
        {
            if(_credits.activeSelf)
            {
                _credits.SetActive(false);
                _mainMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_mainMenu.transform.Find("Play").gameObject);
            }
        }
    }

    public void SetLanguage(bool spanish)
    {
        if (spanish) SetSpanish();
        else SetEnglish();
    }

    private void SetSpanish()
    {
        Debug.Log("español");
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    }

    private void SetEnglish()
    {
        Debug.Log("ingles");
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
    }
}
