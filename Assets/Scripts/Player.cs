using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer automatSprite1;
    public SpriteRenderer automatSprite2;
    public SpriteRenderer automatSprite3;
    public MoneyScript money;

    private AutomatThing[] things = {AutomatThing.Banana, AutomatThing.Banana, AutomatThing.Banana};

    public enum AutomatThing
    {
        Banana,
        Cherry,
        Orange,
        T4,
        T5,
        T6,
        T7,
    }

    // Start is called before the first frame update
    void Start()
    {
        randomizeThings();
        displayThings();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            randomizeThings();
            displayThings();
            cashThings();
        }
    }

    void randomizeThings()
    {
        things[0] = randThing();
        things[1] = randThing();
        things[2] = randThing();
    }
    
    void displayThings()
    {
        setSpriteToThing(automatSprite2, things[0]);
        setSpriteToThing(automatSprite1, things[1]);
        setSpriteToThing(automatSprite3, things[2]);
    }

    void cashThings()
    {
        if (things[0] == things[1] && things[1] == things[2])
        {
            money.Money += 500;
        }
    }

    AutomatThing randThing()
    {
        switch (Random.Range(0, 7))
        {
            case 0:
                return AutomatThing.Banana;
            case 1:
                return AutomatThing.Cherry;
            case 2:
                return AutomatThing.Orange;
            case 3:
                return AutomatThing.T4;
            case 4:
                return AutomatThing.T5;
            case 5:
                return AutomatThing.T6;
            case 6:
                return AutomatThing.T7;
            default:
                return AutomatThing.Banana;
        }
    }

    void setSpriteToThing(SpriteRenderer sprite, AutomatThing thing)
    {
        switch (thing)
        {
            case AutomatThing.Banana:
                sprite.color = Color.yellow;
                break;
            case AutomatThing.Orange:
                sprite.color = Color.yellow + Color.red;
                break;
            case AutomatThing.Cherry:
                sprite.color = Color.red;
                break;
            case AutomatThing.T4:
                sprite.color = Color.blue;
                break;
            case AutomatThing.T5:
                sprite.color = Color.green;
                break;
            case AutomatThing.T6:
                sprite.color = Color.cyan;
                break;
            case AutomatThing.T7:
                sprite.color = Color.magenta;
                break;
        }
    }
}