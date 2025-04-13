using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControler : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anime;
    private float moveX;

    public float speed;
    public int addJumps;
    public bool isGrounded;
    public float jumpForce;

    public int life;
    public TextMeshProUGUI textLife;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        textLife.text = life.ToString();
    }

    void FixedUpdate(){
        Move();
        Atk();

        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, rb.velocity.y);

        if(isGrounded == true){
            addJumps = 1;
            if(Input.GetButtonDown("Jump")){
                Jump();
            }
        }
        else{
            if(Input.GetButtonDown("Jump") && addJumps > 0){
            addJumps--;
            Jump();
        }}
    }

    void Move(){

        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if(moveX > 0){
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            anime.SetBool("isRun", true);
        }

        if(moveX < 0){
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            anime.SetBool("isRun", true);
        }

        if(moveX == 0){
            anime.SetBool("isRun", false);
        }
    }

    void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        anime.SetBool("isJump", true);
    }

    void Atk(){
        if(Input.GetButtonDown("Fire1")){
            anime.Play("attack", -1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Ground"){
            isGrounded = true;
            anime.SetBool("isJump", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.tag == "Ground"){
            isGrounded = false;
        }
    }
}
