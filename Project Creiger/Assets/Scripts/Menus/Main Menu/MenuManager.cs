using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems; // Importar para usar EventSystem se necessário, mas com overlay, menos crítico

public class MenuManager : MonoBehaviour{
    [Header("Panels")]
    public GameObject settingsPanel;    // GameObject "Settings" para aqui
    public GameObject settingsOverlay;  // GameObject "SettingsOverlay" para aqui

    [Header("Fade Settings")]
    public Image fadePanel;
    public float fadeDuration = 1.0f;
    public float settingsFadeDuration = 0.5f; // Duração para o fade do painel de configurações

    private CanvasGroup settingsCanvasGroup; // Referência ao CanvasGroup do settingsPanel
    private bool isSettingsPanelOpen = false;
    private Coroutine currentSettingsFadeCoroutine; // Controla a coroutine de fade

    void Start(){
        // Garante que o CanvasGroup existe no settingsPanel
        settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
        if (settingsCanvasGroup == null){
            settingsCanvasGroup = settingsPanel.AddComponent<CanvasGroup>();
        }

        if (settingsPanel != null){
            settingsPanel.SetActive(false);
            settingsCanvasGroup.alpha = 0f; // Começa invisível
            settingsCanvasGroup.interactable = false; // Não interagível quando invisível
            settingsCanvasGroup.blocksRaycasts = false; // Não bloqueia eventos de raycast quando invisível
        }
        
        if (settingsOverlay != null){
            settingsOverlay.SetActive(false); // Garante que o overlay esteja desativado no início
        }
        
        if (fadePanel != null){
            Color panelColor = fadePanel.color;
            panelColor.a = 0f;
            fadePanel.color = panelColor;
            fadePanel.gameObject.SetActive(false);
        }
    }

    // Função para o botão "Play"
    public void PlayGame(){
        Debug.Log("Iniciando transição para Level 1...");
        StartCoroutine(FadeAndLoadScene("Level 1"));
    }

    IEnumerator FadeAndLoadScene(string sceneName){
        if (fadePanel != null){
            fadePanel.gameObject.SetActive(true);
            Color panelColor = fadePanel.color;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                panelColor.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                fadePanel.color = panelColor;
                yield return null;
            }
            panelColor.a = 1f;
            fadePanel.color = panelColor;
        }

        Debug.Log("Carregando cena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // Função para o botão "Settings"
    public void OpenSettings(){
        if (settingsPanel != null && settingsCanvasGroup != null){
            if (currentSettingsFadeCoroutine != null)
            {
                StopCoroutine(currentSettingsFadeCoroutine); // Para qualquer coroutine de fade anterior
            }
            currentSettingsFadeCoroutine = StartCoroutine(FadeSettingsPanel(true));
        }
    }

    // Função para fechar o painel de configurações (chamada pelo botão do overlay)
    public void CloseSettingsPanel(){
        if (settingsPanel != null && settingsCanvasGroup != null){
            if (currentSettingsFadeCoroutine != null)
            {
                StopCoroutine(currentSettingsFadeCoroutine); // Para qualquer coroutine de fade anterior
            }
            currentSettingsFadeCoroutine = StartCoroutine(FadeSettingsPanel(false));
        }
    }

    IEnumerator FadeSettingsPanel(bool fadeIn){
        float startAlpha = settingsCanvasGroup.alpha;
        float endAlpha = fadeIn ? 1f : 0f;
        float timer = 0f;

        if (fadeIn){
            settingsPanel.SetActive(true);
            if (settingsOverlay != null) settingsOverlay.SetActive(true);
            settingsCanvasGroup.interactable = false; // Desativa interação durante o fade in
            settingsCanvasGroup.blocksRaycasts = false; // Desativa bloqueio durante o fade in
        }

        while (timer < settingsFadeDuration){
            timer += Time.deltaTime;
            settingsCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / settingsFadeDuration);
            yield return null;
        }

        settingsCanvasGroup.alpha = endAlpha; // Garante que a opacidade final seja definida

        if (fadeIn){
            isSettingsPanelOpen = true;
            settingsCanvasGroup.interactable = true; // Ativa interação após o fade in
            settingsCanvasGroup.blocksRaycasts = true; // Ativa bloqueio após o fade in
            Debug.Log("Painel de Configurações aberto com fade in.");
        }else{
            settingsPanel.SetActive(false);
            if (settingsOverlay != null) settingsOverlay.SetActive(false);
            isSettingsPanelOpen = false;
            settingsCanvasGroup.interactable = false; // Desativa interação quando invisível
            settingsCanvasGroup.blocksRaycasts = false; // Desativa bloqueio quando invisível
            Debug.Log("Painel de Configurações fechado com fade out.");
        }
    }

    // Função para o botão "Credits"
    public void ShowCredits(){
        Debug.Log("Créditos (ainda não implementado)");
    }

    // Função para o botão "Quit"
    public void QuitGame(){
        Debug.Log("Saindo do jogo...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}