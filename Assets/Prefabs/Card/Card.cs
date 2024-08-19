
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CardsController cardsController;
    private AudioSource audioSource;
    private Canvas canvas;
    private Image imageComponent;
    [SerializeField] private bool instantiateVisual = true;
    [SerializeField] private bool isIntruder;
    [SerializeField] private Sprite visualImage;
    private VisualCardsHandler visualHandler;
    private Vector3 offset;

    [Header("Movement")]
    //[SerializeField] private float moveSpeedLimit = 50;

    [Header("Selection")]
    public bool selected;
    public float selectionOffset = 50;

    [Header("Visual")]
    [SerializeField] private GameObject cardVisualPrefab;
    [HideInInspector] public CardVisual cardVisual;

    [Header("States")]
    public bool isHovering;

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card> PointerClickEvent;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        cardsController = FindObjectOfType<CardsController>();
        if (!instantiateVisual)
            return;

        visualHandler = FindObjectOfType<VisualCardsHandler>();
        cardVisual = Instantiate(cardVisualPrefab, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<CardVisual>();
        cardVisual.Initialize(this, visualImage);

    }

    void Update()
    {
        if (this == null) return;
        if (cardVisual == null) return;
        ClampPosition();
    }

    void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public CardsController getController()
    {
        return cardsController;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
        isHovering = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
        isHovering = false;

    }

    public bool CheckIfIntruder()
    {
        if (isIntruder)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetIsIntruder(bool intruder)
    {
        isIntruder = intruder;
    }

    public void SetImage(Sprite image)
    {
        visualImage = image;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClickEvent.Invoke(this);
        if (CheckIfIntruder())
        {
            Debug.Log("Correct");
            cardsController.GameController.AddPerfectPoint();
        }
        else
        {
            if (cardsController.GameController.IsRoundFinished())
            {
                return;
            }
            Debug.Log("Wrong");
            cardsController.GameController.AddPlayerMiss();
        }
    }

    public int SiblingAmount()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }

    public float NormalizedPosition()
    {
        return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
    }

    private void OnDestroy()
    {
        DOTween.Kill(1, true);
        DOTween.Kill(2, true);
        DOTween.Kill(3, true);
        DOTween.Kill(4, true);
        DOTween.Kill(5, true);
        DOTween.Kill(this, true);
        if (cardVisual != null)
        {
            Destroy(cardVisual.gameObject);
        }

    }
}
