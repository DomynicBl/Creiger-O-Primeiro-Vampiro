using UnityEngine;

public class Movement : MonoBehaviour
{

    [SerializeField] private float speed;
    private Animator anim;
    private Rigidbody2D body;

    private void Awake(){
        body= GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update(){

        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput * speed , body.linearVelocity.y);

        // Flipa o personagem para a esquerda e direita
        if(horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if(horizontalInput < -0.01f){
            transform.localScale = new Vector3(-1,1,1);
        }


        if(Input.GetKey(KeyCode.Space)){
            body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
        }
        anim.SetBool("Run", horizontalInput != 0 );
    }
    
}
