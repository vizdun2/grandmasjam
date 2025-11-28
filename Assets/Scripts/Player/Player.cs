using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer automatSprite1;
    public SpriteRenderer automatSprite2;
    public SpriteRenderer automatSprite3;
    public MoneyScript money;


    private AutomatThing[] things = { AutomatThing.Banana, AutomatThing.Banana, AutomatThing.Banana };

    public enum AutomatThing
    {
        Banana,
        Cherry,
        Orange,
        PineApple,
        Peach,
        Apple,
        Eggplant,
    }
    float lastSpin;
    float startedSpinning;

    bool isSpinning = false;


    // Start is called before the first frame update
    void Start()
    {
        randomizeThings();
        displayThings();
    }


    void slotSpin() // just visualization
    {
        if (Time.time - lastSpin > 0.1f)
        {
            lastSpin = Time.time;
            randomizeThings();
            displayThings();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            isSpinning = true;
            lastSpin = Time.time;
            startedSpinning = lastSpin;
        }


        if (isSpinning)
        {
            slotSpin();
            if (Time.time - startedSpinning > 4f)
            {
                isSpinning = false;
                cashThings();
            }
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
            money.Money += 100 * ((int)things[0] + 1);
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
                return AutomatThing.PineApple;
            case 4:
                return AutomatThing.Peach;
            case 5:
                return AutomatThing.Apple;
            case 6:
                return AutomatThing.Eggplant;
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
            case AutomatThing.PineApple:
                sprite.color = Color.blue;
                break;
            case AutomatThing.Peach:
                sprite.color = Color.green;
                break;
            case AutomatThing.Apple:
                sprite.color = Color.cyan;
                break;
            case AutomatThing.Eggplant:
                sprite.color = Color.magenta;
                break;
        }
    }
}