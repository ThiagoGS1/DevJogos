using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BounceBall : MonoBehaviour
{
    
    public TextMeshProUGUI scoreTxt;
    public GameObject[] livesImage;

    public float minY = -8f; // Limite inferior da posição da bolinha
    public float maxVelocity = 15f; // Velocidade máxima permitida
    public float resetY = 1f; // Altura específica onde a bolinha irá reaparecer

    Rigidbody2D rb;

    int score = 0;
    int lives = 5;

    // Start é chamado uma vez no início
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.down * 10f;
    }

    // Update é chamado uma vez por frame
    void Update()
    {
        // Verifica se a posição Y está abaixo do limite mínimo
        if (transform.position.y < minY)
        {
            if(lives <= 0){
                GameOver();
            } else {
                // Reseta a posição da bolinha em um Y específico
                transform.position = new Vector3(0f, resetY, 0f); // X e Z permanecem zerados
                rb.velocity = Vector2.down * 10f; // Reseta a velocidade
                lives--;
                livesImage[lives].SetActive(false);
            }
        }

        // Limita a magnitude da velocidade ao máximo permitido
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision){
        Debug.Log("Colidiu com: " + collision.gameObject.name); // Exibe o objeto colidido
        
        if (collision.gameObject.CompareTag("Brick")) {
            Debug.Log("Destruindo: " + collision.gameObject.name); // Confirma a destruição
            Destroy(collision.gameObject); // Destrói APENAS o colidido
            score += 25; // Incrementa o score
            scoreTxt.text = score.ToString("000000"); // Atualiza o texto do score
        }
    }

    void GameOver(){
        Debug.Log("Game Over!");
    }
}
