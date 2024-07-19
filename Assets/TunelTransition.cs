using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TunelTransition : Colliderable
{
    public string sceneName;
    private SceneTranslate SceneTranslate;
    protected override void Start()
    {
        base.Start();
        if (SceneTranslate == null)
            SceneTranslate = GetComponentInChildren<SceneTranslate>();
        GetComponent<BoxCollider2D>().enabled = true;
    }



    // Update is called once per frame
    public void ChangeSceneTo(string sceneName)
    {
        SceneTranslate.ChangeToScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
