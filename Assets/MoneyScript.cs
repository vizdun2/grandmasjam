using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    public TMP_Text text;

    public int startingMoney;

    private int money;
    public int Money
    {
        get { return money; }
        set {
            money = value;
            text.text = $"${money}";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Money = startingMoney;
    }

    // Update is called once per frame
    void Update()
    {
    }
}