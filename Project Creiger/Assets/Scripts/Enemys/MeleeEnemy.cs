using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown; // Tempo entre ataques do inimigo
    [SerializeField] private float range;          // Alcance do ataque do inimigo
    [SerializeField] private int damage;           // Dano causado pelo inimigo

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance; // Distância do collider de ataque
    [SerializeField] private BoxCollider2D boxCollider; // O BoxCollider2D usado para a detecção de ataque

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // A layer do jogador
    private float cooldownTimer = Mathf.Infinity;

    // References
    private Animator anim;
    private Health playerHealth; // Referência ao Health do jogador
    private EnemyPatrol enemyPatrol; // Assumindo que você tem um script de patrulha para o inimigo

    // Adicione uma referência estática ou de fácil acesso ao Health do jogador
    // Isso é útil para que os inimigos possam encontrar o Health do jogador facilmente.
    private static Health playerHealthInstance;

    private bool playerIsDead = false; // Novo estado para verificar se o jogador está morto

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>(); // Tenta encontrar EnemyPatrol no pai, se houver

        // Encontra a instância do Health do jogador uma vez
        // Se você tiver certeza de que o jogador tem a tag "Player", pode fazer assim:
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerHealthInstance = playerObject.GetComponent<Health>();
            if (playerHealthInstance != null)
            {
                // Assina o evento OnDeath do jogador
                playerHealthInstance.OnDeath += OnPlayerDeath;
                Debug.Log($"[MeleeEnemy] Inimigo {gameObject.name} assinado ao evento OnDeath do Player.");
            }
            else
            {
                Debug.LogError("[MeleeEnemy] Componente Health não encontrado no Player! Certifique-se de que o Player tem o script Health.");
            }
        }
        else
        {
            Debug.LogError("[MeleeEnemy] Objeto com a tag 'Player' não encontrado na cena! Certifique-se de que o Player tem a tag 'Player'.");
        }
    }

    private void OnDestroy()
    {
        // Garante que o inimigo se desinscreva do evento quando ele é destruído
        if (playerHealthInstance != null)
        {
            playerHealthInstance.OnDeath -= OnPlayerDeath;
            Debug.Log($"[MeleeEnemy] Inimigo {gameObject.name} desinscrito do evento OnDeath do Player.");
        }
    }

    // Este método é chamado quando o evento OnDeath do jogador é disparado
    private void OnPlayerDeath()
    {
        playerIsDead = true; // Atualiza o estado quando o jogador morre
        Debug.Log($"[MeleeEnemy] Notificado: Player morreu! Inimigo {gameObject.name} vai parar de atacar.");

        // Opcional: Você pode querer forçar o inimigo a parar a animação de ataque imediatamente
        // Se a sua animação de ataque é controlada por um bool, defina-o como false:
        // anim.SetBool("isAttacking", false);
        // Ou volte para uma animação de idle:
        // anim.SetTrigger("idle");

        // Se houver um script de patrulha, reative-o para o inimigo voltar a patrulhar
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = true;
            anim.SetBool("moving", true); // Certifica que a animação de movimento está ativa
        }
        else
        {
             // Se não tiver patrulha, certifique-se de que o inimigo pare qualquer movimento
             // Isso pode ser desativando o Rigidbody2D ou zerando a velocidade
             Rigidbody2D rb = GetComponent<Rigidbody2D>();
             if (rb != null)
             {
                 rb.linearVelocity = Vector2.zero; // <-- AQUI ESTÁ A MUDANÇA!
             }
        }
    }

    private void Update()
    {
        // Se o jogador está morto, o inimigo não faz mais nada relacionado a ataque ou perseguição
        if (playerIsDead)
        {
            return; // Sai do Update para que a lógica de ataque não continue
        }

        cooldownTimer += Time.deltaTime;

        // Se o jogador estiver à vista e o cooldown de ataque terminou
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack"); // Aciona a animação de ataque
                Debug.Log($"[MeleeEnemy] Inimigo {gameObject.name} acionou ataque!");
            }
        }

        // Se houver um script de patrulha, ele é desabilitado/habilitado dependendo se o player está à vista
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    // Verifica se o jogador está dentro do alcance de ataque do inimigo
    private bool PlayerInSight()
    {
        // Se o jogador está morto, ele não está "à vista" para propósitos de ataque
        if (playerIsDead) return false;

        // Realiza um BoxCast para detectar o jogador
        RaycastHit2D hit =
            Physics2D.BoxCast(
                boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
                0, Vector2.left, 0, playerLayer); // Nota: 'Vector2.left' aqui pode precisar ser ajustado se o inimigo não estiver sempre virado para a esquerda

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            playerHealth = hit.transform.GetComponent<Health>(); // Obtém a referência ao Health do jogador
            return true;
        }

        playerHealth = null; // Reseta a referência se o player não estiver mais à vista
        return false;
    }

    // Desenha o Gizmo do BoxCast para visualização no Editor
    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    // ESSA FUNÇÃO SÓ DEVE SER CHAMADA VIA ANIMATION EVENT NO FRAME CORRETO DO ATAQUE DO INIMIGO (IMPACTO)
    private void DamagePlayer()
    {
        // Verifica se o jogador não está morto antes de tentar causar dano
        if (playerIsDead)
        {
            Debug.Log($"[MeleeEnemy] Inimigo {gameObject.name} tentou DamagePlayer, mas Player está morto.");
            return;
        }

        if (PlayerInSight() && playerHealth != null)
        {
            playerHealth.TakeDamage(damage); // Causa dano ao jogador
            Debug.Log($"[MeleeEnemy] Inimigo {gameObject.name} causou {damage} de dano ao Player! Vida restante: {playerHealth.currentHealth}");
        }
        else
        {
            Debug.Log($"[MeleeEnemy] Inimigo {gameObject.name} tentou DamagePlayer, mas Player não está à vista ou playerHealth é nulo.");
        }
    }
}