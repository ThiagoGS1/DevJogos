using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text textoPontuação;
    public GameObject painelGameOver;
    public int inimigosDerrotados;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {   
        inimigosDerrotados = 0;
        textoPontuação.text = "Pontuação: " + inimigosDerrotados;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pontuar(int pontuacao){
        inimigosDerrotados += pontuacao;
        textoPontuação.text = "Pontuação: " + inimigosDerrotados;
    }

    public void GameOver(){
        painelGameOver.SetActive(true);
    }
}
