using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeradorDeObjetos : MonoBehaviour
{
    public GameObject[] objetosParaSpawnar;
    public Transform[] pontosDeSpawn;

    public float tempoMaxEntreSpawns;
    public float tempoAtualSpawns;
    // Start is called before the first frame update
    void Start()
    {
        tempoAtualSpawns = tempoMaxEntreSpawns;
    }

    // Update is called once per frame
    void Update()
    {
        tempoAtualSpawns -= Time.deltaTime;
        if (tempoAtualSpawns <= 0)
        {
            SpawnarObj();
        }
    }

    private void SpawnarObj()
    {
        int objetoAleatorio = Random.Range(0, objetosParaSpawnar.Length);
        int pontoAleatorio = Random.Range(0, pontosDeSpawn.Length);
        
        Instantiate(objetosParaSpawnar[objetoAleatorio], pontosDeSpawn[pontoAleatorio].position, Quaternion.Euler(0f, 0f, 180f));
        tempoAtualSpawns = tempoMaxEntreSpawns;
    }
}
