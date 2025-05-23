using UnityEngine;
using UnityEngine.UI; // Para usar o componente Image

public class PotionManager : MonoBehaviour
{
    [Header("Configurações das Poções")]
    public int maxPotions = 3; // O número máximo de poções que o jogador pode carregar
    [SerializeField] // Permite que a variável privada seja visível e editável no Inspector
    private int currentPotions = 0; // Quantidade atual de poções

    [Header("Referências da UI")]
    public Image[] potionIcons; // Array para arrastar as 3 imagens dos potes de poção

    [Header("Configurações de Recuperação")]
    public float healthRecoveredPerPotion = 30f; // Quanto de vida cada poção recupera

    // Referência ao seu script de vida do jogador (seja lá como ele se chama)
    // Exemplo: public PlayerHealth playerHealth; // Você precisará ter um script PlayerHealth com um método para adicionar vida.

    void Start()
    {
        // Garante que a quantidade inicial de poções seja válida
        currentPotions = Mathf.Clamp(currentPotions, 0, maxPotions);

        // Atualiza a UI no início do jogo
        UpdatePotionUI();
    }

    // Método para adicionar uma poção
    public void AddPotion()
    {
        if (currentPotions < maxPotions)
        {
            currentPotions++;
            UpdatePotionUI();
            Debug.Log("Poção adicionada. Poções atuais: " + currentPotions);
        }
        else
        {
            Debug.Log("Não é possível adicionar mais poções. Máximo atingido.");
        }
    }

    // Método para usar uma poção
    public void UsePotion()
    {
        if (currentPotions > 0)
        {
            currentPotions--;
            // Chamada para o script de vida do jogador para recuperar vida
            // Exemplo: if (playerHealth != null) { playerHealth.RecoverHealth(healthRecoveredPerPotion); }
            
            Debug.Log("Poção usada. Poções restantes: " + currentPotions);
            UpdatePotionUI();
        }
        else
        {
            Debug.Log("Nenhuma poção disponível para usar.");
        }
    }

    // Método que atualiza a visibilidade das imagens dos potes na UI
    void UpdatePotionUI()
    {
        // Verifica se o array de ícones foi preenchido corretamente no Inspector
        if (potionIcons == null || potionIcons.Length == 0)
        {
            Debug.LogWarning("PotionManager: Nenhuma imagem de ícone de poção atribuída ou array vazio!");
            return;
        }

        for (int i = 0; i < potionIcons.Length; i++)
        {
            if (potionIcons[i] != null)
            {
                // Se o índice for menor que a quantidade atual de poções, torna visível
                if (i < currentPotions)
                {
                    potionIcons[i].gameObject.SetActive(true); // Ou pode mudar a cor/alpha para indicar "cheio"
                }
                // Caso contrário, desativa (ou muda para indicar "vazio")
                else
                {
                    potionIcons[i].gameObject.SetActive(false); // Ou pode mudar a cor/alpha para indicar "vazio"
                }
            }
        }
    }

    // Método público para obter a quantidade atual de poções (se precisar em outros scripts)
    public int GetCurrentPotions()
    {
        return currentPotions;
    }
}