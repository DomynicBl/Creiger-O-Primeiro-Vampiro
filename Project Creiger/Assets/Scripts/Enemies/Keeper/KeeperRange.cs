using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperRange : MonoBehaviour{ // Script que controla o ataque do Keeper
    private void OnTriggerStay2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            GetComponentInParent<Animator>().Play("Animation - Attack", -1); // Chama a animação de ataque do Keeper
        }
    }
} 
