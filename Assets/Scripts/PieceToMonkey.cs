using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceToMonkey : MonoBehaviour
{
    public GameObject monkey; // Referencia al GameObject del mono
    public GameObject screenFlash; // Referencia al GameObject que iluminar� la pantalla
    public float fadeDuration = 0.5f;
    private GameController gameController;
   private Animator animator; // Referencia al GameController
    void Start()
    {
        // Obtener referencia al GameController
        gameController = FindObjectOfType<GameController>();
       animator = monkey.GetComponent<Animator>();
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

                // Iluminar la pantalla seg�n si es correcto o incorrecto
                Color flashColor = isCorrect ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.5f);

                StartCoroutine(FlashScreen(flashColor, isCorrect));
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
                    animator.Play("MonkeyCome", 0, 0f);
                }



            }
        }
    }

    private IEnumerator FlashScreen(Color color, bool isCorrect)
    {
        // Activar la pantalla de color
        Image flashImage = screenFlash.GetComponent<Image>();
        Color initialColor = new Color(color.r, color.g, color.b, 0f);
        flashImage.color = initialColor;

        screenFlash.SetActive(true);



        float elapsedTime = 0f;

        // Realizar la transici�n al color objetivo
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
        if (isCorrect)
        {
            gameObject.SetActive(false);
            gameController.finishRound();
        }
    }
}
