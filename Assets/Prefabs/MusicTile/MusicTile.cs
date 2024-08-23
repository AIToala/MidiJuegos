using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MusicTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private MusicTileController gameController;
    private AudioSource audioSource;
    private Canvas canvas;
    private Image imageComponent;

    [Header("Selection Parameters")]
    public bool selected;

    [Header("Hover Parameters")]
    public bool isHovering;

    [Header("Events")]
    [HideInInspector] public UnityEvent<MusicTile> PointerEnterEvent;
    [HideInInspector] public UnityEvent<MusicTile> PointerExitEvent;
    [HideInInspector] public UnityEvent<MusicTile> PointerClickEvent;

    [Header("MusicTile Parameters")]
    [SerializeField] private Sprite image;
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool isCorrect;
    [SerializeField] private int index;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        gameController = FindObjectOfType<MusicTileController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void PointerClick(MusicTile arg0)
    {
        if (isCorrect)
        {
            //
        }
        else
        {
            //
        }
    }

    public bool IsCorrect()
    {
        return isCorrect;
    }

    public bool SetIsCorrect(bool isCorrect)
    {
        this.isCorrect = isCorrect;
        return isCorrect;
    }

    public void SetImage(Sprite image)
    {
        if (image == null) return;
        this.image = image;
        this.GetComponent<Image>().sprite = image;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return index;
    }

    public void PlayClip()
    {
        audioSource.Play();
    }

    public void StopClip()
    {
        audioSource.Stop();
    }

    public void SetClip(AudioClip clip)
    {
        this.clip = clip;
        audioSource.clip = clip;
    }

    void Update()
    {
        if (this == null) return;
    }

    private void OnDestroy()
    {
        DOTween.Kill(this, true);
        PointerClickEvent.RemoveAllListeners();
        PointerEnterEvent.RemoveAllListeners();
        PointerExitEvent.RemoveAllListeners();
        if (this != null)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClickEvent.Invoke(this);
    }
}