using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiro : MonoBehaviour
{
    public float LaserSpeed = 5f;
    public int dano;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
    }

    private void MoveLaser(){
        transform.Translate(Vector3.up * LaserSpeed * Time.deltaTime);
    }

    OnTriggerEnter2D(Collider other){
        if(other.gameObject.CompareTag("enemy")){
            other.gameObject.GetComponente<enemy>().danoInimigo(dano);
            Destroy(gameObject);
        }
    }
    
}
