using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    public int playerNumber = 1;
    public bool isUIInput = false;

    Vector2 inputVector = Vector2.zero;

    //Components
    TopDownCarController topDownCarController;

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        topDownCarController = GetComponent<TopDownCarController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame and is frame dependent
    void Update()
    {
        if (isUIInput)
        {

        }
        else
        {
            inputVector = Vector2.zero;

            switch (playerNumber)
            {
                case 1:
                    // Get input from arrow keys via Unity's input system
                    float arrowHorizontal = Input.GetAxis("Horizontal_P1");
                    float arrowVertical = Input.GetAxis("Vertical_P1");

                    // Get input from WASD keys
                    float wasdHorizontal = 0f;
                    float wasdVertical = 0f;

                    if (Input.GetKey(KeyCode.A)) wasdHorizontal -= 1f;
                    if (Input.GetKey(KeyCode.D)) wasdHorizontal += 1f;
                    if (Input.GetKey(KeyCode.W)) wasdVertical += 1f;
                    if (Input.GetKey(KeyCode.S)) wasdVertical -= 1f;

                    // Use whichever input has higher value (arrows or WASD)
                    inputVector.x = Mathf.Abs(arrowHorizontal) > Mathf.Abs(wasdHorizontal) ? arrowHorizontal : wasdHorizontal;
                    inputVector.y = Mathf.Abs(arrowVertical) > Mathf.Abs(wasdVertical) ? arrowVertical : wasdVertical;
                    break;

                case 2:
                    //Get input from Unity's input system.
                    inputVector.x = Input.GetAxis("Horizontal_P2");
                    inputVector.y = Input.GetAxis("Vertical_P2");
                    break;

                case 3:
                    //Get input from Unity's input system.
                    inputVector.x = Input.GetAxis("Horizontal_P3");
                    inputVector.y = Input.GetAxis("Vertical_P3");
                    break;

                case 4:
                    //Get input from Unity's input system.
                    inputVector.x = Input.GetAxis("Horizontal_P4");
                    inputVector.y = Input.GetAxis("Vertical_P4");
                    break;
            }
        }

        //Send the input to the car controller.
        topDownCarController.SetInputVector(inputVector);
    }

    public void SetInput(Vector2 newInput)
    {
        inputVector = newInput;
    }
}
