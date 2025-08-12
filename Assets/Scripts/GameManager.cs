using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Aseg�rate de tener este 'using' si usas TextMeshPro

/// <summary>
/// Gestiona el estado del juego, incluyendo la pantalla de muerte y la transici�n a la escena del men� principal.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Una instancia est�tica para que pueda ser accedida f�cilmente desde otros scripts.
    public static GameManager Instance { get; private set; }

    [Header("Death Screen")]
    public GameObject deathScreen;
    public TextMeshProUGUI countdownText; // Referencia al texto de la cuenta regresiva

    void Awake()
    {
        // Implementa el patr�n Singleton para asegurar que solo hay una instancia.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Se asegura de que la pantalla de muerte est� inicialmente oculta.
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    /// <summary>
    /// Se llama cuando el jugador muere. Muestra la pantalla de muerte,
    /// pausa el juego y comienza la cuenta regresiva.
    /// </summary>
    public void PlayerDied()
    {
        // Muestra la pantalla de muerte y activa el cursor para interactuar con la UI.
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Inicia la corrutina para la cuenta regresiva.
            StartCoroutine(CountdownToMainMenu());
        }
    }

    /// <summary>
    /// Corrutina que maneja la cuenta regresiva y la transici�n al men� principal.
    /// </summary>
    private IEnumerator CountdownToMainMenu()
    {
        int countdown = 5;

        while (countdown > 0)
        {
            // Actualiza el texto de la cuenta regresiva.
            if (countdownText != null)
            {
                countdownText.text = "Regresando al men�... " + countdown.ToString();
            }
            yield return new WaitForSecondsRealtime(1f); // Espera 1 segundo en tiempo real.
            countdown--;
        }

        // Si la cuenta regresiva ha terminado, regresa al men� principal.
        GoToMainMenu();
    }

    /// <summary>
    /// Carga la escena del men� principal y reanuda el tiempo.
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Reanuda el juego
        SceneManager.LoadScene("MainMenu");

        // Oculta la pantalla de muerte para que no se muestre en el men�
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    // La funci�n RestartLevel ya no es necesaria con esta nueva l�gica.
    // public void RestartLevel()
    // {
    //     Time.timeScale = 1f;
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }
}
