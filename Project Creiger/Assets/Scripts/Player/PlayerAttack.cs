using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour{ // Script que controla o ataque do Keeper
    private void OnTriggerStay2D(Collider2D collision){
        if(collision.CompareTag("Keeper")){
            collision.GetComponent<KeeperController>().life--; // Diminui a vida do Keeper
        }
    }
}

