// Arquivo: MinotauroAI.cs
using UnityEngine;

public class MinotauroAI : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private Health playerHealth; // Referência para a vida do player

    [Header("Parâmetros de Combate")]
    public float speed = 2.5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float attackDamage = 0.5f; // <<--- NOVO CAMPO DE DANO!

    private float nextAttackTime = 0f;
    private Vector3 escalaInicial;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        escalaInicial = transform.localScale;

        // Encontra o jogador e seu script de vida
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = playerObj.GetComponent<Health>(); // Pega o script Health do player
            }
        }
    }

    void Update()
    {
        // Substitua 'IsDead' pela propriedade correta do seu script Health, se necessário
        // Substitua 'currentHealth' pelo nome correto do campo/propriedade de vida no seu script Health
        if (player == null || playerHealth == null || playerHealth.currentHealth <= 0) // Para de funcionar se o player não existe ou está morto
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("moving", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        VirarParaOJogador();

        if (distanceToPlayer <= attackRange)
        {
            Atacar();
        }
        else
        {
            Perseguir();
        }

        anim.SetBool("moving", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    }

    void VirarParaOJogador()
    {
        float direcaoParaOJogador = player.position.x - transform.position.x;
        if (Mathf.Abs(direcaoParaOJogador) > 0.01f) // Evita virar quando está exatamente na mesma posição
        {
            float ladoCorreto = Mathf.Sign(direcaoParaOJogador);
            transform.localScale = new Vector3(Mathf.Abs(escalaInicial.x) * ladoCorreto, escalaInicial.y, escalaInicial.z);
        }
    }

    void Perseguir()
    {
        float moveDirection = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(moveDirection * speed, rb.linearVelocity.y);
    }

    void Atacar()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (Time.time >= nextAttackTime)
        {
            anim.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // <<--- NOVA FUNÇÃO CHAMADA PELA ANIMAÇÃO ---
    // Esta função será chamada por um Animation Event no frame exato do ataque.
    public void DealDamage()
    {
        // Verifica se o jogador ainda está no alcance para levar o dano
        if (playerHealth != null && Vector2.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            Debug.Log($"Boss atacou e causou {attackDamage} de dano.");
            playerHealth.TakeDamage(attackDamage);
        }
    }
}