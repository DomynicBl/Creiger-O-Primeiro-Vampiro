using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    [Header("Referências")]
    public Transform player;
    public Transform leftPatrolPoint;
    public Transform rightPatrolPoint;

    [Header("Parâmetros de Movimento")]
    public float speed = 2f;
    public float jumpForce = 5f;
    public float detectionRange = 10f;
    public LayerMask groundMask;

    private Rigidbody2D rb;
    private AStarPathfinder pathfinder;
    private GridManager gridManager;
    private List<Node> currentPath;
    private int pathIndex = 0;
    private bool goingRight = true;
    private Vector3 initialScale;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {
            Debug.LogError("Rigidbody2D não encontrado!");
        }

        pathfinder = FindObjectOfType<AStarPathfinder>();
        gridManager = FindObjectOfType<GridManager>();

        if (leftPatrolPoint == null || rightPatrolPoint == null) {
            Debug.LogWarning("Patrol points não atribuídos — inimigo não vai patrulhar.");
        }

        initialScale = transform.localScale;
    }

    void Update() {
        // Verifica se o jogador está dentro do alcance
        if (Vector2.Distance(transform.position, player.position) < detectionRange) {
            currentPath = pathfinder.FindPath(transform.position, player.position);
            pathIndex = 0;
        }

        if (currentPath != null && pathIndex < currentPath.Count) {
            Vector2 target = currentPath[pathIndex].worldPosition;
            MoveTowards(target);

            if (Vector2.Distance(transform.position, target) < 0.2f)
                pathIndex++;
        }
        else {
            Patrol(); // Ativa patrulha se não houver caminho
        }
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        // Aplica movimento horizontal mantendo a velocidade vertical
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        // Corrige direção visual do sprite
        if (direction.x != 0){
            Vector3 scale = initialScale;
            scale.x *= -Mathf.Sign(direction.x); // Corrige a inversão
            transform.localScale = scale;
        }

        // Pula se houver obstáculo e estiver no chão
        if (ObstacleInFront() && IsGrounded()){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

    }


    void Patrol() {
        if (leftPatrolPoint == null || rightPatrolPoint == null) return;

        Vector2 target = goingRight ? rightPatrolPoint.position : leftPatrolPoint.position;
        MoveTowards(target);

        if (Vector2.Distance(transform.position, target) < 0.2f) {
            goingRight = !goingRight;
        }
    }

    bool ObstacleInFront() {
        Vector2 direction = new Vector2(Mathf.Sign(rb.linearVelocity.x), 0);
        Vector2 origin = transform.position + Vector3.up * 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1f, groundMask);
        Debug.DrawRay(origin, direction * 1f, Color.green);

        return hit.collider != null;
    }

    bool IsGrounded() {
        Vector2 origin = transform.position + Vector3.down * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.2f, groundMask);
        Debug.DrawRay(origin, Vector2.down * 0.2f, Color.red);

        return hit.collider != null;
    }
}
