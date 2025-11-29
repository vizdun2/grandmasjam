using System.Collections;
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
        WillNotSeePlayer,
        Angry
    }

    public enum GuardAngerState
    {
        NotPlaying,
        NoticeTampered,
        TamperingWithMachine,
        None
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
    public GameObject healthBar;
    bool noticedYouAreNotSpinning = false;
    float noticedYouAreNotSpinningAt;
    float stateDuration = 0;
    public Sprite[] turnAnimationFrames; // 4 sprity otáčení
    public float turnFrameDuration = 0.07f;
    bool isAnimating = false;
    Coroutine stateChangeCoroutine;

    public TextMeshProUGUI stateText;
    GuardAngerState angerReason;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HandleAngerState();
        UpdateAngerLevel(Time.deltaTime);
        CycleState();
        UpdateGuardFace();
    }

    private void HandleAngerState()
    {
        switch (angerReason)
        {
            case GuardAngerState.NotPlaying:
                stateText.text = "Guard sees you are not playing";
                break;
            case GuardAngerState.NoticeTampered:
                stateText.text = "Guard suspects the machine has been tampered with";
                break;
            case GuardAngerState.TamperingWithMachine:
                stateText.text = "Guard sees you doing something suspicious!!!";
                break;
            case GuardAngerState.None:
                stateText.text = "";
                break;
        }
    }

    public void UpdateAngerLevel(float delta)
    {
        if (currentState == GuardState.SeesPlayer)
        {
            if (player.GetHowLongIsPlayingMinigame() > 2f)
            {
                angerLevel += delta * 8;
                player.GetComponent<MonologueHandler>().PlayFuckup(FuckupMonologueCall.NoticeGuardNoticeYouPlayingMinigame);
                angerReason = GuardAngerState.TamperingWithMachine;
            }
            if (player.GetIsSpinning())
            { // bude nasranej na to ze cheatujes maty
                noticedYouAreNotSpinning = false;
                if (player.GetRiggedAmount() > 0)
                {
                    angerLevel += delta * 4 * player.GetRiggedAmount();
                    player.GetComponent<MonologueHandler>().PlayFuckup(FuckupMonologueCall.NoticeGuardNoticeMinigameEffect);
                }
                else
                    angerLevel -= delta * 4;

                if ((int)angerReason < (int)GuardAngerState.NoticeTampered)
                    angerReason = GuardAngerState.NoticeTampered;
            }
            else
            {
                if (!noticedYouAreNotSpinning)
                {
                    if ((int)angerReason < (int)GuardAngerState.NotPlaying)
                        angerReason = GuardAngerState.NotPlaying;
                    noticedYouAreNotSpinning = true;
                    noticedYouAreNotSpinningAt = Time.time;
                }
                if (Time.time - noticedYouAreNotSpinningAt > 5f)
                {
                    player.GetComponent<MonologueHandler>().PlayFuckup(FuckupMonologueCall.NoticeGuardNoticeNotPlaying);
                    angerLevel += delta * 2;
                }
            }
        }
        else if (currentState == GuardState.DoesNotSeePlayer)
        {
            noticedYouAreNotSpinning = false;
        }
        else if (currentState == GuardState.Angry)
        {
            // Pokud hráč hraje správně, guard se uklidňuje i když je naštvaný
            if (player.GetIsSpinning() && player.GetRiggedAmount() == 0)
            {
                angerLevel -= delta * 3; // Pomalu se uklidňuje
            }
            else
            {
                angerLevel += delta * 2; // Pokud cheatuje, ještě víc se naštve
            }
        }

        angerLevel = Mathf.Clamp(angerLevel, 0f, angerLimit);
        UpdateHealthBar();
        if (angerLevel >= angerLimit / 2f)
        {
            ChangeState(GuardState.Angry);
        }
        if (angerLevel >= angerLimit)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
    }

    void ChangeState(GuardState newState)
    {
        if (currentState == newState) return;

        // stop předchozí animace, pokud běží
        if (stateChangeCoroutine != null)
        {
            StopCoroutine(stateChangeCoroutine);
            stateChangeCoroutine = null;
        }

        GuardState oldState = currentState;
        currentState = newState;
        stateDuration = 0f;

        // spustíme animaci přechodu
        stateChangeCoroutine = StartCoroutine(PlayTurnAnimation(oldState, newState));
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
            case GuardState.Angry:
                // zůstane naštvaný, dokud mu neklesne hladina hněvu pod polovinu
                if (angerLevel < angerLimit * .5f)
                {
                    ChangeState(GuardState.DoesNotSeePlayer);
                    stateDuration = 0;
                }
                break;

        }
    }

    void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector2(angerLevel / angerLimit, healthBar.transform.localScale.y);
    }

    void UpdateGuardFace()
    {
        // když právě přehrávám animaci, nech to být
        if (isAnimating) return;

        UpdateGuardFaceImmediate();
    }

    void UpdateGuardFaceImmediate()
    {
        Sprite newFace = null;

        // hodně naštvaný → angry
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
                case GuardState.Angry:
                    newFace = faceAngry;
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

    IEnumerator PlayTurnAnimation(GuardState from, GuardState to)
    {
        isAnimating = true;
        if (guardFaceRenderer != null && turnAnimationFrames != null && turnAnimationFrames.Length > 0)
        {
            for (int i = 0; i < turnAnimationFrames.Length; i++)
            {
                if (turnAnimationFrames[i] == null) continue;

                guardFaceRenderer.sprite = turnAnimationFrames[i];
                yield return new WaitForSeconds(turnFrameDuration);
            }
        }

        UpdateGuardFaceImmediate();

        isAnimating = false;
        stateChangeCoroutine = null;
    }
}