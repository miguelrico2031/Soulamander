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
    [SerializeField] private Toggle _spanishToggle, _englishToggle;


    private IEnumerator Start()
    {

        PauseGame.Instance.enabled = false;

        EventSystem.current.SetSelectedGameObject(_playButton);

        Music.Instance.PlayMenuMusic();

        yield return null;

        Cursor.visible = true;
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

        if(EventSystem.current.currentSelectedGameObject) return;

        if (_credits.activeSelf) EventSystem.current.SetSelectedGameObject(_credits.GetComponentInChildren<Button>().gameObject);
        else if (_settings.activeSelf) EventSystem.current.SetSelectedGameObject(_settings.transform.Find("Cancel").gameObject);
        else EventSystem.current.SetSelectedGameObject(_playButton);
    }

    public void EnterSecondaryMenu(GameObject cancelButton)
    {
        EventSystem.current.SetSelectedGameObject(cancelButton);

        if (LocalizationSettings.SelectedLocale.Identifier.ToString() == "Spanish(es)")
        {
            _spanishToggle.isOn = true;
            Debug.Log("togle español");
        }
        else
        {
            _englishToggle.isOn = true;
            Debug.Log("togle ingles");
        }
    }

    public void ExitSecondaryMenu()
    {
        EventSystem.current.SetSelectedGameObject(_playButton);
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
