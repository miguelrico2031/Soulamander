using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToCity : MonoBehaviour
{
    public static ChangeToCity Instance;

    [SerializeField] private GameObject _salamander, _vacuum, _brokenOrb, _orb;

    private AudioSource _audioSource;
    private SpiritUnion _spiritUnion;

    private void Awake()
    {
        Instance = this;

        _audioSource = GetComponent<AudioSource>();

        Music.Instance.PlayMossMusic();
    }

    public void StartCinematic(SpiritUnion spiritUnion)
    {
        _spiritUnion = spiritUnion;
        StartCoroutine(Cinematic());
    }

    private IEnumerator Cinematic()
    {
        _spiritUnion.CanSwap = false;
        _spiritUnion.CanInteract = false;
        
        var spiritDim = FindAnyObjectByType<SpiritDim>();
        spiritDim.IsFading = false;
        var spiritMove = FindAnyObjectByType<SpiritMovement>();
        spiritMove.CanMove = false;

        _orb.SetActive(true);
        Destroy(_vacuum);

        yield return new WaitForSeconds(3f);

        _audioSource.Play(); //sonido de pasos + ostia al cristal

        yield return new WaitForSeconds(_audioSource.clip.length + 1f);

        _salamander.SetActive(true);
        _brokenOrb.SetActive(true);
        _orb.SetActive(false);

        PauseGame.Instance.FadeIn();

        spiritMove.CanMove = true;
        _spiritUnion.CanSwap = true;
        _spiritUnion.CanInteract = true;
        _spiritUnion.EnableTriggers();

        yield return new WaitForSeconds(5f);

        spiritDim.IsFading = true;
    }
}
