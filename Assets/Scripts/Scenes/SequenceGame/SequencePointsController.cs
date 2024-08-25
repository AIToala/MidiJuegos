using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;

public class SequencePointsController : MonoBehaviour
{
    private SequenceGameController gameController;
    private MainController mainController;
    public AudioSource audioSource;
    public AudioClip soundPoints;
    public GameObject pointsText;
    public GameObject pointsChange;
    public Animator animatorTextChange;
    public GameObject notaMusical;

    private int points;
    void Start()
    {
        gameController = FindObjectOfType<SequenceGameController>();
        mainController = FindObjectOfType<MainController>();
        animatorTextChange = pointsChange.GetComponent<Animator>();
        notaMusical.transform.DOShakeRotation(1f, new Vector3(0f, 0f, 90f), 10, 90f).SetId(10);
        if (gameController == null)
        {
            Debug.LogError("SequenceGameController no encontrado en SequencePointsController.");
            return;
        }
        points = gameController.GetPoints();
        pointsText.GetComponent<TMPro.TextMeshProUGUI>().text = mainController.GetScore().ToString();
        mainController.SetScore(points);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en SequenceGameController.");
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
            Debug.LogError("Animator no encontrado en SequenceGameController.");
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
        DOTween.Kill(notaMusical.transform);
        mainController.LoadNextGame();
    }
}
