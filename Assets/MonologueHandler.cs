using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum EventEffect
{
    Babis,
    Duchod,
    PlusDuchod,
    MinusDuchod,
    None
}

public struct GameEvent
{
    public string monologue;
    public EventEffect effect;
    public AudioClip audioClip;
}

public class MonologueHandler : MonoBehaviour
{
    public AudioClip[] tutorialClips; // put clips as if indexed by enum

    private string[] tutorialSubs =
    {
        "I could open the closet down there!",
        "If I found out how these wires change the slot machine outcomes, I could hack it somehow!!!",
        "I see! So it seems that the wires set one fruit for a slot permanently!",
        "Ooooh! I have to play, otherwise they will kick me out. Comrade Husak would never kick a grandma out of a casino",
        "Doprdele. Maybe the guard is onto the fact that I rigged it. I should return the wires back to normal",
        "Oh no! The guard is looking my way. I have to stop tampering with the slot machine."
    };


    string[,] EVENT_LINES =
    {
        { "Andrej Babis is buying voters! You received money.", "Andrej Babis is buying voters! You received money.", "Andrej Babis is buying voters! You received money." },
        { "You received your retirement pay!", "You received your retirement pay", "You received your retirement pay" },
        { "PlusDuchod1", "PlusDuchod2", "PlusDuchod3" },
        { "MinusDuchod1", "MinusDuchod2", "MinusDuchod3" },
    };


    private AudioSource grandmaVoice;
    public TextMeshProUGUI subtitleText;

    private float lastChangedSubtitleText = 0;
    List<GameEvent> eventQueue = new();

    public GameObject bubble;
    public GameObject megafon;

    public float subTime = 0.2f;
    public Player player;



    // Start is called before the first frame update
    void Start()
    {
        grandmaVoice = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time - lastChangedSubtitleText > subtitleText.text.Count() * subTime)
        {
            if (!IsQueueEmpty())
            {
                popEvent();
            }
            else
            {
                subtitleText.text = "";
            }
        }
    }


    private void popEvent()
    {
        lastChangedSubtitleText = Time.time;
        GameEvent outEvent = eventQueue[0];
        eventQueue.RemoveAt(0);
        subtitleText.text = outEvent.monologue;
        handleEventEffect(outEvent.effect);
        if (outEvent.audioClip != null)
        {
            grandmaVoice.PlayOneShot(outEvent.audioClip);
        }
    }

    private void handleEventEffect(EventEffect eventEffect)
    {   
        if (eventEffect != EventEffect.None)
        {
            InstMegaPhone();
            player.applyEvent(eventEffect);
        }
    }

    public void PushTutorial(TutorialMonologueCall call)
    {
        string subs = tutorialSubs[(int)call];
        GameEvent gameEvent = new() { monologue = subs, effect = EventEffect.None, audioClip = tutorialClips[(int)call] };
        eventQueue.Add(gameEvent);

    }

    public void PushMail()
    {
        EventEffect effectPick = (EventEffect)Random.Range(0, 4);
        string says = EVENT_LINES[(int)effectPick, Random.Range(0, 2)];
        GameEvent gameEvent = new() { effect = effectPick, monologue = says };
        eventQueue.Add(gameEvent);
    }


    public void PlayFuckup(FuckupMonologueCall call)
    {
        int index = 3 + (int)call;
        if (IsQueueEmpty() && !player.playedTutorial[index] && Time.time - lastChangedSubtitleText > subtitleText.text.Count() * subTime)
        {
            lastChangedSubtitleText = Time.time;
            player.playedTutorial[index] = true;
            grandmaVoice.PlayOneShot(tutorialClips[index]);
            subtitleText.text = tutorialSubs[index];
        }
    }

    private void InstMegaPhone()
    {
        megafon.SetActive(true);
        Invoke("DeleteMegaPhone", subtitleText.text.Count() * subTime);
    }
    private void DeleteMegaPhone()
    {
        megafon.SetActive(false);
    }
    public bool IsQueueEmpty() => eventQueue.Count() == 0;
}