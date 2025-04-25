using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperController : MonoBehaviour{

    // Variaveis privadas
    private CapsuleCollider2D colliderKeeper; //Colisor do Keeper
    private Animator anim; //Animações do Player
    private bool goRight = true; //Verifica se o Keeper esta indo para a direita

    // Variaveis publicas, podem ser editaveis na Unity
    public int life; //Vida do Keeper
    public float speed = 2; //Velocidade do Keeper

    public Transform a;
    public Transform b;

    public GameObject range; // GameObject representing the range of the Keeper

    // Funções da Unity 
    void Start(){ // Roda uma vez quando quando carrega a cena
        colliderKeeper = GetComponent<CapsuleCollider2D>(); // Pega o componente CapsuleCollider2D do Keeper
        anim = GetComponent<Animator>(); // Pega o componente Animator do Keeper
    } 

    void Update(){ // Roda em todo frame

        if(life <= 0){ // Verifica se a vida do Keepler é menor ou igual a 0
            this.enabled = false; // Desativa o script do Keepler para não se movimentar mais
            colliderKeeper.enabled = false; // Desativa o colisor do Keepler para não colidir mais
            range.SetActive(false); // Desativa o range do Keepler para não atacar mais
            anim.Play("Animation - Dead", -1); // Ativa a animação de morte

        }

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
            return; // Se o Keeper estiver atacando, não faz nada
        }

        if(goRight == true){ // Se o Keeper estiver indo para a direita
            if(Vector2.Distance(transform.position, b.position) < 0.5f){ // Se o Keeper estiver perto do ponto b
                goRight = false; // Muda a direção do Keeper para a esquerda
            }

            transform.eulerAngles = new Vector3(0f, 0f, 0f); // Rotaciona o Keeper para a direita
            transform.position = Vector2.MoveTowards(transform.position, b.position, speed * Time.deltaTime); // Move o Keeper para a direita

        }else{ // Se o Keeper estiver indo para a esquerda
            if(Vector2.Distance(transform.position, a.position) < 0.5f){ // Se o Keeper estiver perto do ponto a
                goRight = true; // Muda a direção do Keeper para a direita
            }

            transform.eulerAngles = new Vector3(0f, 180f, 0f); // Rotaciona o Keeper para a esquerda
            transform.position = Vector2.MoveTowards(transform.position, a.position, speed * Time.deltaTime); // Move o Keeper para a esquerda
        }
    }
}
