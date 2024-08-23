using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
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
    public GameObject monkey;

    public AudioClip[] wordClips; // Array de clips de audio para las palabras correctas
    public AudioClip monkeyEat;
    public AudioClip appleCrash;
    public AudioClip wrongAnswer;
    public AudioClip correctAnswer;
    private AudioSource audioSource; // Fuente de audio
    public float timeBetweenRepeats = 15f; // Tiempo entre las repeticiones del sonido


    public float fallSpeed = 3f;
    public float minX = -2f; // L�mite izquierdo del �rbol
    public float maxX = 2f;  // L�mite derecho del �rbol
    public float startY = 3.5f; // Altura inicial de la manzana

    public float[] separationPieces = { 0f, -6f, 6f };
    public float[] startYPieces = { -2.61f, -2.81f, -2.71f };

    private Animator animator;
    private Animator animatorMonkey;
    public float rotateSpeed = 100f;

    public int score = 100; // Puntaje inicial
    public int pointsPerCorrectAnswer = 10; // Puntos por acierto
    public int pointsPerIncorrectAnswer = 10; // Puntos que se restan por error
    public int minScore = 20; // Puntaje m�nimo

    private int currentRound = 0;
    private int totalRounds = 2;
    private bool roundFinished = false;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType<GameController>().Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        animator = appleWhole.GetComponent<Animator>();
        animatorMonkey = monkey.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en el GameController.");
            return;
        }
        StartRound();
        InvokeRepeating("PlayCorrectSound", timeBetweenRepeats, timeBetweenRepeats);
    }

    void StartRound()
    {
        roundFinished = false;

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
                textMeshPro.text = textPool[i];
            }
        }
        // Aqu� podr�as hacer otras configuraciones iniciales para la ronda

        // Configurar y lanzar la manzana para la ronda
        FallAndBreak();
    }

    void FallAndBreak()
    {
        // Configurar la posici�n inicial de la manzana
        float randomX = Random.Range(minX, maxX);
        appleWhole.transform.position = new Vector3(randomX, startY, appleWhole.transform.position.z);
        int x = 0;
        float randomFlip = Random.Range(0f, 1f);
        float randomRotation = Random.Range(-0.5f, 0.5f);
        foreach (GameObject piece in applePieces)
        {
            float positionX = randomX + separationPieces[x];
            piece.transform.position = new Vector3(positionX, startYPieces[x], piece.transform.position.z);

            piece.transform.rotation = Quaternion.Euler(new Vector3(0, 0, randomRotation));
            SpriteRenderer spriteRenderer = piece.GetComponent<SpriteRenderer>();
            if (randomFlip > 0.5f)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            randomFlip = Random.Range(0f, 1f);
            x += 1;
        }
        appleWhole.SetActive(true);
        // Iniciar la secuencia de animaci�n y ca�da
        StartCoroutine(WobbleAndFall());
    }
    public void monkeyEatSound()
    {
        StartCoroutine(PlaySoundEnvironment(monkeyEat));

    }
    public void wrongAnswerSound()
    {
        StartCoroutine(PlaySoundEnvironment(wrongAnswer));

    }
    public void correctAnswerSound()
    {
        StartCoroutine(PlaySoundEnvironment(correctAnswer));

    }
    private IEnumerator WobbleAndFall()
    {
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject: " + gameObject.name);
            yield return null;
        }
        // Reproducir la animaci�n de tambaleo
        animator.Play("AppleWobble");

        // Esperar a que la animaci�n "AppleWobble" termine completamente
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        // Iniciar la ca�da de la manzana
        animator.Play("AppleFall");
        yield return null;
        fallSpeed = 3f;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || animator.IsInTransition(0))
        {
            // Mueve la manzana hacia abajo
            appleWhole.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null; // Esperar un frame antes de continuar el bucle
        }

        // Detener el movimiento (manzana ha tocado el suelo o ha terminado la animaci�n)
        fallSpeed = 0f;
        // Desactivar la manzana completa y activar los pedazos
        appleWhole.SetActive(false);
        StartCoroutine(PlaySoundEnvironment(appleCrash));
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
        // Inicia la repetici�n peri�dica del sonido
        InvokeRepeating("PlayCorrectSound", 1f, timeBetweenRepeats);

        yield return new WaitUntil(() => isRoundFinished());
        CancelInvoke("PlayCorrectSound");
        StopCurrentSound();
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
        // Aqu� podr�as mostrar una pantalla de resultados o reiniciar el juego.
        SceneManager.LoadScene("RecogeManzanas - Points");
    }

    private void StopCurrentSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop(); // Detener cualquier sonido que est� en reproducci�n
        }
    }

    private void PlayCorrectSound()
    {
        AudioClip correctClip = GetAudioClipForWord(currentCorrectText);
        if (correctClip != null)
        {
            // Detener cualquier sonido en curso antes de reproducir el nuevo
            StopCurrentSound();
            StartCoroutine(PlaySoundRepeatedly(correctClip));
        }
        else
        {
            Debug.LogError("No se encontr� el clip de audio para la palabra: " + currentCorrectText);
        }
    }

    private AudioClip GetAudioClipForWord(string word)
    {
        // Este m�todo deber�a mapear la palabra correcta a su correspondiente AudioClip
        for (int i = 0; i < wordClips.Length; i++)
        {
            if (wordClips[i].name == word)
            {
                return wordClips[i];
            }
        }
        return null;
    }

    private IEnumerator PlaySoundEnvironment(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            yield return null;
        }
    }

    private IEnumerator PlaySoundRepeatedly(AudioClip clip)
    {
        if (clip != null)
        {
            animatorMonkey.Play("MonoHabla", 0, 0f);
            // Reproducir el sonido la primera vez
            audioSource.PlayOneShot(clip);

            // Esperar la duraci�n del sonido antes de reproducirlo de nuevo
            yield return new WaitForSeconds(clip.length);

            // Reproducir el sonido por segunda vez
            audioSource.PlayOneShot(clip);
        }
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
    }
}
