using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceToMonkey : MonoBehaviour
{
    public GameObject monkey; // Referencia al GameObject del mono
    public GameObject like;
    public GameObject screenFlash; // Referencia al GameObject que iluminará la pantalla
    public float fadeDuration = 0.5f;
    private GameController gameController;
    private Animator animator;
    private Animator animatorLike;// Referencia al GameController
    void Start()
    {
        // Obtener referencia al GameController
        gameController = FindObjectOfType<GameController>();
        animator = monkey.GetComponent<Animator>();
        animatorLike = like.GetComponent<Animator>();
        if (gameController == null)
        {
            Debug.LogError("GameController no encontrado en la escena.");
        }
        //screenFlash = GameObject.Find("ScreenFlash");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == monkey)
        {
            TMP_Text textMeshPro = GetComponentInChildren<TMP_Text>();

            if (textMeshPro != null)
            {
                bool isCorrect = textMeshPro.text == gameController.currentCorrectText;
                Debug.Log(isCorrect);
                // Ajustar el puntaje basado en si es correcto o incorrecto
                gameController.AdjustScore(isCorrect);

                // Iluminar la pantalla según si es correcto o incorrecto
                Color flashColor = isCorrect ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.5f);

                if (isCorrect)
                {

                    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.enabled = false;
                    }

                    // Desactivar cualquier otro componente visual relevante
                    MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = false;
                    }
                    gameController.monkeyEatSound();
                    StartCoroutine(AnimationsCorrect());


                }
                else
                {
                    gameController.wrongAnswerSound();
                    StartCoroutine(FlashScreen(flashColor, isCorrect));

                }



            }
        }
    }
    private IEnumerator AnimationsCorrect()
    {
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject: " + monkey.name);
            yield return null;
        }
        if (animatorLike == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject: " + like.name);
            yield return null;
        }
        animator.Play("MonkeyCome", 0, 0f);
        like.SetActive(true);
        animatorLike.Play("LikeCrece", 0, 0f);
        // Espera un frame para asegurarte de que la animación ha empezado
        yield return null;

        // Obtén la longitud de la animación actual
        float animationLength = animatorLike.GetCurrentAnimatorStateInfo(0).length;

        // Espera el tiempo que dura la animación
        yield return new WaitForSeconds(animationLength);

        // Desactiva el GameObject
        like.SetActive(false);
        // Espera 5 segundos adicionales
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        gameController.finishRound();


    }
    private IEnumerator FlashScreen(Color color, bool isCorrect)
    {
        // Activar la pantalla de color
        Image flashImage = screenFlash.GetComponent<Image>();
        Color initialColor = new Color(color.r, color.g, color.b, 0f);
        flashImage.color = initialColor;

        screenFlash.SetActive(true);



        float elapsedTime = 0f;

        // Realizar la transición al color objetivo
        while (elapsedTime < fadeDuration)
        {
            flashImage.color = Color.Lerp(initialColor, color, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que el color final sea exactamente el targetColor
        flashImage.color = color;

        // Esperar un momento antes de desvanecer de nuevo a transparente
        yield return new WaitForSeconds(0.5f);

        // Desvanecer de nuevo a transparente
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            flashImage.color = Color.Lerp(color, initialColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que el color sea completamente transparente y desactivar el panel
        flashImage.color = initialColor;
        // Esperar un momento (por ejemplo, 0.5 segundos)
        yield return new WaitForSeconds(0.5f);
        flashImage.color = new Color(0, 0, 0, 0);
        screenFlash.SetActive(false);

    }
}
