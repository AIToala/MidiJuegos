using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTranslate : MonoBehaviour
{
    public Canvas SCUI;
    private Slider SCUISlider;
    private float target = 0;
    private float dtimer = 0;

    AsyncOperation op = null;

    private void Start()
    {
        if (SCUI == null)
            SCUI = GameObject.Find("SCUI").GetComponent<Canvas>();

        SCUI.gameObject.SetActive(false);
        SCUISlider = SCUI.GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        UpdateSlider();
    }

    public void ChangeToScene(string sceneName)
    {
        SCUI.gameObject.SetActive(true);
        SCUISlider.value = 0;
        StartCoroutine(ProcessLoading(sceneName));
    }

    private void UpdateSlider()
    {
        if (op != null)
        {
            if (op.progress >= 0.9f)
            {
                target = 1;
            }
            else
                target = op.progress;

            if (SCUISlider.value > 0.99)
            {
                SCUISlider.value = 1;
                SCUI.gameObject.SetActive(false);
                op.allowSceneActivation = true;
                return;
            }
            else
            {
                SCUISlider.value = Mathf.Lerp(SCUISlider.value, target, dtimer * 0.05f);
                dtimer += Time.deltaTime;
            }
        }
    }

    IEnumerator ProcessLoading(string sceneName)
    {
        op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        yield return op;
    }
}
