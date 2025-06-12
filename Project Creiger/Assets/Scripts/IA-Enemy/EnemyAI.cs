using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
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

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    private Vector3 initScale;
    private bool isChasing = false;

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

    private void Awake()
    {
        initScale = transform.localScale;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        pathfinder = FindObjectOfType<AStarPathfinding>();
        if (pathfinder == null)
            Debug.LogWarning("AStarPathfinding script not found in the scene!");
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            if (!isChasing)
            {
                isChasing = true;
                UpdatePath();
            }
            ChasePlayer();
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                path.Clear();
                currentPathIndex = 0;
            }
            Patrol();
        }

        UpdateAnimations();
    }

    private void Patrol()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftEdge.position.x && !IsAtEdge(Vector2.left))
                MoveInDirection(-1);
            else
                ChangeDirectionWithIdle();
        }
        else
        {
            if (transform.position.x < rightEdge.position.x && !IsAtEdge(Vector2.right))
                MoveInDirection(1);
            else
                ChangeDirectionWithIdle();
        }
    }

    private bool IsAtEdge(Vector2 direction)
    {
        Vector2 checkPos = (Vector2)groundCheck.position + direction * 0.3f;
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 0.5f, groundLayer);
        return hit.collider == null;
    }

    private void ChangeDirectionWithIdle()
    {
        idleTimer += Time.deltaTime;
        anim.SetBool("moving", false);

        if (idleTimer >= idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0f;
        }
    }

    private void MoveInDirection(int dir)
    {
        idleTimer = 0f;
        anim.SetBool("moving", true);
        FaceDirection(dir);
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        if (ShouldJump(dir))
            Jump();
    }

    private void FaceDirection(int dir)
    {
        transform.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
        movingLeft = dir == -1;
    }

    private bool ShouldJump(int direction)
    {
        if (!isGrounded) return false;

        Vector2 origin = (Vector2)groundCheck.position + Vector2.right * direction * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, 0.6f, groundLayer);

        return hit.collider != null;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetTrigger("jump");
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
            // Stop enemy if no path
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("moving", false);
            return;
        }

        Vector2 targetPos = path[currentPathIndex];
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos);
        direction.Normalize();

        int moveDir = direction.x > 0 ? 1 : -1;
        FaceDirection(moveDir);

        if (IsPathClear(targetPos))
        {
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (Vector2.Distance(currentPos, targetPos) < 0.2f)
            currentPathIndex++;

        if (ShouldJump(moveDir))
            Jump();

        anim.SetBool("moving", true);
    }

    private void UpdatePath()
    {
        if (pathfinder == null) return;

        List<Vector3> newPath = pathfinder.FindPath(transform.position, player.position);

        if (newPath == null || newPath.Count == 0)
        {
            path.Clear();
            currentPathIndex = 0;
            return;
        }

        path.Clear();
        foreach (Vector3 point in newPath)
            path.Add(point);

        currentPathIndex = 0;
    }

    private bool IsPathClear(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, groundLayer);
        return hit.collider == null;
    }

    private void UpdateAnimations()
    {
        anim.SetBool("moving", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        // Linha removida:
        // anim.SetBool("grounded", isGrounded);
    }
}
