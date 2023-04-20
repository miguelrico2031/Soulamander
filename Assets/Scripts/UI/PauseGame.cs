using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    public static PauseGame Instance;

    public bool Paused { get; private set; }
    public bool IsAtLevel1 = false;

    private bool _isOnClue, _isOnLevelSelect;

    [SerializeField] private GameObject _pausePanel, _bgPanel, _levelSelectPanel, _cluePanel;
    [SerializeField] private TextMeshProUGUI _clueText;
    [SerializeField] private Clues _clues;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _musicSlider, _sfxSlider;

    private EventSystem _eventSystem;
    private Animator _animator;

    private void Awake()
    {
        if (Instance)
        {
            if(Instance != this) Destroy(transform.parent.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
            _eventSystem = transform.parent.GetComponentInChildren<EventSystem>();
            _animator = transform.parent.GetComponent<Animator>();
        }
        
        Resume();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach(var eS in FindObjectsOfType<EventSystem>())
        {
            if (eS != _eventSystem) Destroy(eS.gameObject);
        }

        _clueText.text = _clues.GetClue(scene.name);
        Resume();

        _eventSystem.firstSelectedGameObject = _pausePanel.transform.Find("Resume").gameObject;

        float volume = PlayerPrefs.GetFloat("MusicVolume", -20f);
        _musicSlider.value = volume;
        _audioMixer.SetFloat("Music", volume <= -50f ? -80f : volume);

        volume = PlayerPrefs.GetFloat("SFXVolume", -20f);
        _sfxSlider.value = volume;
        _audioMixer.SetFloat("SFX", volume <= -50f ? -80f : volume);


        if (scene.name == "Desert1") return;

        FadeIn();
    }



    private void Update()
    {
        if (Input.GetButtonDown("Pause")) Pause(!Paused);

        if(Input.GetButtonDown("Cancel"))
        {
            if(_isOnClue)
            {
                _cluePanel.SetActive(false);
                _pausePanel.SetActive(true);
                _eventSystem.SetSelectedGameObject(_pausePanel.transform.Find("Resume").gameObject);
                _isOnClue = false;
            }
            else if(_isOnLevelSelect)
            {
                _levelSelectPanel.SetActive(false);
                _pausePanel.SetActive(true);
                _eventSystem.SetSelectedGameObject(_pausePanel.transform.Find("Resume").gameObject);
                _isOnLevelSelect = false;
            }

            else Resume();
        }

        if (_eventSystem.currentSelectedGameObject) return;

        if (_isOnClue) _eventSystem.SetSelectedGameObject(_cluePanel.transform.Find("Cancel").gameObject);
        else if (_isOnLevelSelect) _eventSystem.SetSelectedGameObject(_levelSelectPanel.transform.Find("Cancel").gameObject);
        else _eventSystem.SetSelectedGameObject(_pausePanel.transform.Find("Resume").gameObject);
    }

    public void Pause(bool pause)
    {
        if (pause) Pause();

        else Resume();
    }

    private void Pause()
    {
        Paused = true;
        _pausePanel.SetActive(true);
        _bgPanel.SetActive(true);
        _levelSelectPanel.SetActive(false);
        _cluePanel.SetActive(false);
        Time.timeScale = 0f;
        Cursor.visible = true;

        _eventSystem.SetSelectedGameObject(_pausePanel.transform.Find("Resume").gameObject);
    }

    private void Resume()
    {
        StartCoroutine(ResumeAtNextFrame());

    }

    private IEnumerator ResumeAtNextFrame()
    {
        yield return null;
        Paused = false;
        _pausePanel.SetActive(false);
        _bgPanel.SetActive(false);
        _levelSelectPanel.SetActive(false);
        _cluePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        if(!IsAtLevel1) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else SceneManager.LoadScene("Desert1_NC");
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnClue(bool onClue)
    {
        _clueText.text = _clues.GetClue(SceneManager.GetActiveScene().name);
        _isOnClue = onClue;
    }
    public void OnLevelSelect(bool onLevelSelect) => _isOnLevelSelect = onLevelSelect;

    public void ChangeMusicVolume(float volume)
    {
        _audioMixer.SetFloat("Music", volume <= -50f ? -80f : volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void ChangeSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFX", volume <= -50f ? -80f : volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void FadeOut()
    {
        _animator.SetBool("FadeOut", true);
        _animator.SetBool("FadeIn", false);
    }

    public void FadeIn()
    {
        _animator.SetBool("FadeOut", false);
        _animator.SetBool("FadeIn", true);
        Invoke(nameof(FadeIdle), 2f);
    }

    private void FadeIdle()
    {
        _animator.SetBool("FadeOut", false);
        _animator.SetBool("FadeIn", false);
    }
}
