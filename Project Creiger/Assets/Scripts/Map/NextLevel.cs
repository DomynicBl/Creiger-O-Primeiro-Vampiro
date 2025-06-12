using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalDeSaida : MonoBehaviour
{
    public FadeScript fadeScript;  // arraste no inspector o objeto com o fade

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int cenaAtual = SceneManager.GetActiveScene().buildIndex;
            int proximaCena = cenaAtual + 1;

            if (proximaCena < SceneManager.sceneCountInBuildSettings)
            {
                fadeScript.StartFade(() =>
                {
                    SceneManager.LoadScene(proximaCena);
                });
            }
            else
            {
                Debug.Log("Fim do jogo ou fases completas.");
                // Pode fazer fade e carregar menu, por exemplo
            }
        }
    }
}
