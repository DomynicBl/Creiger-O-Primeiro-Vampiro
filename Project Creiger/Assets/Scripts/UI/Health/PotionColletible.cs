using UnityEngine;

public class PotionCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Usa CompareTag para performance e segurança
        {
            PlayerInventory playerInventory = collision.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                if (playerInventory.AddPotion()) // Tenta adicionar a poção ao inventário do jogador
                {
                    gameObject.SetActive(false); // Desativa (remove) a poção do cenário
                    Debug.Log("[PotionCollectible] Poção coletada e desativada.");
                }
            }
        }
    }
}