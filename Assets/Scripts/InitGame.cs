using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene("MainMenuScene");
    }
    // Update is called once per frame
    void QuitGame()
    {
        Application.Quit();
    }
}
