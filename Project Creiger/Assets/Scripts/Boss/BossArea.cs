using UnityEngine;

public class BossAreaTrigger : MonoBehaviour
{
    public GameObject bossBarUI; // Arraste aqui o UI da boss bar

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossBarUI.SetActive(true); // Ativa a boss bar
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossBarUI.SetActive(false); // Desativa ao sair (opcional)
        }
    }
}
