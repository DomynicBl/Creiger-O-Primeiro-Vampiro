using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour{ // Classe que controla a vida do player
    // Funções da Unity
    void Start(){ // Roda uma vez quando quando carrega a cena

    } 

    void Update(){ // Roda em todo frame
        
    }
    
    private void OnTriggerEnter2D(Collider2D collision){ // Função que verifica se o player colidiu com o coração
        if(collision.gameObject.tag == "Player"){ // Verifica se o objeto colidido tem o player
            collision.GetComponent<PlayerController>().life++; // Adiciona 1 vida ao player
            Destroy(this.gameObject); // Destroi o objeto do coração
        }
    }
}
