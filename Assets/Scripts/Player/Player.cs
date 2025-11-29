using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;


public enum TutorialMonologueCall
{
    NoticeCouldOpenCloset,
    EnterMinigame,
    NoticeMinigameEffect,
    NoticeGuardNoticeNotPlaying,
    NoticeGuardNoticeMinigameEffect,
    NoticeGuardNoticeYouPlayingMinigame,
}

public class Player : MonoBehaviour
{
    [System.Serializable]
    public class EconomyConfig
    {
        public int bet = 100;
        public int threeOfKind = 500;
        public int twoOfKind = 200;
        public int straight = 500;
    }

    public EconomyConfig economy;
    public float timeToShuffleSeconds = 60;

    public SpriteRenderer automatSprite1;
    public SpriteRenderer automatSprite2;
    public SpriteRenderer automatSprite3;
    public SpriteRenderer slotsBackground;
    public MoneyScript money;
    public Guard guard;
    public Sprite bananaSprite;
    public Sprite cherrySprite;
    public Sprite orangeSprite;
    public Sprite pineAppleSprite;
    public Sprite peachSprite;
    public Sprite appleSprite;
    public Sprite eggplantSprite;
    private bool[] playedTutorial = new bool[6];

    public Sprite[] movingThingsSprites;

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

    private float playingMinigameFor = 0;

    public Switch[] switchObjects;
    public Transform[] barSprites;
    List<ThingSwitch> thingSwitches = new List<ThingSwitch>();

    class ThingSwitch
    {
        public AutomatThing leftBonus;
        public AutomatThing rightBonus;
        public bool up;
    }

    private MonologueHandler monologueHandler;
    private int spinCount = 0;
    private int spinCountAtRigged = 0;
    private float lastPlayedTutorial = 0;

    private float lastShuffledCables = 0;

    private float lastTriedEvent = 0;
    private float timeToEventSeconds = 30;
    public int meanTimeToEvent = 2;

    void Start()
    {
        monologueHandler = GetComponent<MonologueHandler>();
        miniGame.gameObject.SetActive(isPlayingMinigame);
        randomizeThings();
        shuffleCables();
        displayThings();
        slotsBackground.sprite = movingThingsSprites[0];
    }

    void slotSpin() // just visualization
    {
        if (Time.time - lastSpin > 0.1f)
        {
            lastSpin = Time.time;
            slotsBackground.sprite = movingThingsSprites[Random.Range(1, movingThingsSprites.Length)];
            // guard.UpdateAngerLevel(0.1f);
        }
    }

    void Update()
    {
        if (Time.time > 10)
        {
            Tutorial(TutorialMonologueCall.NoticeCouldOpenCloset);
        }

        if (isSpinning)
        {
            slotSpin();
            if (Time.time - startedSpinning > timeToSpin)
            {
                isSpinning = false;
                slotsBackground.sprite = movingThingsSprites[0];

                randomizeThings();
                displayThings();
                automatSprite1.enabled = true;
                automatSprite2.enabled = true;
                automatSprite3.enabled = true;
                cashThings();
                if (spinCount - spinCountAtRigged > 1 && GetRiggedAmount() > 0)
                    Tutorial(TutorialMonologueCall.NoticeMinigameEffect);
                spinCount++;
            }
        }

        if (currentCable != null)
        {
            if (currentCable.fromtFruitWire == null || currentCable.toPosWire == null)
            {
                currentCable.cable.to = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (currentCable.fromtFruitWire != null)
                    currentCable.cable.from =
                        fruitWires[(int)currentCable.fromtFruitWire].gameObject.transform.position;
                if (currentCable.toPosWire != null)
                    currentCable.cable.from = posWires[(int)currentCable.toPosWire].gameObject.transform.position;
            }
        }

        if (Time.time - lastShuffledCables > timeToShuffleSeconds)
            shuffleCables();

        if (Time.time - lastTriedEvent > timeToEventSeconds)
        {
            lastTriedEvent = Time.time;

            if (Random.Range(0, meanTimeToEvent) == 0)
            {
                monologueHandler.maybePushEvent((MonologueHandler.MailEvent)Random.Range(0, 4));
            }
        }


        if (Input.GetKeyDown("space"))
            shuffleCables();

        if (isPlayingMinigame)
            playingMinigameFor += Time.deltaTime;

        if (Input.GetKeyDown("backspace"))
            debugStats();
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

        for (int i = 0; i < 3; i++)
            things[i] = randThing();

        for (int i = 0; i < 3; i++)
        {
            if (cableWireIndex.ContainsKey(i))
                things[shuffledWires[i]] = shuffledThings[(int)cableWireIndex[i].fromtFruitWire];
        }
    }

    void displayThings()
    {
        setSpriteToThing(automatSprite2, things[0]);
        setSpriteToThing(automatSprite1, things[1]);
        setSpriteToThing(automatSprite3, things[2]);
    }

    int thingsValue()
    {
        if (things[0] == things[1] && things[1] == things[2])
            return economy.threeOfKind;
        else if (((int)things[0] == (int)(things[1] - 1) && (int)things[0] == (int)(things[2] - 2)) ||
                 ((int)things[0] == (int)(things[1] + 1) && (int)things[0] == (int)(things[2] + 2)))
            return economy.straight;
        else if (things[0] == things[1] || things[0] == things[2] || things[1] == things[2])
            return economy.twoOfKind;

        return 0;
    }

    void cashThings()
    {
        money.Money -= economy.bet;
        money.Money += thingsValue();
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

    public void leverPulled()
    {
        if (!isSpinning && money.Money >= 100)
        {
            isSpinning = true;
            lastSpin = Time.time;
            startedSpinning = lastSpin;
            automatSprite1.enabled = false;
            automatSprite2.enabled = false;
            automatSprite3.enabled = false;
        }
    }

    public void closetOpened()
    {
        playedTutorial[(int)TutorialMonologueCall.NoticeCouldOpenCloset] = true;
        Tutorial(TutorialMonologueCall.EnterMinigame);
        isPlayingMinigame = !isPlayingMinigame;
        playingMinigameFor = 0;
        miniGame.gameObject.SetActive(isPlayingMinigame);
        if (GetRiggedAmount() > 0)
        {
            spinCountAtRigged = spinCount;
        }
    }

    class CableConnection
    {
        public int? fromtFruitWire;
        public int? toPosWire;
        public Cable cable;
    }

    public WireEnd[] fruitWires;
    public WireEnd[] posWires;
    public GameObject cablePrefab;
    CableConnection currentCable = null;
    Dictionary<int, CableConnection> cableWireIndex = new Dictionary<int, CableConnection>();
    Dictionary<int, CableConnection> cableFruitIndex = new Dictionary<int, CableConnection>();
    AutomatThing[] shuffledThings = new AutomatThing[4];
    int[] shuffledWires = new int[3];

    void shuffleCables()
    {
        if (currentCable != null)
        {
            Destroy(currentCable.cable.gameObject);
            currentCable = null;
        }

        foreach (var cb in cableWireIndex)
            Destroy(cb.Value.cable.gameObject);
        foreach (var cb in cableFruitIndex)
            Destroy(cb.Value.cable.gameObject);
        cableWireIndex.Clear();
        cableFruitIndex.Clear();

        for (int i = 0; i < 4; i++)
            shuffledThings[i] = (AutomatThing)Random.Range(0, 7);

        List<int> wireBag = new List<int>(new[] { 0, 1, 2 });
        for (int i = 0; i < 3; i++)
        {
            int idx = Random.Range(0, wireBag.Count);
            shuffledWires[i] = wireBag[idx];
            wireBag.RemoveAt(idx);
        }
    }

    void finishCurrentCable()
    {
        if (currentCable != null && currentCable.fromtFruitWire != null && currentCable.toPosWire != null)
        {
            cableFruitIndex[(int)currentCable.fromtFruitWire] = currentCable;
            cableWireIndex[(int)currentCable.toPosWire] = currentCable;
            currentCable.cable.from = fruitWires[(int)currentCable.fromtFruitWire].gameObject.transform.position;
            currentCable.cable.to = posWires[(int)currentCable.toPosWire].gameObject.transform.position;
            currentCable = null;
        }
    }

    public void clickedPosWire(int id)
    {
        if (currentCable == null)
        {
            if (!cableWireIndex.ContainsKey(id))
            {
                currentCable = new CableConnection();
                currentCable.toPosWire = id;
                GameObject cb = Instantiate(cablePrefab);
                cb.transform.SetParent(miniGame);
                currentCable.cable = cb.GetComponent<Cable>();
            }
            else
            {
                currentCable = cableWireIndex[id];
                cableFruitIndex.Remove((int)currentCable.fromtFruitWire);
                cableWireIndex.Remove((int)currentCable.toPosWire);
                currentCable.toPosWire = null;
            }
        }
        else if (currentCable.toPosWire == null && !cableWireIndex.ContainsKey(id))
        {
            currentCable.toPosWire = id;
        }
        else
        {
            Destroy(currentCable.cable.gameObject);
            currentCable = null;
        }

        finishCurrentCable();
    }

    public void clickedFruitWire(int id)
    {
        if (currentCable == null)
        {
            if (!cableFruitIndex.ContainsKey(id))
            {
                currentCable = new CableConnection();
                currentCable.fromtFruitWire = id;
                GameObject cb = Instantiate(cablePrefab);
                cb.transform.SetParent(miniGame);
                currentCable.cable = cb.GetComponent<Cable>();
            }
            else
            {
                currentCable = cableFruitIndex[id];
                cableFruitIndex.Remove((int)currentCable.fromtFruitWire);
                cableWireIndex.Remove((int)currentCable.toPosWire);
                currentCable.fromtFruitWire = null;
            }
        }
        else if (currentCable.fromtFruitWire == null && !cableFruitIndex.ContainsKey(id))
        {
            currentCable.fromtFruitWire = id;
        }
        else
        {
            Destroy(currentCable.cable.gameObject);
            currentCable = null;
        }

        finishCurrentCable();
    }


    public void Tutorial(TutorialMonologueCall tutorial)
    {
        if (!playedTutorial[(int)tutorial] && Time.time - lastPlayedTutorial > 1f)
        {
            lastPlayedTutorial = Time.time;
            playedTutorial[(int)tutorial] = true;
            monologueHandler.PlayTutorial(tutorial);
        }
    }

    public bool GetIsSpinning() => isSpinning;

    public bool GetIsPlayingMinigame() => isPlayingMinigame;

    public float GetHowLongIsPlayingMinigame() => playingMinigameFor;

    public int GetRiggedAmount()
    {
        return cableFruitIndex.Count();
    }

    void debugStats()
    {
        int oldmoney = money.Money;
        List<int> pnls = new List<int>();

        for (int i = 0; i < 10_000; i++)
        {
            money.Money = 1000;
            randomizeThings();
            cashThings();
            pnls.Add(money.Money - 1000);
        }

        money.Money = oldmoney;

        Debug.Log($"Average PnL with this config and wires: ${pnls.Average()}");
    }

    private int duchodLevels = 500;
    public void applyEvent(MonologueHandler.MailEvent me)
    {
        switch (me)
        {
            case MonologueHandler.MailEvent.Duchod:
                money.Money += duchodLevels;
                break;
            case MonologueHandler.MailEvent.Babis:
                money.Money += 5_000;
                break;
            case MonologueHandler.MailEvent.MinusDuchod:
                duchodLevels -= 100;
                break;
            case MonologueHandler.MailEvent.PlusDuchod:
                duchodLevels += 100;
                break;
        }
    }
}