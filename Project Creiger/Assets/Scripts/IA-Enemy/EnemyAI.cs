using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3f;
    public float jumpForce = 12f;
    public float groundCheckDistance = 1f;
    public float detectionRange = 8f;
    public float chaseRange = 4f;
    public float stoppingDistance = 0.5f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform player;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isChasing;
    private bool facingRight = false; // Começa olhando para a esquerda
    private float lastKnownPlayerDirection = -1f; // Começa andando para esquerda
    private bool hasSeenPlayer = false;

    private float jumpCooldown = 0.5f;  // tempo mínimo entre pulos
    private float lastJumpTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        UpdateFacingDirection(lastKnownPlayerDirection);
    }

    void Update()
    {
        DetectGround();

        // Debug: posição do inimigo e groundCheck
        Debug.Log($"Inimigo posição: {transform.position}, GroundCheck posição: {groundCheck.position}");

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            if (!hasSeenPlayer)
            {
                hasSeenPlayer = true;
                isChasing = true;

                lastKnownPlayerDirection = Mathf.Sign(player.position.x - transform.position.x);
                UpdateFacingDirection(lastKnownPlayerDirection);
                Debug.Log("Jogador detectado. Perseguindo na direção: " + lastKnownPlayerDirection);
            }
        }
        else if (distanceToPlayer > detectionRange)
        {
            if (hasSeenPlayer)
                Debug.Log("Jogador perdido. Voltando a patrulhar.");

            isChasing = false;
            hasSeenPlayer = false;

            lastKnownPlayerDirection = -1f;
            UpdateFacingDirection(lastKnownPlayerDirection);
        }

        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        // Debug: velocidade e direção
        Debug.Log($"Direção atual: {lastKnownPlayerDirection}, Velocidade X: {rb.linearVelocity.x}");
    }

    void DetectGround()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    bool IsGroundAhead()
    {
        Vector2 origin = (Vector2)(groundCheck.position + Vector3.right * lastKnownPlayerDirection * 0.5f);
        float rayLength = groundCheckDistance + 0.2f;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
        Debug.DrawRay(origin, Vector2.down * rayLength, hit.collider != null ? Color.green : Color.red);
        return hit.collider != null;
    }

    bool IsPlatformWithinJump()
    {
        Vector2 origin = (Vector2)(groundCheck.position + Vector3.right * lastKnownPlayerDirection * 0.8f + Vector3.up * 1f);
        float rayLength = 1.5f;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
        Debug.DrawRay(origin, Vector2.down * rayLength, hit.collider != null ? Color.cyan : Color.clear);
        return hit.collider != null;
    }

    bool IsObstacleAhead()
    {
        Vector2 origin = (Vector2)(groundCheck.position + Vector3.up * 0.1f);
        Vector2 direction = Vector2.right * lastKnownPlayerDirection;
        float distance = 0.5f;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);
        Debug.DrawRay(origin, direction * distance, hit.collider != null ? Color.yellow : Color.clear);
        return hit.collider != null;
    }

    void Patrol()
    {
        MoveInDirection(lastKnownPlayerDirection);

        if (!IsGroundAhead() && IsPlatformWithinJump())
        {
            if (isGrounded && Time.time - lastJumpTime > jumpCooldown)
            {
                Jump();
                lastJumpTime = Time.time;
            }
        }
        else if (IsObstacleAhead())
        {
            if (isGrounded && Time.time - lastJumpTime > jumpCooldown)
            {
                Jump();
                lastJumpTime = Time.time;
            }
        }
        else if (!IsGroundAhead())
        {
            FlipDirection();
            Debug.Log("Fim da plataforma, inimigo virou.");
        }
    }

    void ChasePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        lastKnownPlayerDirection = Mathf.Sign(player.position.x - transform.position.x);
        UpdateFacingDirection(lastKnownPlayerDirection);

        if (distanceToPlayer > stoppingDistance)
        {
            MoveInDirection(lastKnownPlayerDirection);

            if ((!IsGroundAhead() && IsPlatformWithinJump()) || IsObstacleAhead())
            {
                if (isGrounded && Time.time - lastJumpTime > jumpCooldown)
                {
                    Jump();
                    lastJumpTime = Time.time;
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            Debug.Log("Inimigo perto do jogador, parando.");
        }
    }

    void MoveInDirection(float direction)
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction * speed;
        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            Debug.Log("Inimigo pulou");
        }
    }

    void FlipDirection()
    {
        lastKnownPlayerDirection *= -1f;
        UpdateFacingDirection(lastKnownPlayerDirection);
    }

    void UpdateFacingDirection(float direction)
    {
        bool shouldFaceRight = direction > 0;
        if (facingRight != shouldFaceRight)
        {
            facingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
            transform.localScale = scale;
        }
    }
}
