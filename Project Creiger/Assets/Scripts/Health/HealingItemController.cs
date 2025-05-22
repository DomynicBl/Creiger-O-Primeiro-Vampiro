using UnityEngine;

public class HealingItemController : MonoBehaviour
{
    [SerializeField] private float healAmount = 5f;
    [SerializeField] private float cooldown = 60f;
    [SerializeField] private int maxUses = 3;

    private float lastUseTime = -Mathf.Infinity;
    private int usesRemaining;
    private Health playerHealth;
    private HealingItemUI healingItemUI;

    private void Start()
    {
        usesRemaining = maxUses;

        playerHealth = FindFirstObjectByType<Health>();
        if (playerHealth == null)
            Debug.LogError("Health não encontrado na cena!");

        // Tenta encontrar o HealingItemUI no mesmo GameObject ou em um objeto relacionado
        healingItemUI = GetComponentInChildren<HealingItemUI>();  // Se estiver em um filho do objeto

        if (healingItemUI == null)
            Debug.LogWarning("HealingItemUI não atribuído.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryUseItem();
        }
    }

    public void TryUseItem()
    {
        if (usesRemaining <= 0)
        {
            Debug.Log("Todos os usos do item de cura foram consumidos.");
            return;
        }

        if (Time.time < lastUseTime + cooldown)
        {
            float timeLeft = (lastUseTime + cooldown) - Time.time;
            Debug.Log($"Item em recarga. Aguarde {timeLeft:F1} segundos.");
            return;
        }

        // Cura o jogador
        playerHealth.AddHealth(healAmount);
        lastUseTime = Time.time;
        usesRemaining--;

        Debug.Log($"Item de cura usado! Usos restantes: {usesRemaining}");

        // Atualiza a UI visual
        if (healingItemUI != null)
        {
            healingItemUI.UpdateVisual(usesRemaining <= 0);
        }
    }
}
