using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class painelGameOver : MonoBehaviour
{
    // Start is called before the first frame update
    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("XDDDD");
    }

}
