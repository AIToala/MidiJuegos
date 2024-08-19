using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] public GameObject LoadingScreen;
    [SerializeField] private MainController mainController;
    public Image LoadingBarFill;

    public void LoadScene()
    {
        string game = mainController.GetCurrentGame();
        StartCoroutine(LoadSceneAsync(game));
    }

    IEnumerator LoadSceneAsync(string game)
    {
        LoadingScreen.SetActive(true);
        mainController.StopMusic();
        AsyncOperation operation = SceneManager.LoadSceneAsync(game);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarFill.fillAmount = progress;
            yield return new WaitForEndOfFrame();
        }
    }

}
