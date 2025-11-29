using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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

    public enum MailEvent
    {
        Babis,
        Duchod,
        PlusDuchod,
        MinusDuchod,
    }

    string[,] EVENT_LINES =
    {
        { "Babis1", "Babis2", "Babis3" },
        { "Duchod1", "Duchod2", "Duchod3" },
        { "PlusDuchod1", "PlusDuchod2", "PlusDuchod3" },
        { "MinusDuchod1", "MinusDuchod2", "MinusDuchod3" },
    };

    string eventToLine(MailEvent me)
    {
        return EVENT_LINES[(int)me, Random.Range(0, 3)];
    }

    private MailEvent? pendingEvent = null;

    private AudioSource grandmaVoice;
    public TextMeshProUGUI subtitleText;

    private float lastChangedSubtitleText = 0;

    public GameObject bubble;
    
    public float subTime = 0.2f;
    public Player player;

    private bool isShownRn
    {
        get { return bubble.activeInHierarchy; }
        set { bubble.SetActive(value); }
    }

// Start is called before the first frame update
    void Start()
    {
        grandmaVoice = gameObject.GetComponent<AudioSource>();
        isShownRn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastChangedSubtitleText > subtitleText.text.Count() * subTime)
        {
            popTutorialSubs();
        }
    }

    public void maybePushEvent(MailEvent maile)
    {
        if (pendingEvent != null)
            return;
        else if (isShownRn)
        {
            pendingEvent = maile;
        }
        else
        {
            pushSomeSubs("Postak", eventToLine(maile));
            player.applyEvent((MailEvent)maile);
        }
    }

    private void pushSomeSubs(string kdo, string co)
    {
        subtitleText.text = co;
        lastChangedSubtitleText = Time.time;
        isShownRn = true;
    }

    private void popTutorialSubs()
    {
        if (pendingEvent != null)
        {
            pushSomeSubs("Postak", eventToLine((MailEvent)pendingEvent));
            player.applyEvent((MailEvent)pendingEvent);
        }
        else
        {
            lastChangedSubtitleText = Time.time;
            subtitleText.text = "";
            isShownRn = false;
        }
    }

    private void pushTutorialSubs(TutorialMonologueCall call)
    {
        subtitleText.text = tutorialSubs[(int)call];
        lastChangedSubtitleText = Time.time;
        isShownRn = true;
    }

    public void PlayTutorial(TutorialMonologueCall call)
    {
        pushTutorialSubs(call);
        grandmaVoice.PlayOneShot(tutorialClips[(int)call]);
    }
}