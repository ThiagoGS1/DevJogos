using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ativadorDeInimigos : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy")){
            other.gameObject.GetComponent<enemy>().AtivarInimigo();
            Debug.Log("Ativar Inimigo");
        }
    }

}
