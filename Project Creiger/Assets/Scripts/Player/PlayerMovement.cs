using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float sprintMultiplier = 2f;

    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private int jumpCount;
    private int maxJumps = 2;

    private Stamina stamina;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stamina = GetComponent<Stamina>();
        if (stamina == null)
            Debug.LogError("Script Stamina não encontrado no jogador!");
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float finalSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift) && stamina.currentStamina > 0)
        {
            finalSpeed *= sprintMultiplier;
            stamina.ConsumeStamina(Time.deltaTime);
        }

        body.linearVelocity = new Vector2(horizontalInput * finalSpeed, body.linearVelocity.y);

        // Flip player
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(4, 4, 1);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-4, 4, 1);

        // Pulo ou pulo duplo
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            Jump();
        }

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);
    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        jumpCount++;
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            jumpCount = 0; // Reseta os pulos ao tocar o chão
        }
    }
}
