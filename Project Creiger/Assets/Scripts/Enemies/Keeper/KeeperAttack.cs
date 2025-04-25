using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperAttack : MonoBehaviour{ // Script que controla o ataque do Keeper
    private void OnTriggerStay2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            collision.GetComponent<PlayerController>().life--; // Diminui a vida do player
        }
    }
}

