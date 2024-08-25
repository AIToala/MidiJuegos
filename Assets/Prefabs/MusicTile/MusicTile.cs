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
    public Image imageComponent;
    public Image iconComponent;

    [Header("Selection Parameters")]
    public bool selected;

    [Header("Hover Parameters")]
    public bool isHovering;

    [Header("Events")]
    [HideInInspector] public UnityEvent<MusicTile> PointerEnterEvent;
    [HideInInspector] public UnityEvent<MusicTile> PointerExitEvent;
    [HideInInspector] public UnityEvent<MusicTile> PointerClickEvent;

    [Header("MusicTile Parameters")]
    [SerializeField] private AudioClip clip;
    [SerializeField] private string color;
    [SerializeField] private bool isCorrect;
    [SerializeField] private int index;

    public Color32 baseColor = new Color32(254, 254, 254, 255);

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        gameController = FindObjectOfType<MusicTileController>();
        audioSource = GetComponent<AudioSource>();
    }

    public Image GetImageComponent()
    {
        return imageComponent;
    }

    public Image GetIconComponent()
    {
        return iconComponent;
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

    public void SetBase()
    {
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
            imageComponent.sprite = Resources.Load<Sprite>("SecuenciaImages/MusicTile/BaseColor");
        }
        if (iconComponent == null)
        {
            iconComponent = GetComponentInChildren<Image>();
            iconComponent.sprite = Resources.Load<Sprite>("SecuenciaImages/MusicTile/MusicalNote");
        }
        imageComponent.color = new Color32(254, 254, 254, 255);
        if (this.color == "BLUE") iconComponent.color = new Color32(10, 126, 242, 255);
        if (this.color == "GREEN") iconComponent.color = new Color32(82, 197, 75, 255);
        if (this.color == "RED") iconComponent.color = new Color32(230, 50, 44, 255);
        if (this.color == "YELLOW") iconComponent.color = new Color32(251, 198, 46, 255);
    }

    public void SetGlow()
    {
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
            imageComponent.sprite = Resources.Load<Sprite>("SecuenciaImages/MusicTile/BaseColor");
        }
        if (iconComponent == null)
        {
            iconComponent = GetComponentInChildren<Image>();
            iconComponent.sprite = Resources.Load<Sprite>("SecuenciaImages/MusicTile/MusicalNote");
        }
        iconComponent.color = new Color32(254, 254, 254, 255);
        if (this.color == "BLUE") imageComponent.color = new Color32(10, 126, 242, 255);
        if (this.color == "GREEN") imageComponent.color = new Color32(82, 197, 75, 255);
        if (this.color == "RED") imageComponent.color = new Color32(230, 50, 44, 255);
        if (this.color == "YELLOW") imageComponent.color = new Color32(251, 198, 46, 255);
    }

    public Color32 GetColorGlow()
    {
        if (this.color == "BLUE") return new Color32(10, 126, 242, 255);
        if (this.color == "GREEN") return new Color32(82, 197, 75, 255);
        if (this.color == "RED") return new Color32(230, 50, 44, 255);
        if (this.color == "YELLOW") return new Color32(251, 198, 46, 255);
        return new Color32(254, 254, 254, 255);
    }

    public void SetColor(string color)
    {
        this.color = color;
    }

    public string GetColor()
    {
        return color;
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
        audioSource.PlayOneShot(clip);
    }

    public void StopClip()
    {
        audioSource.Stop();
    }

    public void SetClip(AudioClip clip)
    {
        if (clip == null) return;
        this.clip = clip;
        this.GetComponent<AudioSource>().clip = clip;
        this.GetComponent<AudioSource>().volume = 0.35f;
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
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