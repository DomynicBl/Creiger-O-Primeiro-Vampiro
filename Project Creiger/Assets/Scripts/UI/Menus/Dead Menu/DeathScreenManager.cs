using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class DeathScreenManager : MonoBehaviour
{
    [Header("UI Panels - Hierarquia")]
    [SerializeField] private GameObject deadOverlayPanel;
    [SerializeField] private GameObject deadMenuPanel;
    [SerializeField] private GameObject deadPanel;

    [Header("Canvas Group para Fade")]
    [SerializeField] private CanvasGroup deadOverlayCanvasGroup;

    [Header("UI Elements")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text deathMessageText;

    [Header("Game References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameObject[] enemiesToStop;

    private void Awake()
    {
        if (deadOverlayPanel != null)
        {
            deadOverlayPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("[DeathScreenManager] Death Screen UI GameObject not assigned! Please assign it in the Inspector.", this);
            this.enabled = false;
            return;
        }

        if (deadOverlayCanvasGroup == null)
            deadOverlayCanvasGroup = deadOverlayPanel.GetComponent<CanvasGroup>();

        if (deadOverlayCanvasGroup != null)
        {
            deadOverlayCanvasGroup.alpha = 0f;
            deadOverlayCanvasGroup.interactable = false;
            deadOverlayCanvasGroup.blocksRaycasts = false;
        }
        if (deadMenuPanel != null) deadMenuPanel.SetActive(false);
        if (deadPanel != null) deadPanel.SetActive(false);

        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }
            if (playerHealth == null)
            {
                Debug.LogError("[DeathScreenManager] Player Health component not assigned or found! Make sure the Player has the 'Player' tag and a Health component.", this);
                this.enabled = false;
                return;
            }
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitToMainMenu); // <--- MUDANÇA AQUI!
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath += ShowDeathScreen;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= ShowDeathScreen;
        }
    }

    private void ShowDeathScreen()
    {
        Time.timeScale = 0f; // Pausa o jogo imediatamente

        Debug.Log("[DeathScreenManager] Player morreu! Exibindo tela de morte.");
        StartCoroutine(FadeInDeathScreen());
        if (deathMessageText != null)
        {
            deathMessageText.text = "Você Morreu!";
        }
        StopEnemiesCombat();
    }

    IEnumerator FadeInDeathScreen()
    {
        if (deadOverlayPanel == null || deadOverlayCanvasGroup == null) yield break;
        deadOverlayPanel.SetActive(true);
        deadMenuPanel.SetActive(true);
        deadPanel.SetActive(true);
        deadOverlayCanvasGroup.alpha = 1f;
        deadOverlayCanvasGroup.interactable = true;
        deadOverlayCanvasGroup.blocksRaycasts = true;
        yield return null;
    }

    private void StopEnemiesCombat()
    {
        if (enemiesToStop != null)
        {
            foreach (GameObject enemy in enemiesToStop)
            {
                if (enemy != null)
                {
                    MeleeEnemy meleeEnemy = enemy.GetComponent<MeleeEnemy>();
                    if (meleeEnemy != null)
                    {
                        meleeEnemy.enabled = false;
                        Debug.Log($"[DeathScreenManager] Combate do inimigo {enemy.name} parado.");
                    }

                    Animator enemyAnim = enemy.GetComponent<Animator>();
                    if (enemyAnim != null)
                    {
                        enemyAnim.ResetTrigger("meleeAttack");
                        enemyAnim.speed = 1f;
                    }

                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb != null) enemyRb.simulated = true;
                }
            }
        }
    }

    public void RestartGame()
    {
        Debug.Log("[DeathScreenManager] Reiniciando jogo...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu() // <--- NOVO MÉTODO
    {
        Debug.Log("[DeathScreenManager] Saindo para o Menu Principal...");
        Time.timeScale = 1f; // Garante que o tempo esteja normalizado
        SceneManager.LoadScene("Main Menu"); // <--- Carrega a cena "Main Menu"
    }
}