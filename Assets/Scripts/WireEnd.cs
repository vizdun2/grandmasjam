using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireEnd : MonoBehaviour
{
    public Player player;
    public int wireId;
    public bool isFruitWire = true;
    
    void OnMouseDown()
    {
        if (isFruitWire)
            player.clickedFruitWire(wireId);
        else
            player.clickedPosWire(wireId);
    }
}
