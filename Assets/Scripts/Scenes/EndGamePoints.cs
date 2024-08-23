using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePoints : MonoBehaviour
{
    private MainController mainController;
    public AudioSource audioSource;
    public AudioClip soundPoints;
    public GameObject pointsText;
    public GameObject pointsChange;
    public GameObject coinsText;
    public GameObject coinsChange;
    public Animator animatorTextChange;
    public Animator animatorCoinsChange;

    private int points;
    private int coins;
    void Start()
    {
        mainController = MainController.main;
        animatorTextChange = pointsChange.GetComponent<Animator>();
        animatorCoinsChange = coinsChange.GetComponent<Animator>();
        points = mainController.GetScore();
        coins = mainController.GetCoins();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en IntrusoPointsController.");
            return;
        }
        audioSource.clip = soundPoints;
        pointsChange.GetComponent<TMPro.TextMeshProUGUI>().text = "+" + points.ToString();
        CalculateCoins();
        StartCoroutine(ShowAndAnimateTextChange());
        StartCoroutine(ShowAndAnimateCoinsChange());
    }

    private IEnumerator ShowAndAnimateTextChange()
    {
        if (animatorTextChange == null)
        {
            Debug.LogError("Animator no encontrado en EndGamePoints.");
            yield return null;
        }
        animatorTextChange.Play("FadeInText");
        yield return new WaitForSeconds(animatorTextChange.GetCurrentAnimatorStateInfo(0).length);
        yield return null;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        pointsText.GetComponent<TMPro.TextMeshProUGUI>().text = points.ToString();
        yield return new WaitForSeconds(1.5f);
        //SEND REPORT OF GAME FINISHED TO MAIN CONTROLLER
    }

    private IEnumerator ShowAndAnimateCoinsChange()
    {
        if (animatorCoinsChange == null)
        {
            Debug.LogError("Animator no encontrado en EndGamePoints.");
            yield return null;
        }
        animatorCoinsChange.Play("FadeInCoins");
        yield return new WaitForSeconds(animatorCoinsChange.GetCurrentAnimatorStateInfo(0).length);
        yield return null;
        mainController.SetCoins(coins);
        coinsText.GetComponent<TMPro.TextMeshProUGUI>().text = coins.ToString();
        yield return new WaitForSeconds(8f);
        //SEND REPORT OF GAME FINISHED TO MAIN CONTROLLER
        mainController.FinishAndReturnToMenu();
    }

    private void CalculateCoins()
    {
        coins = points / 10;

        coinsChange.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "+" + coins.ToString();
    }
}
