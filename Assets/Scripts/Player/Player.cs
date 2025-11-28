using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer automatSprite1;
    public SpriteRenderer automatSprite2;
    public SpriteRenderer automatSprite3;
    public MoneyScript money;
    public Guard guard;
    public Sprite bananaSprite;
    public Sprite cherrySprite;
    public Sprite orangeSprite;
    public Sprite pineAppleSprite;
    public Sprite peachSprite;
    public Sprite appleSprite;
    public Sprite eggplantSprite;

    public Transform miniGame;

    public float timeToSpin = 4f;
    public bool rigged = false;
    public float defaultProbability = 0.5f;

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

    private bool isPlayingMinigame = false;

    public Switch[] switchObjects;
    public Transform[] barSprites;
    List<ThingSwitch> thingSwitches = new List<ThingSwitch>();

    class ThingSwitch
    {
        public AutomatThing leftBonus;
        public AutomatThing rightBonus;
        public bool up;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < switchObjects.Length; i++)
        {
            switchObjects[i].parent = this;
            switchObjects[i].id = i;

            ThingSwitch ts = new ThingSwitch
            {
                leftBonus = randThing(),
                rightBonus = randThing(),
                up = false
            };
            thingSwitches.Add(ts);
            setSwitchFlipped(i, ts.up);
            switchObjects[i].leftBonusSprite.sprite = thingToSprite(ts.leftBonus);
            switchObjects[i].rightBonusSprite.sprite = thingToSprite(ts.rightBonus);
        }

        miniGame.gameObject.SetActive(isPlayingMinigame);

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
        if (isSpinning)
        {
            slotSpin();
            if (Time.time - startedSpinning > timeToSpin)
            {
                isSpinning = false;
                cashThings();
            }
        }
    }

    void randomizeThings()
    {
        if (rigged)
        {
            things[0] = randThing();
            things[1] = things[0];
            things[2] = things[0];
            return;
        }

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
        else if ((int)things[0] == (int)(things[1] - 1) && (int)things[0] == (int)(things[2] - 2))
        {
            money.Money += 100 * ((int)things[0] + 1);
        }
        else if (things[0] == things[1] || things[0] == things[2] || things[1] == things[2])
        {
            money.Money += 50 * ((int)things[0] + 1);
        }
        else
        {
            money.Money -= 100;
        }
    }

    AutomatThing randThing()
    {
        float val = Random.Range(0.0f, 1.0f);
        float current = 0;

        for (int i = 0; i < 7; i++)
        {
            AutomatThing t = (AutomatThing)i;
            current += calcThingWeight(t);
            if (val <= current)
            {
                return t;
            }
        }

        return AutomatThing.Banana;
    }

    public Sprite thingToSprite(AutomatThing thing)
    {
        switch (thing)
        {
            case AutomatThing.Banana:
                return bananaSprite;
            case AutomatThing.Orange:
                return orangeSprite;
            case AutomatThing.Cherry:
                return cherrySprite;
            case AutomatThing.PineApple:
                return pineAppleSprite;
            case AutomatThing.Peach:
                return peachSprite;
            case AutomatThing.Apple:
                return appleSprite;
            case AutomatThing.Eggplant:
                return eggplantSprite;
            default:
                return bananaSprite;
        }
    }

    void setSpriteToThing(SpriteRenderer sprite, AutomatThing thing)
    {
        sprite.sprite = thingToSprite(thing);
    }

    void setSwitchFlipped(int id, bool up)
    {
        thingSwitches[id].up = up;
        switchObjects[id].switchSprite.color = thingSwitches[id].up ? Color.white : Color.gray;
        redrawBars();
    }

    public void switchSwitch(int id)
    {
        setSwitchFlipped(id, !thingSwitches[id].up);
    }

    float totalWeights()
    {
        float sum = defaultProbability * 7;
        foreach (var ts in thingSwitches)
        {
            sum += (ts.up ? 2 : 0);
        }

        return sum;
    }

    float calcThingWeight(AutomatThing thing)
    {
        float sum = defaultProbability;
        foreach (var ts in thingSwitches)
        {
            sum += ((ts.leftBonus == thing && ts.up) ? 1 : 0) + ((ts.rightBonus == thing && ts.up) ? 1 : 0);
        }

        return (sum == 0) ? 0 : (sum / totalWeights());
    }

    void redrawBars()
    {
        for (int i = 0; i < 7; i++)
        {
            barSprites[i].transform.localScale = new Vector3(barSprites[i].transform.localScale.x,
                calcThingWeight((AutomatThing)i), barSprites[i].transform.localScale.z);
        }
    }

    public void leverPulled()
    {
        if (!isSpinning && money.Money >= 100)
        {
            isSpinning = true;
            lastSpin = Time.time;
            startedSpinning = lastSpin;
        }
    }

    public void closetOpened()
    {
        isPlayingMinigame = !isPlayingMinigame;
        miniGame.gameObject.SetActive(isPlayingMinigame);
    }
}