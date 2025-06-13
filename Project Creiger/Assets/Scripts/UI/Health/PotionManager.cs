using UnityEngine;
using UnityEngine.UI; // Necessário para o componente Image

public class PotionManager : MonoBehaviour
{
    [Header("Referências da UI")]
    public Image[] potionIcons; // Array para arrastar as 3 imagens dos potes de poção

    [Header("Referência ao Jogador")]
    // Precisamos de uma referência ao inventário do jogador para poder "ouvi-lo"
    [SerializeField] private PlayerInventory playerInventory;

    void Start()
    {
        // Verifica se a referência ao inventário foi definida no Inspector
        if (playerInventory == null)
        {
            Debug.LogError("[PotionManager] A referência ao PlayerInventory não foi atribuída no Inspector!", this);
            return;
        }

        // INSCRIÇÃO NO EVENTO:
        // Dizemos ao nosso método UpdatePotionUI para ser executado toda vez que o evento OnPotionCountChanged for disparado.
        playerInventory.OnPotionCountChanged += UpdatePotionUI;
    }

    private void OnDestroy()
    {
        // BOA PRÁTICA:
        // Quando este objeto for destruído, removemos a inscrição para evitar erros.
        if (playerInventory != null)
        {
            playerInventory.OnPotionCountChanged -= UpdatePotionUI;
        }
    }

    // O método agora recebe a contagem atual de poções diretamente do evento.
    void UpdatePotionUI(int currentPotionCount)
    {
        if (potionIcons == null || potionIcons.Length == 0)
        {
            Debug.LogWarning("[PotionManager] Nenhuma imagem de ícone de poção atribuída!");
            return;
        }

        // Itera por todos os ícones da UI
        for (int i = 0; i < potionIcons.Length; i++)
        {
            if (potionIcons[i] != null)
            {
                // Se o índice 'i' for menor que a quantidade de poções, o ícone deve estar ativo.
                // Ex: 2 poções -> ícones 0 e 1 ficam ativos.
                if (i < currentPotionCount)
                {
                    potionIcons[i].gameObject.SetActive(true);
                }
                // Caso contrário, o ícone deve ser desativado.
                else
                {
                    potionIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    // REMOVEMOS os métodos AddPotion(), UsePotion() e a variável currentPotions
    // pois essa lógica agora pertence exclusivamente ao PlayerInventory.
}