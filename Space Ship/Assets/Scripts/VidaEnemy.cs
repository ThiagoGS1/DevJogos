using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaEnemy : MonoBehaviour
{
    public int vidaMax;
    public int vidaAtual;
    // Start is called before the first frame update
    void Start()
    {
        vidaAtual = vidaMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void damage(int dano){
        vidaAtual -= dano;

        if(vidaAtual <= 0){
            Destroy(gameObject);
        }
    }
}
