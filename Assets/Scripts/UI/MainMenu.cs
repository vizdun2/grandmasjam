using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu: MonoBehaviour
{
    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
