using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Wired");
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
