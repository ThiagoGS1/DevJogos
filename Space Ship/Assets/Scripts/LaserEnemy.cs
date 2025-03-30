using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    public float VelLaser;
    // Start is called before the first frame update
    public int dano;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
    }

    private void MoveLaser(){
        transform.Translate(Vector3.up * VelLaser * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<Player>().danoPlayer(dano);
            Destroy(this.gameObject);
        }
    }

}
