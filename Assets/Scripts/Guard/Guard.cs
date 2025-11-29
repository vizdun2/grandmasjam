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

    public GuardState currentState = GuardState.SeesPlayer;

    public float angerLevel = 0f;

    public float angerLimit = 100f;

    public Player player;
    private int stateChangeInterval = 3;

    public TMP_Text stateText;
    public Image healthBar;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("cycleState", stateChangeInterval, stateChangeInterval);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAngerLevel(Time.deltaTime);
        stateText.text = $"Guard is {currentState}";
    }

    public void UpdateAngerLevel(float delta)
    {
        switch (currentState)
        {
            case GuardState.SeesPlayer:
                angerLevel += delta * 2;
                break;
            case GuardState.DoesNotSeePlayer:
                angerLevel -= delta;
                break;
            case GuardState.WillSeePlayer:
                break;
            case GuardState.WillNotSeePlayer:
                angerLevel -= delta * 0.5f;
                break;
        }
        angerLevel = Mathf.Clamp(angerLevel, 0f, angerLimit);
        UpdateHealthBar();
    }

    void ChangeState(GuardState newState)
    {
        currentState = newState;
    }

    void cycleState()
    {
        switch (currentState)
        {
            case GuardState.SeesPlayer:
                ChangeState(GuardState.WillNotSeePlayer);
                break;
            case GuardState.DoesNotSeePlayer:
                ChangeState(GuardState.WillSeePlayer);
                break;
            case GuardState.WillSeePlayer:
                ChangeState(GuardState.SeesPlayer);
                break;
            case GuardState.WillNotSeePlayer:
                ChangeState(GuardState.DoesNotSeePlayer);
                break;

        }
        stateText.text = $"Guard is {currentState}";
    }

    void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(angerLevel / angerLimit, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
}