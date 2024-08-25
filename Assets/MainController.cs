using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    public static MainController main;

    [SerializeField] private string sceneName;
    [Header("Global Game Objects")]
    [SerializeField] private List<string> games;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip soundBG;

    [Header("Global Game Settings")]
    [SerializeField] private int score = 0;
    [SerializeField] private int coins = 0;
    [SerializeField] private bool gameFinished = false;
    //[SerializeField] private bool withTutorials = false;
    [SerializeField] private string currentGame;

    public void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject);
            sceneName = main.sceneName;
            games = main.games;
            audioSource = main.audioSource;
            soundBG = main.soundBG;
            score = main.score;
            coins = main.coins;
            gameFinished = main.gameFinished;
            currentGame = main.currentGame;
        }
        else
        {
            main = this;
            DontDestroyOnLoad(transform.root);
        }
    }

    public int LoadHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    public int LoadPlayerCoins()
    {
        return PlayerPrefs.GetInt("Coins", 0);
    }

    public void StopMusic()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public string GetNextGame()
    {
        string game = games[UnityEngine.Random.Range(0, games.Count)];
        currentGame = game;
        RemoveGame(game);
        return game;
    }

    public void FinishAndReturnToMenu()
    {
        Debug.Log("Returning to main menu");
        SceneManager.LoadScene("MainMenuScene");
        gameFinished = true;
        SavePlayerData(score, coins);
        StartGameAgain();
        Debug.Log("CoinsFinish: " + coins);
    }

    public string RemoveGame(string game)
    {
        games.Remove(game);
        return game;
    }

    public void LoadGames()
    {
        games = new List<string>
        {
            "SecuenciaGame",
        };
        currentGame = GetNextGame();
        Debug.Log("Starting game: " + currentGame);
    }

    public string GetCurrentGame()
    {
        return currentGame;
    }

    void StartGameAgain()
    {
        ResetGame();
        LoadGames();
    }

    IEnumerator InitializeGame()
    {
        yield return null;
        StartGameAgain();
    }

    void ResetGame()
    {
        score = 0;
        coins = main.GetCoins();
        gameFinished = false;
        Debug.Log("Coins: " + coins);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = soundBG;
        audioSource.Play();
    }

    void Start()
    {
        score = LoadHighScore();
        coins = LoadPlayerCoins();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(InitializeGame());
    }

    private void SavePlayerData(int score, int coins)
    {
        if (score > LoadHighScore())
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (gameFinished)
        {
            SavePlayerData(score, coins);
        }
    }

    public int GetScore()
    {
        return score;
    }

    public int SetScore(int newScore)
    {
        score += newScore;
        return score;
    }

    public bool IsGameFinished()
    {
        return gameFinished;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(GetCurrentGame(), LoadSceneMode.Single);
    }

    public void LoadNextGame()
    {
        //Maybe put animations in here like fade out and on start in next scene a fadein
        if (games.Count == 0)
        {
            FinishGame();
            return;
        }
        GetNextGame();
        SceneManager.LoadScene(GetCurrentGame(), LoadSceneMode.Single);
    }

    public void FinishGame()
    {
        SceneManager.LoadScene("EndGame");
    }

    public int GetCoins()
    {
        return coins;
    }

    public int SetCoins(int newCoins)
    {
        coins += newCoins;
        return coins;
    }
}
