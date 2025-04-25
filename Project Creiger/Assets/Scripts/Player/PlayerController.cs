using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Importa o namespace para usar o SceneManager
using UnityEngine; 
using TMPro; // Importa o namespace para usar o TextMeshPro

public class PlayerController : MonoBehaviour{
    // Variaveis privadas
    private Rigidbody2D rb; //Colisor do Player
    private CapsuleCollider2D colliderPlayer; //Colisor do Player
    private Animator anim; //Animações do Player
    private float moveX; //Movimento do jogador no eixo X
    private float lastAttackTime = -Mathf.Infinity; // Quando foi o último ataque

    // Vareis publicas, podem ser editaveis na Unity
    public bool isGrounded = false; //Verifica se o player esta no chao
    public float speed = 10; //Velocidade do player
    public float jumpForce = 11; //Força do pulo do player
    public int addJump; //Numero de pulos  permitidos ao player
    public float attackCooldown = 0.5f; //Cooldown do ataque do player
    public int life; //Vida do player
    public int maxLife = 5; //Vida maxima do player
    public TextMeshProUGUI textLife; //Texto que mostra a vida do player

    // Funções da Unity
    void Start(){ // Roda uma vez quando quando carrega a cena
        rb = GetComponent<Rigidbody2D>(); // Pega o componente Rigidbody2D do Player
        anim = GetComponent<Animator>(); // Pega o componente Animator do Player
        colliderPlayer = GetComponent<CapsuleCollider2D>(); // Pega o componente CapsuleCollider2D do Player
    } 

    void Update(){ // Roda em todo frame
        moveX = Input.GetAxisRaw("Horizontal");
        
        if (Input.GetButtonDown("Jump") && isGrounded){ // Verifica se o player apertou o botão de pulo estando no chao
            addJump = 1; // Adiciona um pulo ao player
            //jumpForce = 12; // Aumenta a força do pulo
            Jump(); // Chama a função de pulo
            jumpForce = 11; // Reseta a força do pulo
        }else if(Input.GetButtonDown("Jump") && addJump>0){ // Verifica se o player apertou o botão de pulo e não estando no chao
            addJump--; // Remove um pulo do player
            Jump(); // Chama a função de pulo
        }

        Attack(); // Chama a função de ataque
        textLife.text = life.ToString(); // Atualiza o texto da vida do player

        if(life <= 0){ // Verifica se a vida do player é menor ou igual a 0
            this.enabled = false; // Desativa o script do player para não se movimentar mais
            colliderPlayer.enabled = false; // Desativa o colisor do player para não colidir mais
            rb.gravityScale = 0; // Desativa a gravidade do player para não do mapa
            anim.Play("Animation - Die", -1); // Ativa a animação de morte
            StartCoroutine(DeathDelay()); // Inicia a rotina de atraso para a morte
        }
    
        IEnumerator DeathDelay() { // Método para atraso antes de carregar a cena de Game Over
            yield return new WaitForSeconds(2f); // Aguarda 2 segundos
            SceneManager.LoadScene("Demo - Sprint 2"); // Carrega a cena de Game Over
        }
    }

    void FixedUpdate(){ // Roda em todo frame fixo
        Move(); // Chama a função de movimento
    }

    // Funções do Player 
    void OnCollisionEnter2D(Collision2D collision){ // Função que verifica se o player colidiu com o chão
        if (collision.gameObject.CompareTag("Ground")){ // Verifica se o objeto colidido tem a tag "Ground"
            isGrounded = true; // O player esta no chao
            anim.SetBool("IsJump", false); // Desativa a animação de pulo
        }
    }

    void OnCollisionExit2D(Collision2D collision){ // Função que verifica se o player saiu da colisão com o chão
        if (collision.gameObject.CompareTag("Ground")){ // Verifica se o objeto colidido tem a tag "Ground"
            isGrounded = false; // O player não esta no chao
        }
    }

    void Move(){ // Função de movimento do Player
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
            
        if(moveX == 0){ // Verifica se o player não esta se movendo
            anim.SetBool("IsRun", false); // Ativa a animação de correr
        }else if(moveX < 0){ // Verifica se o player esta se movendo para a esquerda
            transform.eulerAngles = new Vector3(0f, 180f, 0f); // Inverte o sprite do player para a esquerda 
            anim.SetBool("IsRun", true); // Ativa a animação de correr  
        }else if(moveX > 0){ // Verifica se o player esta se movendo para a direita
            transform.eulerAngles = new Vector3(0f, 0f, 0f); // Inverte o sprite do player para a direita
            anim.SetBool("IsRun", true); // Ativa a animação de correr
        }
    }

    void Jump(){ // Função de pulo do Player 
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetBool("IsJump", true); // Ativa a animação de pulo 
    }

    void Attack() { // Função de ataque do Player
        if (Input.GetButtonDown("Fire1") && Time.time >= lastAttackTime + attackCooldown){ // Verifica se o player apertou o botão de ataque e se o cooldown do ataque acabou
            anim.Play("Animation - Attack", -1); // Ativa a animação
            lastAttackTime = Time.time; // Atualiza o tempo do último ataque
        }
    }
}
