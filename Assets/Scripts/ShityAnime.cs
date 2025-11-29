using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShityAnime : MonoBehaviour
{
    public float AnimationSpeed = 0.1f;
    public Sprite[] AnimatedLevers;

    SpriteRenderer sprite;

    int animatingAtIndex = 0;
    float lastAnimated = 0;
    public bool isAnimating = false;
    public bool loop = true;
    public bool dieOnEnd = false;
    public bool endOnEnd = false;
    public bool backwards = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Animate(bool value)
    {
        isAnimating = true;
        lastAnimated = Time.time;
        animatingAtIndex = 0;
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
        if (!backwards)
            animatingAtIndex++;
        else
            animatingAtIndex--;
        animatingAtIndex = Math.Abs(animatingAtIndex % AnimatedLevers.Length);
        sprite.sprite = AnimatedLevers[animatingAtIndex];
        if (animatingAtIndex == 0 && !loop)
        {
            isAnimating = false;
            if (dieOnEnd)
                Destroy(gameObject);
            else if (endOnEnd)
            {
                int idx = AnimatedLevers.Length - 1;
                sprite.sprite = AnimatedLevers[idx];
                animatingAtIndex = idx;
            }
        }
    }
}