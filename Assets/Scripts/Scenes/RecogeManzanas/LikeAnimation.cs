using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikeAnimation : StateMachineBehaviour
{
    // Este m�todo se llama cuando la animaci�n sale del estado
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // C�digo que se ejecuta cuando la animaci�n ha terminado
        animator.gameObject.SetActive(false);
        // Aqu� puedes poner el c�digo que desees ejecutar
    }
}
