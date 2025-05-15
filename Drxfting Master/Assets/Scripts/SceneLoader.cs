using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenuScene : MonoBehaviour
{
    // Este método será chamado ao clicar no botão
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("fase");
    }

    public void LoadQuit()
    {
        // No Editor
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // No build (Windows, Android, etc)
                Application.Quit();
        #endif
    }



}
