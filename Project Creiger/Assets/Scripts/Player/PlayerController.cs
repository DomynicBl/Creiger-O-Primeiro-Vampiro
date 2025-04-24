using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    // Variaveis privadas
    private Rigidbody2D rb; //Colisor do Player
    private Animator anim; //Animações do Player
    private float moveX; //Movimento do jogador no eixo X

    // Vareis publicas, podem ser editaveis na Unity
    public float speed = 8; //Velocidade do player
    public float jumpForce = 8; //Força do pulo do player
    public int addJump; //Numero de pulos  permitidos ao player
    public bool isGrounded = false; //Verifica se o player esta no chao

    // Funções da Unity
    void Start(){ // Roda uma vez quando quando carrega a cena
        rb = GetComponent<Rigidbody2D>(); // Pega o componente Rigidbody2D do Player
        anim = GetComponent<Animator>(); // Pega o componente Animator do Player
    } 

    void Update(){ // Roda em todo frame
        moveX = Input.GetAxisRaw("Horizontal");
        
        if (Input.GetButtonDown("Jump") && isGrounded ){ // Verifica se o player apertou o botão de pulo estando no chao
            addJump = 1; // Adiciona um pulo ao player
            Jump(); // Chama a função de pulo
        }else if(Input.GetButtonDown("Jump") && addJump>0){ // Verifica se o player apertou o botão de pulo e não estando no chao
            addJump--; // Remove um pulo do player
            Jump(); // Chama a função de pulo
        }
    }

    void FixedUpdate(){ // Roda em todo frame fixo
        Move();
    }

    // Funções do Player 
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

}
