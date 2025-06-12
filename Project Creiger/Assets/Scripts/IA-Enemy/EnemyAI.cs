using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // --- MUDANÇA: Introdução de uma máquina de estados para controlar o comportamento ---
    private enum AIState { Patrol, Chase, Attack }
    private AIState currentState;

    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Enemy")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [Header("Movement Parameters")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float detectionRadius = 8f;

    // --- MUDANÇA: Novos parâmetros para o estado de ataque ---
    [Header("Attack Parameters")]
    [SerializeField] private float attackRange = 1.5f; // Distância para começar a atacar
    [SerializeField] private float attackCooldown = 2f; // Tempo entre ataques
    private float attackTimer = 0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    private Vector3 initScale;
    
    // Patrol
    private bool movingLeft = true;
    private float idleTimer = 0f;
    [SerializeField] private float idleDuration = 2f;

    // Pathfinding
    private List<Vector2> path = new List<Vector2>();
    private int currentPathIndex = 0;
    private AStarPathfinding pathfinder;
    private float pathUpdateTimer = 0f;
    private float pathUpdateCooldown = 0.5f;
    private float jumpNodeHeightThreshold = 0.5f;

    private void Awake()
    {
        initScale = transform.localScale;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        
        // --- MUDANÇA: Inicia no estado de patrulha ---
        currentState = AIState.Patrol;
    }

    private void Start()
    {
        pathfinder = FindObjectOfType<AStarPathfinding>();
        if (pathfinder == null)
            Debug.LogError("AStarPathfinding script not found in the scene! EnemyAI will not work correctly.");
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (player == null) return;

        // Decrementa o timer de ataque
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // --- MUDANÇA: Lógica principal agora é uma máquina de estados ---
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        UpdateState(distanceToPlayer);

        switch (currentState)
        {
            case AIState.Patrol:
                HandlePatrolState();
                break;
            case AIState.Chase:
                HandleChaseState();
                break;
            case AIState.Attack:
                HandleAttackState(distanceToPlayer);
                break;
        }

        UpdateAnimations();
    }
    
    // --- MUDANÇA: Função que decide o estado atual da IA ---
    private void UpdateState(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attack;
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            currentState = AIState.Chase;
        }
        else
        {
            currentState = AIState.Patrol;
        }
    }

    // --- MUDANÇA: Lógicas de comportamento separadas por estado ---
    private void HandlePatrolState()
    {
        // Limpa o caminho se estava perseguindo antes
        if (path.Count > 0)
        {
            path.Clear();
            currentPathIndex = 0;
        }
        
        Patrol();
    }

    private void HandleChaseState()
    {
        ChasePlayer();
    }
    
    private void HandleAttackState(float distanceToPlayer)
    {
        // --- MUDANÇA: Lógica de Ataque ---
        // 1. Para de se mover
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // 2. Vira para o jogador (A CORREÇÃO PRINCIPAL)
        FacePlayer();
        
        // 3. Ataca se o cooldown terminou
        if (attackTimer <= 0)
        {
            Attack();
        }
    }
    
    // --- MUDANÇA: Nova função para virar na direção do jogador ---
    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            FaceDirection(1); // Jogador à direita
        }
        else if (player.position.x < transform.position.x)
        {
            FaceDirection(-1); // Jogador à esquerda
        }
    }

    private void Attack()
    {
        // Aciona a animação de ataque e reseta o cooldown
        anim.SetTrigger("attack"); // Você precisará criar este trigger no Animator
        attackTimer = attackCooldown;
        Debug.Log("Inimigo atacou!");
        // Aqui você pode adicionar a lógica de dano (ex: criar uma hitbox de ataque)
    }

    private void Patrol()
    {
        if (idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (idleTimer <= 0)
            {
                movingLeft = !movingLeft;
            }
            return;
        }

        int direction = movingLeft ? -1 : 1;
        
        if ((movingLeft && transform.position.x <= leftEdge.position.x) ||
            (!movingLeft && transform.position.x >= rightEdge.position.x) ||
            IsAtEdge(new Vector2(direction, 0)))
        {
            idleTimer = idleDuration;
            FaceDirection(direction * -1);
        }
        else
        {
            MoveInDirection(direction);
        }
    }

    private bool IsAtEdge(Vector2 direction)
    {
        Vector2 checkPos = (Vector2)groundCheck.position + direction.normalized * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 1f, groundLayer);
        return hit.collider == null;
    }

    private void MoveInDirection(int dir)
    {
        FaceDirection(dir);
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        if (ShouldJumpOverObstacle(dir))
            Jump();
    }

    private void FaceDirection(int dir)
    {
        if (dir == 0) return;
        transform.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
        movingLeft = dir == -1;
    }

    private bool ShouldJumpOverObstacle(int direction)
    {
        if (!isGrounded) return false;
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, Vector2.right * direction, 0.6f, groundLayer);
        return wallHit.collider != null;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");
        }
    }
    
    private void ChasePlayer()
    {
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateCooldown)
        {
            pathUpdateTimer = 0f;
            UpdatePath();
        }
    
        if (path == null || path.Count == 0 || currentPathIndex >= path.Count)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
    
        Vector2 targetNodePosition = path[currentPathIndex];
        float xDifference = targetNodePosition.x - transform.position.x;
        int moveDir = (xDifference > 0.1f) ? 1 : (xDifference < -0.1f) ? -1 : 0;
    
        FaceDirection(moveDir);
    
        if (isGrounded && targetNodePosition.y > transform.position.y + jumpNodeHeightThreshold)
        {
            Jump();
        }
    
        if (moveDir != 0 && !IsAtEdge(new Vector2(moveDir, 0)))
        {
            rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    
        if (Vector2.Distance(transform.position, targetNodePosition) < 0.5f)
        {
            currentPathIndex++;
        }
    }

    private void UpdatePath()
    {
        if (pathfinder == null || player == null) return;
    
        List<Vector3> newPath3D = pathfinder.FindPath(transform.position, player.position);
    
        if (newPath3D != null && newPath3D.Count > 0)
        {
            path.Clear();
            foreach (Vector3 point in newPath3D) path.Add(point);
            currentPathIndex = 0;
        }
        else
        {
            path.Clear();
            currentPathIndex = 0;
        }
    }

    private void UpdateAnimations()
    {
        // A animação de movimento só deve ser verdadeira se não estivermos no estado de ataque
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f && currentState != AIState.Attack;
        anim.SetBool("moving", isMoving);
        anim.SetBool("grounded", isGrounded);
    }
}