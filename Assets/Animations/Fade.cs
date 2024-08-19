using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    void Start()
    {
        Invoke("FadeOut", 2);
    }

    public void FadeOut()
    {
        animator.Play("FadeOut");
    }
}
