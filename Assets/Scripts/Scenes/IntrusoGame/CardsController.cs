using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CardsController : MonoBehaviour
{
    [SerializeField] public IntrusoGameController GameController;
    [SerializeField] private Card selectedCard;
    [SerializeReference] private Card hoveredCard;

    [SerializeField] private GameObject slotPrefab;
    private RectTransform rect;

    [Header("Spawn Settings")]
    [SerializeField] private int cardsToSpawn = 4;
    [SerializeField] private int intruderPosition;
    public List<Card> cards;
    public List<Sprite> images;

    bool isCrossing = false;
    //[SerializeField] private bool tweenCardReturn = true;

    void Start()
    {
        GameController = FindObjectOfType<IntrusoGameController>();
        images = new List<Sprite>(4);
        for (int i = 0; i < cardsToSpawn; i++)
        {
            Instantiate(slotPrefab, transform);
        }

        rect = GetComponent<RectTransform>();
        cards = GetComponentsInChildren<Card>().ToList();

        loadIntruder();
        int cardCount = 0;
        foreach (Card card in cards)
        {
            if (cardCount != intruderPosition)
            {
                card.SetIsIntruder(false);
                card.SetImage(images[cardCount]);
            }
            else
            {
                card.SetIsIntruder(true);
                card.SetImage(images[cardCount]);
            }
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.PointerClickEvent.AddListener(CardPointerClick);
            card.name = cardCount.ToString();
            cardCount++;
        }

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].cardVisual != null)
                    cards[i].cardVisual.UpdateIndex(transform.childCount);
            }
        }
        GameController.StartTimer();
    }

    void CardPointerEnter(Card card)
    {
        hoveredCard = card;
    }

    void CardPointerExit(Card card)
    {
        hoveredCard = null;
    }

    void CardPointerClick(Card card)
    {
        selectedCard = card;
    }

    void Update()
    {
        if (selectedCard == null)
            return;

        if (isCrossing)
            return;
    }

    void loadIntruder()
    {
        Sprite[] resources = Resources.LoadAll<Sprite>("IntrusoImages/Intruders");
        if (resources == null || resources.Length == 0)
        {
            Debug.LogError("No images found in folder: " + "IntrusoImages/Intruders");
            return;
        }
        List<String> chooseRandomly = new List<String>() {
            "Gato",
            "Perro",
            "Oso",
            "Monito"
        };
        intruderPosition = UnityEngine.Random.Range(0, 4);
        String intruderName = chooseRandomly[UnityEngine.Random.Range(0, chooseRandomly.Count)];
        chooseRandomly.Remove(intruderName);
        String animalOfHomeName = chooseRandomly[UnityEngine.Random.Range(0, chooseRandomly.Count)];
        Sprite intruderSprite = null;
        resources = resources.OrderBy(x => Guid.NewGuid()).ToArray();
        List<Sprite> resourcesList = new List<Sprite>(resources);

        foreach (Sprite resource in resourcesList)
        {
            if (resource.name.Contains(intruderName))
            {
                intruderSprite = resource;
                Debug.Log("Intruder sprite: " + intruderSprite);
                break;
            }
        }

        for (int i = 0; i < resourcesList.Count; i++)
        {
            if (images.Count == 4)
            {
                break;
            }
            if (resources[i].name.Contains(animalOfHomeName))
            {
                images.Add(resources[i]);
            }
        }

        if (intruderSprite == null)
        {
            Debug.LogError("No sprite found for intruder: " + intruderName);
            return;
        }
        else
        {
            images[intruderPosition] = intruderSprite;
        }
    }
}
