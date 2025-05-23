using UnityEngine;
using System; // Necessário para o evento Action

public class PlayerInventory : MonoBehaviour
{
    private int _potionCount = 0;
    private const int MaxPotions = 3; // Máximo de poções que o jogador pode carregar
    [SerializeField] private float healAmountPercentage = 0.2f; // Cura 20% da vida máxima

    public event Action<int> OnPotionCountChanged; // Evento para notificar o HUD sobre mudanças no contador de poções

    private Health _playerHealth; // Referência ao script de vida do jogador

    void Start()
    {
        _playerHealth = GetComponent<Health>(); // Obtém o componente Health do mesmo GameObject (o Player)
        if (_playerHealth == null)
        {
            Debug.LogError("[PlayerInventory] Health component not found on this GameObject! PlayerInventory requires a Health component.", this);
        }
        OnPotionCountChanged?.Invoke(_potionCount); // Notifica o HUD para inicializar com 0 poções
    }

    // Tenta adicionar uma poção ao inventário
    // Retorna true se a poção foi adicionada com sucesso, false se o inventário estiver cheio.
    public bool AddPotion()
    {
        if (_potionCount < MaxPotions)
        {
            _potionCount++;
            OnPotionCountChanged?.Invoke(_potionCount); // Notifica o HUD
            Debug.Log($"[PlayerInventory] Poção coletada! Poções atuais: {_potionCount}");
            return true;
        }
        Debug.Log("[PlayerInventory] Inventário de poções cheio!");
        return false;
    }

    // Usa uma poção, consumindo uma do inventário e curando o jogador
    public void UsePotion()
    {
        if (_potionCount > 0)
        {
            _potionCount--;
            OnPotionCountChanged?.Invoke(_potionCount); // Notifica o HUD

            // Cura o jogador com base na porcentagem da vida máxima
            _playerHealth.AddHealth(_playerHealth.MaxHealth * healAmountPercentage);
            Debug.Log($"[PlayerInventory] Poção usada! Vida: {_playerHealth.currentHealth}. Poções restantes: {_potionCount}");
        }
        else
        {
            Debug.Log("[PlayerInventory] Sem poções para usar!");
        }
    }

    void Update()
    {
        // Se a tecla "E" for pressionada, tenta usar uma poção
        if (Input.GetKeyDown(KeyCode.E))
        {
            UsePotion();
        }
    }
}