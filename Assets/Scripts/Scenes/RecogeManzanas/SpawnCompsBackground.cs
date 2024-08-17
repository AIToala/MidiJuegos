using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCompsBackground : MonoBehaviour
{
    // Referencia al Prefab o GameObject a clonar
    public GameObject objectToSpawn;
    // Intervalo de tiempo entre las instancias
    public float spawnInterval = 17f;
    // Rango en el eje X donde se generar�n los objetos
    public float minX = -7f;
    public float maxX = 5f;
    // Posici�n en Y (puedes cambiarla si quieres)
    public float yPosition = 0.97f;
    // Posici�n en Z (si trabajas en 2D, generalmente se mantiene en 0)
    public float zPosition = 0f;

    void Start()
    {
        // Inicia la invocaci�n repetida
        InvokeRepeating("SpawnObject", 16f, spawnInterval);
    }

    void SpawnObject()
    {
        // Generar una posici�n aleatoria en el eje X dentro del rango especificado
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, yPosition, zPosition);

        float randomRotationZ = Random.Range(-180f, 180f);
        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, randomRotationZ);

        // Instancia el objeto en la posici�n generada
        Instantiate(objectToSpawn, spawnPosition, spawnRotation);
    }
}
