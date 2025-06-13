using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalDeSaida : MonoBehaviour
{
    [Tooltip("O nome exato do arquivo da cena para carregar.")]
    public string nomeDaProximaCena;

    private bool foiAtivado = false; // Trava para impedir ativação múltipla

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (foiAtivado) return; // Se já foi ativado, não faz nada.

        if (collision.CompareTag("Player"))
        {
            if (string.IsNullOrEmpty(nomeDaProximaCena))
            {
                Debug.LogError("O nome da próxima cena não foi definido no Inspector do Portal!", this);
                return;
            }

            foiAtivado = true; // Ativa a trava

            // Chama o Singleton do FadeScript para iniciar a transição
            FadeScript.Instance.StartFade(() =>
            {
                SceneManager.LoadScene(nomeDaProximaCena);
            });
        }
    }
}