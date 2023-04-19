using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public static EndGame Instance;

    [SerializeField] private float _creditsDuration = 10f;
    [SerializeField] private Transform _endTarget, _creditsTarget;
    [SerializeField] private GameObject _creator;
    [SerializeField] private Image _fadeOut;

    private int _spiritCount = 0;
    private bool _moveCreator;
    private float _t = 0f;

    private void Awake()
    {
        Instance = this;

        foreach(var os in FindObjectsOfType<OtherSpirit>()) _spiritCount++;
    }

    private void Start()
    {
        Music.Instance.StopMusic();
    }
    private void Update()
    {
        if (!_moveCreator) return;

        _creator.transform.position = Vector2.Lerp(_endTarget.position, _creditsTarget.position, _t);
        _t += Time.deltaTime / _creditsDuration;

        if(Vector2.Distance(_creator.transform.position, _creditsTarget.position) < 2f)
        {
            StartCoroutine(ReturnToMainMenu());
        }
    }

    private IEnumerator ReturnToMainMenu()
    {
        _fadeOut.gameObject.SetActive(true);
        float t = 0f;
        while(t <= 1)
        {
            _fadeOut.color = new Color (0f, 0f, 0f, Mathf.Lerp(0f, 1f, t));

            t += Time.deltaTime / 3.5f;
            yield return null;
        }

        SceneManager.LoadScene("Desert1");
    }
    

    public void SpiritMerge()
    {
        _spiritCount--;

        if (_spiritCount > 0) return;

        StartCoroutine(EndCinematic());
    }

    private IEnumerator EndCinematic()
    {
        yield return new WaitForSeconds(3f);

        var spiritMovement = FindObjectOfType<SpiritMovement>();
        var spiritDim = FindObjectOfType<SpiritDim>();
        var spiritUnion = FindObjectOfType<SpiritUnion>();
        
        spiritUnion.ExitGolem();
        spiritUnion.CanSwap = false;
        spiritUnion.CanInteract = false;
        spiritMovement.CanMove = false;
        spiritDim.IsFading = false;
        spiritMovement.GetComponent<Collider2D>().enabled = false;
        

        var startPos = spiritMovement.transform.position;

        float t = 0f;
        while (Vector2.Distance(spiritMovement.transform.position, _endTarget.position) > 0.05f)
        {
            spiritMovement.transform.position = Vector2.Lerp(startPos, _endTarget.position, t);
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2f);


        var light = spiritMovement.GetComponentInChildren<Light2D>();

        t = 0f;
        var initialIntensity = light.intensity;
        var initialRadius = light.pointLightOuterRadius;
        while(t <= 1f)
        {
            light.intensity = Mathf.Lerp(initialIntensity, 13f, t);
            light.pointLightOuterRadius = Mathf.Lerp(initialRadius, 4f, t);

            t += Time.deltaTime / 3f;
            yield return null;
        }

        spiritMovement.GetComponentInChildren<SpriteRenderer>().enabled = false;
        spiritMovement.transform.Find("Fire trail").gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        Music.Instance.PlayMenuMusic(7f);

        _creator.SetActive(true);
        var creatorSR = _creator.GetComponent<SpriteRenderer>();
        var color = new Color(1f, 1f, 1f, 0f);
        creatorSR.color = color;

        t = 0f;
        while (t <= 1f)
        {
            color.a = Mathf.Lerp(0f, 1f, t);
            creatorSR.color = color;

            light.intensity = Mathf.Lerp(13f, 0f, t);
            //light.pointLightOuterRadius = Mathf.Lerp(4f, 0f, t);

            t += Time.deltaTime / 4f;
            yield return null;
        }

        spiritMovement.transform.position = new Vector3(-4f, transform.position.y + 3f, 0f);
        spiritMovement.transform.SetParent(_creator.transform);

        FindObjectOfType<PixelPerfectCamera>().enabled = false;

        FindObjectOfType<CameraController>().FollowPlayerX();
        FindObjectOfType<CameraController>().FollowPlayerY();

        yield return new WaitForSeconds(2f);

        _moveCreator = true;

    }
}
