using UnityEngine;

public class IAFSMEnemy : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Parâmetros de Movimento")]
    public float moveSpeed = 3f;
    public float jumpForce = 5f;
    public float detectRadius = 8f;
    public LayerMask groundLayer;

    private bool isGrounded;
    private bool isFacingRight = true;

    private enum State { Idle, Chase, JumpObstacle, JumpGap }
    private State currentState = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Garante que comece virado para a direita
        if (!isFacingRight)
            Flip();
    }

    void Update()
    {
        CheckGround();
        UpdateState();

        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
            case State.Chase:
                ChasePlayer();
                break;
            case State.JumpObstacle:
            case State.JumpGap:
                Jump();
                break;
        }

        FlipIfNeeded();
        UpdateAnimation();
    }

    void UpdateState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectRadius)
        {
            currentState = State.Chase;

            if (DetectObstacle())
                currentState = State.JumpObstacle;

            if (DetectGap())
                currentState = State.JumpGap;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
    }

    bool DetectObstacle()
    {
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(transform.position, dir, 1f, groundLayer);
    }

    bool DetectGap()
    {
        Vector2 frontCheck = transform.position + (isFacingRight ? Vector3.right : Vector3.left) * 0.5f;
        return !Physics2D.Raycast(frontCheck, Vector2.down, 1.5f, groundLayer);
    }

    void FlipIfNeeded()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !isFacingRight)
            Flip();
        else if (player.position.x < transform.position.x && isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
            animator.SetBool("isWalking", isWalking);
        }
    }
}
