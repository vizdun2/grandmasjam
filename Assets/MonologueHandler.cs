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

    private AudioSource grandmaVoice;
    public TextMeshProUGUI subtitleText;

    private float lastChangedSubtitleText = 0;

    // Start is called before the first frame update
    void Start()
    {
        grandmaVoice = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastChangedSubtitleText > subtitleText.text.Count() * 0.2f)
        {
            popTutorialSubs();
        }
    }

    private void popTutorialSubs()
    {
        lastChangedSubtitleText = Time.time;
        subtitleText.text = "";
    }

    private void pushTutorialSubs(TutorialMonologueCall call)
    {
        subtitleText.text = tutorialSubs[(int)call];
        lastChangedSubtitleText = Time.time;
    }

    public void PlayTutorial(TutorialMonologueCall call)
    {
        pushTutorialSubs(call);
        // grandmaVoice.PlayOneShot(tutorialClips[(int)call]);
    }
}