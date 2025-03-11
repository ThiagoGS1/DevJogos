using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{

    public float speed = 10;
    public float max_x = 6;
    float movHorizontal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movHorizontal = Input.GetAxis("Horizontal");
        if((movHorizontal>0 && transform.position.x<max_x) || (movHorizontal<0 && transform.position.x>-max_x)){
            transform.position += Vector3.right * movHorizontal * speed * Time.deltaTime;
        }
    }
}
