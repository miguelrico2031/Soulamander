using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameStartCam _gsc;
    [SerializeField] private GameObject _credits, _mainMenu, _settings, _playButton;


    private void Start()
    {
        Cursor.visible = true;

        PauseGame.Instance.enabled = false;

        EventSystem.current.SetSelectedGameObject(_playButton);

        Music.Instance.PlayMenuMusic();

    }

    public void PlayGame()
    {
        Cursor.visible = false;
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
                EventSystem.current.SetSelectedGameObject(_playButton);
            }
            if(_settings.activeSelf)
            {
                _settings.SetActive(false);
                _mainMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_playButton);
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
