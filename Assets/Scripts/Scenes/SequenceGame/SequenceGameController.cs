using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SequenceGameController : MonoBehaviour
{
    public static SequenceGameController instance;
    [Header("Game Objects")]
    [SerializeField] public GameObject sequenceTitle;
    [SerializeField] public GameObject roundText;
    [SerializeField] public GameObject sequenceObject;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private MusicTileController musicTileController;
    [SerializeField] private TimerController timer;
    [SerializeField] private GameObject information;

    [Header("Game Setup")]
    [SerializeField] private bool roundFinished = false;
    [SerializeField] private bool gameFinished = false;
    [SerializeField] private int pointsPerCorrectAnswer = 25;
    [SerializeField] private int pointsPerIncorrectAnswer = 25;
    [SerializeField] private int minScore = 20;
    [SerializeField] private List<int> sequence = new List<int>(4);
    [SerializeField] private List<AudioClip> audioSequence = new List<AudioClip>(4);

    [Header("Game Settings")]
    [SerializeField] private int numberOfErrors = 0;
    [SerializeField] private int points = 0;
    [SerializeField] private int currentRound = 0;
    [SerializeField] private int totalRounds = 2;

    [Header("Animations")]
    private Animator animatorRoundText;
    private Animator animatorTitle;
    private int tilesToSpawn;

    public void Awake()
    {
        // Si ya existe una instancia de GameController, destruye este objeto
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Si no existe una instancia, asigna esta como la instancia �nica
        instance = this;

        // No destruyas este objeto al cargar nuevas escenas
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        // Aseg�rate de que la instancia sea null al destruirse
        if (instance == this)
        {
            instance = null;
        }
    }

    void Start()
    {
        audioSequence = new List<AudioClip>(4);
        information.SetActive(true);
        sequenceTitle.SetActive(true);
        animatorTitle = sequenceTitle.GetComponent<Animator>();
        animatorRoundText = roundText.GetComponent<Animator>();
        musicTileController = FindObjectOfType<MusicTileController>();
        timer = FindObjectOfType<TimerController>();
        audioSource = GetComponent<AudioSource>();

        musicTileController.GameController = this;
        sequenceObject.SetActive(false);
        points = 0;
        numberOfErrors = 0;
        StartTimer();
        StartCoroutine(BeginGame());
    }

    private IEnumerator BeginGame()
    {
        yield return StartCoroutine(ShowAndAnimateTitle());
        yield return StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        roundFinished = false;
        information.SetActive(false);
        GenerateRandomSequence();
        audioSequence = GenerateRandomAudioSequence();
        roundText.SetActive(true);
        roundText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ronda " + (currentRound + 1);
        yield return StartCoroutine(ShowAndAnimateRoundText());
        sequenceObject.SetActive(true);
        musicTileController.Initialize();
        yield return StartCoroutine(StartRoundCoroutine());
    }

    private IEnumerator ShowAndAnimateTitle()
    {
        if (animatorTitle == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject" + gameObject.name);
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        //play-sound-title
        //wait-for-sound
        animatorTitle.Play("FadeInTitle");
        yield return new WaitForSeconds(animatorTitle.GetCurrentAnimatorStateInfo(0).length);
        yield return null;
        sequenceTitle.SetActive(false);
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

    public void GenerateRandomSequence()
    {
        sequence = musicTileController.GenerateRandomSequence();
    }

    public List<AudioClip> GenerateRandomAudioSequence()
    {
        if (audioSequence == null)
        {
            audioSequence = new List<AudioClip>(4);
        }
        List<String> randomInstrument = new List<String>() {
            "Piano",
            "Guitar",
            "Violin"
        };
        String instrument = randomInstrument[UnityEngine.Random.Range(0, randomInstrument.Count)];
        Debug.Log("Instrument: " + instrument);
        AudioClip[] resources = Resources.LoadAll<AudioClip>("SecuenciaImages/Audios/Notes");
        if (resources == null || resources.Length == 0)
        {
            Debug.LogError("No audios found in folder: " + "SecuenciaImages/Audios/Notes");
            return null;
        }
        resources = resources.OrderBy(x => Guid.NewGuid()).ToArray();
        List<AudioClip> resourcesList = new List<AudioClip>(resources);
        int i = 0;
        while (i < resourcesList.Count)
        {
            if (resourcesList[i].name.Contains(instrument))
            {
                audioSequence.Add(resourcesList[i]);
                if (audioSequence.Count == 4)
                {
                    return audioSequence;
                }
            }
            i++;
        }
        return null;
    }

    public List<int> GetSequence()
    {
        return sequence;
    }

    public List<AudioClip> GetAudioSequence()
    {
        return audioSequence;
    }

    public void SetAudioSequence(List<AudioClip> audioSequence)
    {
        this.audioSequence = audioSequence;
    }

    public bool IsRoundFinished()
    {
        if (roundFinished) return true;
        return false;
    }

    public void FinishRound()
    {
        sequenceObject.SetActive(false);
        roundFinished = true;
    }

    public double GetTimeElapsed()
    {
        return timer.GetTimeElapsed();
    }

    public void AddPoint()
    {
        audioSource.clip = Resources.Load<AudioClip>("SecuenciaImages/Audios/correct");
        audioSource.Play();
        StartCoroutine(WaitForSound());
        if (numberOfErrors == 0)
        {
            points += (pointsPerCorrectAnswer * 3) + 50;
            return;
        }
        points += pointsPerCorrectAnswer * 3;
    }

    public void AddPlayerMiss()
    {
        audioSource.clip = Resources.Load<AudioClip>("SecuenciaImages/Audios/wrong");
        audioSource.Play();
        StartCoroutine(WaitForSound());
        numberOfErrors++;
        if (points < minScore) return;
        if (numberOfErrors > 3) return;
        points -= pointsPerIncorrectAnswer;
    }

    private IEnumerator WaitForSound()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
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
        Debug.Log("Game Finished");
        Debug.Log("Points: " + points);
        SceneManager.LoadScene("SecuenciaGame - Points");
    }

    public bool isGameFinished()
    {
        return gameFinished;
    }
}
