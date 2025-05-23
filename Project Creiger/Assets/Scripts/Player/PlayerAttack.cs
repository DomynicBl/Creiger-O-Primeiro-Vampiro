using UnityEngine;
using System; // Necessário para Action

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown = 0.6f; // Tempo entre ataques
    [SerializeField] private float range = 1.0f;          // Alcance do ataque
    [SerializeField] private int damage = 1;              // Dano causado pelo ataque

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance = 0.5f; // Distância do collider de ataque
    [SerializeField] private BoxCollider2D boxCollider;     // O BoxCollider2D usado para o ataque

    [Header("Enemy Layer")]
    [SerializeField] private LayerMask enemyLayer; // A layer dos inimigos

    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;

    private KillStreakManager _killStreakManager; // Referência ao KillStreakManager

    private void Awake()
    {
        anim = GetComponent<Animator>();
        // Encontra o KillStreakManager na cena, pois não é Singleton persistente neste momento.
        _killStreakManager = UnityEngine.Object.FindAnyObjectByType<KillStreakManager>();
        if (_killStreakManager == null)
        {
            Debug.LogError("[PlayerMeleeAttack] KillStreakManager not found in the scene! Make sure it's present and active.", this);
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Se o botão esquerdo do mouse é pressionado e o cooldown terminou
        if (Input.GetKeyDown(KeyCode.Mouse0) && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0; // Reseta o cooldown

            // Escolhe um índice de ataque aleatório para diferentes animações de ataque
            int randomIndex = UnityEngine.Random.Range(0, 3); // Random.Range(int, int) é exclusivo no segundo parâmetro (0, 1, 2)
            anim.SetInteger("AttackIndex", randomIndex);
            anim.SetTrigger("attack"); // Aciona o trigger de ataque no Animator
        }
    }

    // ESSA FUNÇÃO DEVE SER CHAMADA VIA ANIMATION EVENT NO FRAME CORRETO DO ATAQUE (IMPACTO)
    private void DamageEnemy()
    {
        // Realiza um BoxCast para detectar inimigos dentro do alcance do ataque
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.zero, 0, enemyLayer);

        if (hit.collider != null)
        {
            Health enemyHealth = hit.transform.GetComponent<Health>();
            if (enemyHealth != null)
            {
                // Se o inimigo for morrer com este ataque, inscreva-se no evento OnDeath dele
                // Isso garante que o KillStreakManager só conte a kill QUANDO o inimigo realmente morrer.
                // Desinscreve-se primeiro para evitar assinaturas duplicadas
                enemyHealth.OnDeath -= HandleEnemyDeath;
                enemyHealth.OnDeath += HandleEnemyDeath; // Inscreve
                Debug.Log($"[PlayerMeleeAttack] Player atacou e inimigo {enemyHealth.name} será morto. Inscrevendo no OnDeath.");

                enemyHealth.TakeDamage(damage); // Aplica o dano ao inimigo
            }
        }
    }

    // Método que será chamado quando o evento OnDeath de um inimigo for disparado
    private void HandleEnemyDeath()
    {
        if (_killStreakManager != null)
        {
            _killStreakManager.AddKill(); // Adiciona uma kill ao contador
            Debug.Log("[PlayerMeleeAttack] Inimigo morreu! Adicionando kill ao KillStreakManager.");
        }
        // Para ser totalmente robusto, aqui você deveria desinscrever:
        // ((Health)sender).OnDeath -= HandleEnemyDeath; // Se pudéssemos obter o sender
    }

    // Desenha o Gizmo do BoxCast para visualização no Editor
    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z)
        );
    }
}