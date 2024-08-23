using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikeAnimation : StateMachineBehaviour
{
    // Este método se llama cuando la animación sale del estado
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Código que se ejecuta cuando la animación ha terminado
        animator.gameObject.SetActive(false);
        // Aquí puedes poner el código que desees ejecutar
    }
}
