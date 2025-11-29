using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public Player player;
    
    void OnMouseDown()
    {
        player.leverPulled();
        GetComponent<ShityAnime>().Animate(true);
    }
}