using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public int id;
    public SpriteRenderer switchSprite;
    public SpriteRenderer leftBonusSprite;
    public SpriteRenderer rightBonusSprite;
    public Player parent;

    void Awake()
    {
        switchSprite = transform.Find("SwitchSprite").GetComponent<SpriteRenderer>();
        leftBonusSprite = transform.Find("LeftBonusSprite").GetComponent<SpriteRenderer>();
        rightBonusSprite = transform.Find("RightBonusSprite").GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        parent.switchSwitch(id);
    }
}