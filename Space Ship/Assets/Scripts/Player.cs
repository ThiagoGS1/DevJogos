using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Slider barraDeVidaJogador;
    public Rigidbody2D oRigidbody2D;
    public GameObject laserDoJogador;
    public Transform localDisparo;
    public float velNave;
    private Vector2 teclas;
    public int vidaAtual;
    public int vidaMax;

    // Start is called before the first frame update
    void Start()
    {
        vidaAtual = vidaMax;
        barraDeVidaJogador.maxValue = vidaMax;
        barraDeVidaJogador.value = vidaAtual;
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

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("enemy"))
            Destroy(gameObject);
    }

    public void danoPlayer(int dano_player){
        vidaAtual -= dano_player;

        barraDeVidaJogador.value = vidaAtual;
        
        if(vidaAtual <= 0){
            GameManager.instance.GameOver();
            Destroy(this.gameObject);
        }
    }

}
