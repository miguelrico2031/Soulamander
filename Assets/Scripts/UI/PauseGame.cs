using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

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

        _audioMixer.SetFloat("Music", -20f);
        _audioMixer.SetFloat("SFX", -20f);

        if (scene.name == "Desert1") return;

        _animator.SetBool("FadeOut", false);
        _animator.SetBool("FadeIn", true);
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

        _eventSystem.SetSelectedGameObject(_pausePanel.transform.Find("Resume").gameObject);
    }

    private void Resume()
    {
        Paused = false;
        _pausePanel.SetActive(false);
        _bgPanel.SetActive(false);
        _levelSelectPanel.SetActive(false);
        _cluePanel.SetActive(false);
        Time.timeScale = 1f;
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

    public void OnClue(bool onClue) => _isOnClue = onClue;
    public void OnLevelSelect(bool onLevelSelect) => _isOnLevelSelect = onLevelSelect;

    public void ChangeMusicVolume(float volume)
    {
        _audioMixer.SetFloat("Music", volume <= -50f ? -80f : volume);
    }

    public void ChangeSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFX", volume <= -50f ? -80f : volume);
    }

    public void FadeOut()
    {
        _animator.SetBool("FadeOut", true);
        _animator.SetBool("FadeIn", false);
    }
}
