using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class CardVisual : MonoBehaviour
{
    private bool initialize = false;

    [Header("Card")]
    public Card parentCard;
    private Transform cardTransform;
    private Vector3 rotationDelta;
    private int savedIndex;
    Vector3 movementDelta;
    private Canvas canvas;

    [Header("Audios")]
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip[] animalsClips;

    [Header("References")]
    public Transform visualShadow;
    private Vector2 shadowDistance;
    private Canvas shadowCanvas;
    [SerializeField] private Transform shakeParent;
    [SerializeField] private Transform tiltParent;
    [SerializeField] private Image cardImage;
    [SerializeField] private Image answerImage;
    [SerializeField] private AudioSource audioSource;

    [Header("Follow Parameters")]
    [SerializeField] private float followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 20;
    [SerializeField] private float rotationSpeed = 20;
    [SerializeField] private float autoTiltAmount = 30;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] private bool scaleAnimations = true;
    [SerializeField] private float scaleOnHover = 1.5f;
    //[SerializeField] private float scaleOnSelect = 1.25f;
    [SerializeField] private float scaleTransition = .15f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    [Header("Select Parameters")]
    //[SerializeField] private float selectPunchAmount = 1.75f;

    [Header("Hover Parameters")]
    [SerializeField] private float hoverPunchAngle = 5;
    [SerializeField] private float hoverTransition = .15f;

    [Header("Curve")]
    [SerializeField] private CurveParameters curve;

    private float curveYOffset;
    private float curveRotationOffset;

    private void Start()
    {
        shadowDistance = visualShadow.localPosition;
    }

    public void Initialize(Card target, Sprite image, int index = 0)
    {
        //Declarations
        parentCard = target;
        cardTransform = target.transform;
        canvas = GetComponent<Canvas>();
        shadowCanvas = visualShadow.GetComponent<Canvas>();
        answerImage = transform.GetChild(1).GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.5f;
        cardImage = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        cardImage.GetComponent<Image>().sprite = image;
        if (target.CheckIfIntruder())
        {
            answerImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("correct");
        }
        else
        {
            answerImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("wrong");
        }

        //Event Listening
        parentCard.PointerEnterEvent.AddListener(PointerEnter);
        parentCard.PointerExitEvent.AddListener(PointerExit);
        parentCard.PointerClickEvent.AddListener(PointerClick);
        //Initialization
        initialize = true;
    }

    public void UpdateIndex(int length)
    {
        transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
    }

    void Update()
    {
        if (!initialize || parentCard == null) return;

        HandPositioning();
        SmoothFollow();
        FollowRotation();
        CardTilt();
    }

    private void HandPositioning()
    {
        curveYOffset = curve.positioning.Evaluate(parentCard.NormalizedPosition()) * curve.positioningInfluence * parentCard.SiblingAmount();
        curveYOffset = parentCard.SiblingAmount() < 5 ? 0 : curveYOffset;
        curveRotationOffset = curve.rotation.Evaluate(parentCard.NormalizedPosition());
    }

    private void SmoothFollow()
    {
        Vector3 verticalOffset = Vector3.up * curveYOffset;
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, followSpeed * Time.deltaTime);
    }

    private void FollowRotation()
    {
        Vector3 movement = transform.position - cardTransform.position;
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = movement * rotationAmount;
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
    }

    private void CardTilt()
    {
        savedIndex = parentCard.ParentIndex();
        float sine = Mathf.Sin(Time.time + savedIndex);
        float cosine = Mathf.Cos(Time.time + savedIndex);
        ;
        float tiltX = 0;
        float tiltY = 0;
        float tiltZ = curveRotationOffset * (curve.rotationInfluence * parentCard.SiblingAmount());

        float lerpX = Mathf.LerpAngle(tiltParent.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(tiltParent.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

        tiltParent.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    private void PointerClick(Card card)
    {
        answerImage.GetComponent<Image>().enabled = true;
        if (scaleAnimations && answerImage.GetComponent<Image>().sprite.name == "correct")
        {
            audioSource.clip = correctClip;
            audioSource.Play();
            StartCoroutine(PlaySound());
            transform.DOScale(Vector3.one * scaleOnHover, scaleTransition).SetEase(scaleEase).SetId(1);
            StartCoroutine(FinishAnimations());
        }
        else
        {
            audioSource.clip = wrongClip;
            audioSource.Play();
            transform.DOPunchRotation(Vector3.forward * 10, 1, 20, 1).SetId(3);
        }
    }

    private IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        if (cardImage.GetComponent<Image>().sprite.name.Contains("Pajaro"))
        {
            if (cardImage.GetComponent<Image>().sprite.name == "Pajaro 1")
            {
                audioSource.volume = 0.2f;
                audioSource.clip = animalsClips[3];
            }
            else
            {
                audioSource.clip = animalsClips[0];
            }
        }
        else if (cardImage.GetComponent<Image>().sprite.name.Contains("Gato"))
        {
            audioSource.clip = animalsClips[1];
        }
        else if (cardImage.GetComponent<Image>().sprite.name.Contains("Perro"))
        {
            audioSource.clip = animalsClips[2];
        }
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
    }

    private IEnumerator FinishAnimations()
    {
        yield return new WaitForSeconds(5f);
        parentCard.getController().ClearCards();
        parentCard.getController().GameController.FinishRound();
    }

    private void PointerEnter(Card card)
    {
        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase).SetId(4);

        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
    }

    private void PointerExit(Card card)
    {
        transform.DOScale(1, scaleTransition).SetEase(scaleEase).SetId(5);
    }
}
