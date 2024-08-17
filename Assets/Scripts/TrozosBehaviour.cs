using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrozosBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float moveSpeed = 2f; // Velocidad de movimiento hacia la izquierda
    public float rotateSpeed = 100f; // Velocidad de rotación en grados por segundo
    public float duration = 5f; // Duración en segundos durante la cual el sprite se moverá y rotará

    private float elapsedTime = 0f; // Tiempo transcurrido

    void Update()
    {
        // Incrementar el tiempo transcurrido
        elapsedTime += Time.deltaTime;

        // Verificar si el tiempo transcurrido es menor que la duración
        if (elapsedTime < duration)
        {
            // Mover el sprite hacia la izquierda
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            // Rotar el sprite sobre su propio eje (en el eje Z)
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }
}
