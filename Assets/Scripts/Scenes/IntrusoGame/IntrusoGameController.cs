using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntrusoGameController : MonoBehaviour
{
    public static IntrusoGameController instance;
    [Header("Game Objects")]
    [SerializeField] public GameObject intrusoTitle;
    [SerializeField] public GameObject roundText;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private CardsController cardsController;
    [SerializeField] private TimerController timer;

    [Header("Game Setup")]
    [SerializeField] private bool roundFinished = false;
    [SerializeField] private bool gameFinished = false;
    [SerializeField] private int pointsPerCorrectAnswer = 20;
    [SerializeField] private int pointsPerIncorrectAnswer = 20;
    [SerializeField] private int minScore = 20;

    [Header("Game Settings")]
    [SerializeField] private int numberOfErrors = 0;
    [SerializeField] private int points = 100;
    [SerializeField] private int currentRound = 0;
    [SerializeField] private int totalRounds = 2;

    [Header("Animations")]
    private Animator animatorRoundText;
    private Animator animatorTitle;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType<IntrusoGameController>().Length > 1)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        intrusoTitle.SetActive(true);
        animatorTitle = intrusoTitle.GetComponent<Animator>();
        animatorRoundText = roundText.GetComponent<Animator>();
        cardsController = FindObjectOfType<CardsController>();
        timer = FindObjectOfType<TimerController>();

        cardsController.GameController = this;
        points = 100;
        numberOfErrors = 0;
        StartTimer();
        StartCoroutine(BeginGame());
    }

    private IEnumerator BeginGame()
    {
        yield return StartCoroutine(ShowAndAnimateTitle());
        yield return StartCoroutine(StartRound());
    }

    private IEnumerator ShowAndAnimateTitle()
    {
        if (animatorTitle == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject: " + gameObject.name);
            yield return null;
        }
        animatorTitle.Play("IntrusoTitleAppear");
        yield return new WaitForSeconds(animatorTitle.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(4f);
        animatorTitle.Play("IntrusoTitleDissappear");
        yield return new WaitForSeconds(animatorTitle.GetCurrentAnimatorStateInfo(0).length);
        yield return null;
        intrusoTitle.SetActive(false);
    }

    private IEnumerator StartRound()
    {
        roundFinished = false;
        roundText.SetActive(true);
        roundText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ronda " + (currentRound + 1);
        yield return StartCoroutine(ShowAndAnimateRoundText());

        cardsController.Initialize();

        yield return StartCoroutine(StartRoundCoroutine());
    }

    private IEnumerator ShowAndAnimateRoundText()
    {
        if (animatorRoundText == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject" + gameObject.name);
            yield return null;
        }
        animatorRoundText.Play("FadeInRonda");
        yield return new WaitForSeconds(animatorRoundText.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(2f);
        animatorRoundText.Play("FadeOutRonda");
        yield return new WaitForSeconds(animatorRoundText.GetCurrentAnimatorStateInfo(0).length);
        yield return null;
        roundText.SetActive(false);
    }

    private IEnumerator StartRoundCoroutine()
    {
        yield return new WaitUntil(() => IsRoundFinished());
        currentRound++;
        if (currentRound < totalRounds)
        {
            StartCoroutine(StartRound());
        }
        else
        {
            FinishGame();
        }
    }

    public bool IsRoundFinished()
    {
        if (roundFinished) return true;
        return false;
    }

    public void FinishRound()
    {
        roundFinished = true;
    }

    public double GetTimeElapsed()
    {
        return timer.GetTimeElapsed();
    }

    public void AddPerfectPoint()
    {
        if (numberOfErrors == 0)
        {
            points += pointsPerCorrectAnswer;
        }
    }

    public void AddPlayerMiss()
    {
        numberOfErrors++;
        if (points < minScore) return;
        if (numberOfErrors > 3) return;
        if (points - pointsPerIncorrectAnswer < minScore) return;
        points -= pointsPerIncorrectAnswer;
    }

    public void StartTimer()
    {
        timer.BeginTimer();
    }

    public void StopTimer()
    {
        timer.EndTimer();
    }

    public int GetNumberOfPlayerMisses()
    {
        return numberOfErrors;
    }

    public int GetPoints()
    {
        return points;
    }

    public void FinishGame()
    {
        StopTimer();
        gameFinished = true;
        SceneManager.LoadScene("IntrusoGame - Points");
    }

    public bool isGameFinished()
    {
        return gameFinished;
    }
}
