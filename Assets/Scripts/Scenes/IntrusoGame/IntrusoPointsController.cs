using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntrusoPointsController : MonoBehaviour
{
    private IntrusoGameController gameController;
    private MainController mainController;
    public AudioSource audioSource;
    public AudioClip soundPoints;
    public GameObject pointsText;
    public GameObject pointsChange;
    public Animator animatorTextChange;

    private int points;
    void Start()
    {
        gameController = FindObjectOfType<IntrusoGameController>();
        mainController = FindObjectOfType<MainController>();
        animatorTextChange = pointsChange.GetComponent<Animator>();
        if (gameController == null)
        {
            Debug.LogError("IntrusoGameController no encontrado en IntrusoPointsController.");
            return;
        }
        points = gameController.GetPoints();
        pointsText.GetComponent<TMPro.TextMeshProUGUI>().text = mainController.GetScore().ToString();
        mainController.SetScore(points);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en IntrusoPointsController.");
            return;
        }
        audioSource.clip = soundPoints;
        pointsChange.GetComponent<TMPro.TextMeshProUGUI>().text = "+" + points.ToString();
        StartCoroutine(ShowAndAnimateTextChange());
    }

    private IEnumerator ShowAndAnimateTextChange()
    {
        if (animatorTextChange == null)
        {
            Debug.LogError("Animator no encontrado en IntrusoPointsController.");
            yield return null;
        }
        animatorTextChange.Play("FadeInText");
        yield return new WaitForSeconds(animatorTextChange.GetCurrentAnimatorStateInfo(0).length);
        yield return null;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        points = mainController.GetScore();
        pointsText.GetComponent<TMPro.TextMeshProUGUI>().text = points.ToString();
        yield return new WaitForSeconds(5f);
        Destroy(gameController.gameObject);
        yield return null;
        //SEND REPORT OF GAME FINISHED TO MAIN CONTROLLER
        mainController.LoadNextGame();
    }
}
