using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D oRigidbody2D;
    public GameObject laserDoJogador;
    public Transform localDisparo;

    public float velNave;
    private Vector2 teclas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Atk();
    }

    void Move(){
        teclas = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        oRigidbody2D.velocity = teclas.normalized * velNave;
    }

    void Atk(){
        if(Input.GetButtonDown("Fire1")){
            Instantiate(laserDoJogador, localDisparo.position, localDisparo.rotation);
        }
    }

}
