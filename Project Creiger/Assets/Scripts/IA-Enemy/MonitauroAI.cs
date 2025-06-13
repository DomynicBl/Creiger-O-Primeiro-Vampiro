using UnityEngine;

public class MinotauroAI : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Parâmetros de Combate")]
    public float speed = 2.5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    private float nextAttackTime = 0f;
    private Vector3 escalaInicial;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        escalaInicial = transform.localScale;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Corrigido para rb.velocity
            anim.SetBool("IsWalking", false);
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

        // Atualiza a animação de caminhada com base na velocidade atual
        anim.SetBool("moving", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    }

    void VirarParaOJogador()
    {
        float direcaoParaOJogador = player.position.x - transform.position.x;
        if (direcaoParaOJogador != 0)
        {
            float ladoCorreto = Mathf.Sign(direcaoParaOJogador);
            transform.localScale = new Vector3(-Mathf.Abs(escalaInicial.x) * ladoCorreto, escalaInicial.y, escalaInicial.z);
        }
    }

    void Perseguir()
    {
        float moveDirection = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(moveDirection * speed, rb.linearVelocity.y);
        // Removido anim.SetBool daqui para controle centralizado no Update()
    }

    void Atacar()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        // Animação de andar será automaticamente desativada no Update() com velocidade 0

        if (Time.time >= nextAttackTime)
        {
            anim.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}
