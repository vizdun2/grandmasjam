using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Guard : MonoBehaviour
{
    public enum GuardState
    {
        SeesPlayer,
        DoesNotSeePlayer,
        WillSeePlayer,
        WillNotSeePlayer
    }

    public SpriteRenderer guardFaceRenderer;
    public Sprite faceSeesPlayer;
    public Sprite faceDoesNotSeePlayer;
    public Sprite faceLookRight;
    public Sprite faceLookLeft;
    public Sprite faceAngry;

    public GuardState currentState = GuardState.SeesPlayer;

    public float angerLevel = 0f;

    public float angerLimit = 100f;

    public Player player;

    public TMP_Text stateText;
    public GameObject healthBar;

    bool noticedYouAreNotSpinning = false;
    float noticedYouAreNotSpinningAt;

    float stateDuration = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAngerLevel(Time.deltaTime);
        CycleState();
        stateText.text = $"Guard is {currentState}";
        UpdateGuardFace();
    }

    public void UpdateAngerLevel(float delta)
    {
        if (currentState == GuardState.SeesPlayer)
        {
            if (player.GetHowLongIsPlayingMinigame() > 2f)
            {
                angerLevel += delta * 8;
                player.Tutorial(TutorialMonologueCall.NoticeGuardNoticeYouPlayingMinigame);
            }
            if (player.GetIsSpinning())
            { // bude nasranej na to ze cheatujes maty
                noticedYouAreNotSpinning = false;
                if (player.GetRiggedAmount() > 0)
                {
                    angerLevel += delta * 4 * player.GetRiggedAmount();
                    player.Tutorial(TutorialMonologueCall.NoticeGuardNoticeMinigameEffect);
                }
                else
                    angerLevel -= delta * 4;
            }
            else
            {

                if (!noticedYouAreNotSpinning)
                {
                    noticedYouAreNotSpinning = true;
                    noticedYouAreNotSpinningAt = Time.time;
                }
                if (Time.time - noticedYouAreNotSpinningAt > 5f)
                {
                    player.Tutorial(TutorialMonologueCall.NoticeGuardNoticeNotPlaying);
                    angerLevel += delta * 2;
                }
            }
        }
        else if (currentState == GuardState.DoesNotSeePlayer)
        {
            noticedYouAreNotSpinning = false;
        }



        angerLevel = Mathf.Clamp(angerLevel, 0f, angerLimit);
        UpdateHealthBar();
    }

    void ChangeState(GuardState newState)
    {
        currentState = newState;
        UpdateGuardFace();
    }

    void CycleState()
    {
        stateDuration += Time.deltaTime;

        switch (currentState)
        {
            case GuardState.SeesPlayer:
                if (!noticedYouAreNotSpinning)
                {
                    if (stateDuration > 10)
                    {
                        ChangeState(GuardState.WillNotSeePlayer);
                        stateDuration = 0;
                    }
                }
                break;
            case GuardState.DoesNotSeePlayer:
                if (stateDuration > 10)
                {
                    ChangeState(GuardState.WillSeePlayer);
                    stateDuration = 0;
                }
                break;
            case GuardState.WillSeePlayer:
                if (stateDuration > 2)
                {
                    ChangeState(GuardState.SeesPlayer);
                    stateDuration = 0;
                }
                break;
            case GuardState.WillNotSeePlayer:
                if (stateDuration > 2)
                {
                    ChangeState(GuardState.DoesNotSeePlayer);
                    stateDuration = 0;
                }
                break;

        }
        stateText.text = $"Guard is {currentState}";
    }

    void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector2(angerLevel / angerLimit, healthBar.transform.localScale.y);
    }

    void UpdateGuardFace()
    {
        Sprite newFace = null;

        // Pokud je hodně naštvaný, zobraz angry face
        if (angerLevel > angerLimit * .5f)
        {
            newFace = faceAngry;
        }
        else
        {
            switch (currentState)
            {
                case GuardState.SeesPlayer:
                    newFace = faceSeesPlayer;
                    break;
                case GuardState.DoesNotSeePlayer:
                    newFace = faceDoesNotSeePlayer;
                    break;
                case GuardState.WillSeePlayer:
                    newFace = faceLookRight;
                    break;
                case GuardState.WillNotSeePlayer:
                    newFace = faceLookLeft;
                    break;
            }
        }

        if (guardFaceRenderer != null && newFace != null)
        {
            guardFaceRenderer.sprite = newFace;

            Color color = guardFaceRenderer.color;
            color.a = 0.9f;
            guardFaceRenderer.color = color;
        }
    }
}