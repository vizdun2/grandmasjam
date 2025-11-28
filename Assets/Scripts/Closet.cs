using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closet : MonoBehaviour
{
    public Player player;
    
    void OnMouseDown()
    {
        player.closetOpened();
    }
}
