using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleFall : MonoBehaviour
{
    public GameObject appleWhole; // Referencia al GameObject de la manzana completa
    public GameObject[] applePieces; // Referencias a los GameObjects de los pedazos de manzana
    public float fallSpeed = 10f; // Velocidad de ca�da

    public float minX = -2f; // L�mite izquierdo del �rbol
    public float maxX = 2f;  // L�mite derecho del �rbol
    public float startY = 3.5f ; // Altura inicial de la manzana

    private Animator animator;
    public float rotateSpeed = 100f;

    void Start()
    {
        // Configurar la posici�n inicial de la manzana
        float randomX = Random.Range(minX, maxX);
        transform.position = new Vector3(randomX, startY, transform.position.z);

        // Obtener el Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject: " + gameObject.name);
            return;
        }

        // Iniciar la secuencia de animaci�n y ca�da
        StartCoroutine(WobbleAndFall());
    }
    private IEnumerator WobbleAndFall()
    {
        // Reproducir la animaci�n de tambaleo
        animator.Play("AppleWobble");

        // Esperar hasta que la animaci�n de tambaleo termine
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        // Iniciar la ca�da de la manzana
        animator.Play("AppleFall");
        //while (transform.position.y > 0)
        //{
        //    transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        //    yield return null;
        //}
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || animator.IsInTransition(0) && transform.position.y > -2.68)
        {
            // Mueve la manzana hacia abajo
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null; // Esperar un frame antes de continuar el bucle
        }

        // Detener el movimiento (manzana ha tocado el suelo o ha terminado la animaci�n)
        fallSpeed = 0f;
        // Desactivar la manzana completa y activar los pedazos
        appleWhole.SetActive(false);
        foreach (GameObject piece in applePieces)
        {
            piece.SetActive(true);
        }
    }
    //void Update()
    //{
    //    // Mueve la manzana hacia abajo
    //    appleWhole.transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

    //    // Verifica si la manzana ha alcanzado el suelo (por ejemplo, y <= 0)
    //    if (appleWhole.transform.position.y <= 0)
    //    {
    //        // Desactivar la manzana completa y activar los pedazos
    //        appleWhole.SetActive(false);
    //        foreach (GameObject piece in applePieces)
    //        {
    //            piece.SetActive(true);
    //        }
    //    }
    //}
}
