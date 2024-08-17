using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameController : MonoBehaviour
{
    public string[] possibleCorrectTexts = { "FA", "LA", "RA", "CA" }; // Array de posibles textos correctos
    public string currentCorrectText; // Texto correcto actual seleccionado
    Dictionary<string, string[]> incorrectTexts = new Dictionary<string, string[]>()
    {
    {"FA", new string[] {"LA", "RA"} },
    {"LA", new string[] {"FA", "NA"} },
    {"RA", new string[] { "FA", "NA"} },
    {"CA", new string[] { "FA", "NA"} },
    };// Array de textos incorrectos
    public GameObject[] applePieces; // Referencias a los pedazos de manzana
    public GameObject appleWhole;

    public float fallSpeed = 10f;
    public float minX = -2f; // Límite izquierdo del árbol
    public float maxX = 2f;  // Límite derecho del árbol
    public float startY = 3.5f; // Altura inicial de la manzana

    private Animator animator;
    public float rotateSpeed = 100f;

    public int score = 100; // Puntaje inicial
    public int pointsPerCorrectAnswer = 10; // Puntos por acierto
    public int pointsPerIncorrectAnswer = 10; // Puntos que se restan por error
    public int minScore = 20; // Puntaje mínimo

    private int currentRound = 0;
    private int totalRounds = 2;
    private bool roundFinished = false;
    void Start()
    {
        animator = appleWhole.GetComponent<Animator>();
        StartRound();
    }

    void StartRound()
    {
        roundFinished = false;
        // Seleccionar un texto correcto aleatorio
        currentCorrectText = possibleCorrectTexts[Random.Range(0, possibleCorrectTexts.Length)];

        // Selecciona un texto correcto aleatorio del array
        currentCorrectText = possibleCorrectTexts[Random.Range(0, possibleCorrectTexts.Length)];
        List<string> textPool = new List<string>(incorrectTexts[currentCorrectText]);
        textPool.Add(currentCorrectText);

        // Mezclar la lista para que los textos se asignen aleatoriamente
        Shuffle(textPool);

        // Asignar los textos mezclados a los pedazos de manzana
        for (int i = 0; i < applePieces.Length; i++)
        {
            TMP_Text textMeshPro = applePieces[i].GetComponentInChildren<TMP_Text>();

            if (textMeshPro != null && i < textPool.Count)
            {
                Debug.Log(textMeshPro.text);
                textMeshPro.text = textPool[i];
            }
        }
        // Aquí podrías hacer otras configuraciones iniciales para la ronda
        Debug.Log("Texto correcto seleccionado: " + currentCorrectText);
        Debug.Log("Puntaje inicial: " + score);

        // Configurar y lanzar la manzana para la ronda
        FallAndBreak();
    }

    void FallAndBreak()
    {
        // Configurar la posición inicial de la manzana
        float randomX = Random.Range(minX, maxX);
        appleWhole.transform.position = new Vector3(randomX, startY, transform.position.z);
        appleWhole.SetActive(true);
        // Iniciar la secuencia de animación y caída
        StartCoroutine(WobbleAndFall());
    }

    private IEnumerator WobbleAndFall()
    {
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject: " + gameObject.name);
            yield return null;
        }
        // Reproducir la animación de tambaleo
        animator.Play("AppleWobble");

        // Esperar hasta que la animación de tambaleo termine
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        // Iniciar la caída de la manzana
        animator.Play("AppleFall");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || animator.IsInTransition(0) && appleWhole.transform.position.y > 2.68)
        {
            // Mueve la manzana hacia abajo
            appleWhole.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null; // Esperar un frame antes de continuar el bucle
        }

        // Detener el movimiento (manzana ha tocado el suelo o ha terminado la animación)
        fallSpeed = 0f;
        // Desactivar la manzana completa y activar los pedazos
        appleWhole.SetActive(false);
        foreach (GameObject piece in applePieces)
        {
            SpriteRenderer spriteRenderer = piece.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            MeshRenderer meshRenderer = piece.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }
            piece.SetActive(true);
        }

        yield return new WaitUntil(() => isRoundFinished());
       
        // Iniciar la siguiente ronda o finalizar
        currentRound++;

        if (currentRound < totalRounds)
        {
            StartRound();
        }
        else
        {
            EndGame();
        }
    }

    bool isRoundFinished()
    {
        if (roundFinished)
        {
            return true;
        }
        return false;
    }

    public void finishRound()
    {
        foreach (GameObject piece in applePieces)
        {
            piece.SetActive(false);
        }
        roundFinished = true;

    }

    void EndGame()
    {
        Debug.Log("Juego terminado. Puntaje final: " + score);
        // Aquí podrías mostrar una pantalla de resultados o reiniciar el juego.
    }


    void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    public void AdjustScore(bool isCorrect)
    {
        if (isCorrect)
        {
            // Acierto: sumar puntos
            score += pointsPerCorrectAnswer;
        }
        else
        {
            // Error: restar puntos, pero no bajar de minScore
            if (score > minScore)
            {
                score -= pointsPerIncorrectAnswer;
                if (score < minScore)
                {
                    score = minScore;
                }
            }
        }

        // Mostrar el puntaje actualizado
        Debug.Log("Puntaje actual: " + score);
    }
}
