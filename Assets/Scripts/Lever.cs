using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public Player player;
    public float AnimationSpeed;
    public Sprite[] AnimatedLevers;

    int animatingAtIndex = 0;
    float lastAnimated = 0;
    bool isAnimating = false;


    void OnMouseDown()
    {
        player.leverPulled();
        isAnimating = true;
    }

    void Update()
    {
        if (isAnimating && Time.time - lastAnimated > AnimationSpeed)
        {
            lastAnimated = Time.time;
            Anime();
        }
    }

    void Anime()
    {
        animatingAtIndex++;
        animatingAtIndex = animatingAtIndex % 5;
        GetComponent<SpriteRenderer>().sprite = AnimatedLevers[animatingAtIndex];
        if (animatingAtIndex == 0)
            isAnimating = false;
    }

}

