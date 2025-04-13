using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KepperRange : MonoBehaviour
{
    
    private void OnTriggerStay2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            GetComponentInParent<Animator>().Play("atk", -1);
        }
    }


}
