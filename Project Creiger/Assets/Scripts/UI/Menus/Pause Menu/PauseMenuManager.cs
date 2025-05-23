using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels - Hierarquia")]
    public GameObject pauseOverlayPanel;  // O pai de tudo, escurece o fundo (PauseOverlay)
    public GameObject pauseMenuPanel;     // O painel do menu de pause (PauseMenu)
    public GameObject pauseSettingsPanel; // O painel de configurações (PauseSettings)

    [Header("Canvas Groups para Fades")]
    public CanvasGroup pauseOverlayCanvasGroup; // CanvasGroup do PauseOverlay (para fade do menu inteiro)
    public CanvasGroup settingsCanvasGroup;     // CanvasGroup do PauseSettings (para fade das configurações)

    // Removidas as variáveis públicas de duração de fade
    // private float overlayFadeDuration = 0f; // Valor fixo para fade instantâneo
    // private float settingsFadeDuration = 0f; // Valor fixo para fade instantâneo

    private bool isPaused = false;
    private bool isSettingsOpen = false;

    void Awake()
    {
        // Garante que os Canvas Groups estejam atribuídos. Se não, tenta pegar automaticamente.
        if (pauseOverlayPanel != null && pauseOverlayCanvasGroup == null)
            pauseOverlayCanvasGroup = pauseOverlayPanel.GetComponent<CanvasGroup>();
        if (pauseSettingsPanel != null && settingsCanvasGroup == null)
            settingsCanvasGroup = pauseSettingsPanel.GetComponent<CanvasGroup>();

        // Garante que todos os painéis estejam desativados e transparentes no início
        if (pauseOverlayCanvasGroup != null)
        {
            pauseOverlayCanvasGroup.alpha = 0f;
            pauseOverlayCanvasGroup.interactable = false;
            pauseOverlayCanvasGroup.blocksRaycasts = false;
        }
        if (pauseOverlayPanel != null) pauseOverlayPanel.SetActive(false);

        if (settingsCanvasGroup != null)
        {
            settingsCanvasGroup.alpha = 0f;
            settingsCanvasGroup.interactable = false;
            settingsCanvasGroup.blocksRaycasts = false;
        }
        if (pauseSettingsPanel != null) pauseSettingsPanel.SetActive(false);
        
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f; // Garante que o jogo não comece pausado
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSettingsOpen)
            {
                CloseSettings();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        StartCoroutine(FadeInOverlay(pauseMenuPanel));
        Debug.Log("Jogo Pausado.");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        StartCoroutine(FadeOutOverlay());
        Debug.Log("Jogo Retomado.");
    }

    public void OpenSettings()
    {
        isSettingsOpen = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        StartCoroutine(FadeInSettings());
        Debug.Log("Abrindo Configurações do Pause.");
    }

    public void CloseSettings()
    {
        isSettingsOpen = false;
        StartCoroutine(FadeOutSettings());
        Debug.Log("Fechando Configurações do Pause.");
    }

    public void ExitGameToMainMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Saindo para o Menu Principal.");
        SceneManager.LoadScene("Main Menu");
    }

    // --- Coroutines de Fade (com duração fixa em 0) ---

    IEnumerator FadeInOverlay(GameObject contentToActivate)
    {
        if (pauseOverlayPanel == null || pauseOverlayCanvasGroup == null) yield break;

        pauseOverlayPanel.SetActive(true);
        // Não há loop de interpolação, apenas define o alpha final
        pauseOverlayCanvasGroup.alpha = 1f; 
        
        if (contentToActivate != null) contentToActivate.SetActive(true); 

        pauseOverlayCanvasGroup.interactable = true;
        pauseOverlayCanvasGroup.blocksRaycasts = true;
        yield return null; // Retorna um frame para garantir que o CanvasGroup seja processado
    }

    IEnumerator FadeOutOverlay()
    {
        if (pauseOverlayPanel == null || pauseOverlayCanvasGroup == null) yield break;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false); 
        if (pauseSettingsPanel != null) pauseSettingsPanel.SetActive(false); 

        pauseOverlayCanvasGroup.interactable = false;
        pauseOverlayCanvasGroup.blocksRaycasts = false;

        // Não há loop de interpolação, apenas define o alpha final
        pauseOverlayCanvasGroup.alpha = 0f;
        pauseOverlayPanel.SetActive(false); 
        yield return null; // Retorna um frame
    }

    IEnumerator FadeInSettings()
    {
        if (pauseSettingsPanel == null || settingsCanvasGroup == null) yield break;

        settingsCanvasGroup.alpha = 0f; // Garante que comece invisível
        pauseSettingsPanel.SetActive(true); // Ativa o painel

        // Não há loop de interpolação, apenas define o alpha final
        settingsCanvasGroup.alpha = 1f;
        settingsCanvasGroup.interactable = true;
        settingsCanvasGroup.blocksRaycasts = true;
        yield return null; // Retorna um frame
    }

    IEnumerator FadeOutSettings()
    {
        if (pauseSettingsPanel == null || settingsCanvasGroup == null) yield break;

        settingsCanvasGroup.interactable = false;
        settingsCanvasGroup.blocksRaycasts = false;

        // Não há loop de interpolação, apenas define o alpha final
        settingsCanvasGroup.alpha = 0f;
        pauseSettingsPanel.SetActive(false); 
        yield return null; // Retorna um frame

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true); // Volta para o menu de pause
    }
}