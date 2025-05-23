using UnityEngine;
using System.Collections;
using System; // Necessário para usar Action (eventos)

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth; // Vida máxima inicial do objeto
    public float currentHealth { get; private set; } // Vida atual

    private Animator anim;
    private bool dead;

    // Propriedade pública para acessar a vida máxima
    public float MaxHealth
    {
        get { return startingHealth; }
    }

    // Eventos para comunicação com outros scripts (novos)
    public event Action OnPlayerDamaged; // Disparado quando o PLAYER toma dano efetivo
    public event Action OnDeath;         // Disparado quando a vida do objeto chega a zero

    private SpriteRenderer spriteRend; // Para efeitos visuais ao tomar dano

    [Header("Components")]
    [SerializeField] private Behaviour[] componentsToDisableOnDeath; // Scripts a desabilitar na morte

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        // Não causa dano se já está morto ou o dano é zero ou negativo
        if (currentHealth <= 0 || _damage <= 0) return;

        float healthBeforeDamage = currentHealth; // Guarda a vida antes de aplicar o dano
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        // Dispara o evento OnPlayerDamaged SOMENTE se o objeto é o Player E a vida realmente diminuiu
        if (this.gameObject.CompareTag("Player") && currentHealth < healthBeforeDamage)
        {
            Debug.Log($"[Health] PLAYER: Recebeu dano efetivo! Vida antes: {healthBeforeDamage}, Vida agora: {currentHealth}. Disparando OnPlayerDamaged.");
            StartCoroutine(FlashRed()); // Efeito visual de piscar vermelho
            OnPlayerDamaged?.Invoke();
        }
        else if (!this.gameObject.CompareTag("Player") && currentHealth < healthBeforeDamage)
        {
            // Log e efeito para inimigos tomando dano
            Debug.Log($"[Health] {gameObject.name}: Recebeu dano efetivo! Vida antes: {healthBeforeDamage}, Vida agora: {currentHealth}.");
            StartCoroutine(FlashRed()); // Efeito visual de piscar vermelho
        }

        if (currentHealth <= 0) // Morreu
        {
            if (!dead) // Garante que a lógica de morte só rode uma vez
            {
                anim.SetTrigger("die"); // Animação de morte

                // Desativa todos os componentes anexados listados
                if (componentsToDisableOnDeath != null)
                {
                    foreach (Behaviour component in componentsToDisableOnDeath)
                    {
                        if (component != null) // Verifica se o componente não é nulo
                            component.enabled = false;
                    }
                }

                dead = true;
                OnDeath?.Invoke(); // Dispara o evento de morte
                Debug.Log($"[Health] {gameObject.name} morreu. Disparando OnDeath.");
            }
        }
        else // Tomou dano, mas não morreu
        {
            anim.SetTrigger("hurt"); // Animação de dor
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
        Debug.Log($"[Health] {gameObject.name}: Curado em {_value}. Vida atual: {currentHealth}.");
    }

    // Corrotina para fazer o sprite piscar vermelho (feedback de dano)
    private IEnumerator FlashRed()
    {
        if (spriteRend != null)
        {
            Color originalColor = spriteRend.color;
            spriteRend.color = Color.red; // Muda para vermelho
            yield return new WaitForSeconds(0.1f); // Espera um curto período
            spriteRend.color = originalColor; // Volta à cor original
        }
    }

    // Este método é tipicamente chamado por um Animation Event no último frame da animação de morte
    private void Deactivate()
    {
        gameObject.SetActive(false); // Desativa o GameObject
        Debug.Log($"[Health] {gameObject.name} desativado.");
    }
}