using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Movimento : MonoBehaviour{
    private float horizontalInput;
    
    private Rigidbody2D rb;
    [SerializeField] private int velocidade = 100;
    [SerializeField] private Transform PlayerBase;
    [SerializeField] private LayerMask GroundLayer; 
    private bool estaNoChao;
    //private Animator animator;
    //private SpriteRenderer spriteRenderer;
    //private int movendoHash = Animator.StringToHash("movendo");
    //private int saltandoHash = Animator.StringToHash("saltando");
    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update(){
        horizontalInput = Input.GetAxis("Horizontal");
        
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao){
            rb.AddForce(Vector2.up * 800);
        }

        estaNoChao = Physics2D.OverlapCircle(PlayerBase.position, 0.2f, GroundLayer);
        //animator.SetBool(movendoHash, horizontalInput != 0);
        //animator.SetBool(saltandoHash, !estaNoChao);
        /*if(horizontalInput > 0){
            spriteRenderer.flipX = false;
        }else if(horizontalInput < 0){
            spriteRenderer.flipX = true;
        }*/
    }
    private void FixedUpdate(){
        rb.linearVelocity = new Vector2(horizontalInput * velocidade, rb.linearVelocity.y);
    }
}