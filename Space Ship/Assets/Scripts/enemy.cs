using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public GameObject laserEnemy;
    public Transform localShoot;
    public float velEnemy;
    public float tempoMaxLasers;
    public float tempoAtualLasers;
    public int vidaAtual;
    public int vidaMax;
    public bool atiradorxd;
    // Start is called before the first frame update
    void Start()
    {
        vidaAtual = vidaMax;
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();

        if(atiradorxd == true)
            AtirarLaser();

    }

    private void MoveEnemy(){
        transform.Translate(Vector3.down * velEnemy * Time.deltaTime);
    }

    private void AtirarLaser(){
        tempoAtualLasers -= Time.deltaTime;

        if(tempoAtualLasers <= 0){
            Instantiate(laserEnemy, localShoot.position, Quaternion.Euler(0f,0f,0f));
            tempoAtualLasers = tempoMaxLasers;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }
    
    public void danoInimigo(int damage_inimigo){
        vidaAtual -= damage_inimigo;
        if(vidaAtual <= 0){
            Destroy(gameObject);
        }
    }
}
