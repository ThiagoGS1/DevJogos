using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool isFinishLine = false;
    public int checkPointNumber = 1;
    
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o carro do jogador que está colidindo
        if (isFinishLine && other.CompareTag("Player"))
        {
            // Notifica o GameManager que o jogador cruzou a linha de chegada
            GameManager.instance.OnRaceCompleted();
        }
    }
}