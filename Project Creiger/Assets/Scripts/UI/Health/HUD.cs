using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importe este namespace se estiver usando TextMeshPro

public class HUD : MonoBehaviour
{
    [Header("Potion Display")]
    [SerializeField] private Image[] potionImages; // Array de imagens para as poções (para cada slot)

    [Header("Kill Streak Display")]
    [SerializeField] private TMP_Text killCountText; // Objeto de texto para o contador de kills
    [SerializeField] private Image fireEffectImage; // Imagem/Animação para o efeito de fogo do kill streak

    private PlayerInventory _playerInventory;
    private KillStreakManager _killStreakManager;

    private void Start() // Usamos Start, pois não há DontDestroyOnLoad/Singleton aqui
    {
        // Encontra o PlayerInventory na cena
        _playerInventory = Object.FindAnyObjectByType<PlayerInventory>(); // Correção API
        if (_playerInventory == null)
        {
            Debug.LogError("[HUD] PlayerInventory not found in the scene! Make sure it's present and active.", this);
        }
        if (_playerInventory != null)
        {
            _playerInventory.OnPotionCountChanged += UpdatePotionIcons; // Se inscreve no evento de poções
            UpdatePotionIcons(0); // Inicializa os ícones de poção como invisíveis
        }

        // Encontra o KillStreakManager na cena
        _killStreakManager = Object.FindAnyObjectByType<KillStreakManager>(); // Correção API
        if (_killStreakManager == null)
        {
            Debug.LogError("[HUD] KillStreakManager not found in the scene! Make sure it's present and active.", this);
        }
        if (_killStreakManager != null)
        {
            _killStreakManager.OnKillCountChanged += UpdateKillCountText; // Se inscreve no evento de kills
            _killStreakManager.OnFireEffectStateChanged += SetFireEffectState; // Se inscreve no evento do fogo
            UpdateKillCountText(0); // Inicializa o texto do contador de kills com 0
            SetFireEffectState(false); // Inicializa o efeito de fogo como invisível
        }
    }

    // Desinscrição dos eventos quando o objeto é destruído para evitar erros
    private void OnDestroy()
    {
        if (_playerInventory != null)
        {
            _playerInventory.OnPotionCountChanged -= UpdatePotionIcons;
        }
        if (_killStreakManager != null)
        {
            _killStreakManager.OnKillCountChanged -= UpdateKillCountText;
            _killStreakManager.OnFireEffectStateChanged -= SetFireEffectState;
        }
    }

    // Atualiza a visibilidade das imagens de poção no HUD
    private void UpdatePotionIcons(int potionCount)
    {
        if (potionImages == null || potionImages.Length == 0) return;

        for (int i = 0; i < potionImages.Length; i++)
        {
            potionImages[i].enabled = (i < potionCount);
        }
    }

    // Atualiza o texto do contador de kills para exibir apenas o número
    private void UpdateKillCountText(int killCount)
    {
        if (killCountText != null)
        {
            killCountText.text = killCount.ToString();
        }
    }

    // Ativa ou desativa a imagem do efeito de fogo no HUD
    private void SetFireEffectState(bool active)
    {
        if (fireEffectImage != null)
        {
            fireEffectImage.enabled = active;
        }
    }
}