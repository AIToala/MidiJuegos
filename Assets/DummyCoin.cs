using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCoin : MonoBehaviour
{
    public MainController main;
    public GameObject textCoin;
    private int coins;
    void Start()
    {
        main = MainController.main;
        coins = main.GetCoins();

        textCoin.GetComponent<TMPro.TextMeshProUGUI>().text = coins.ToString();
    }

    void Update()
    {
        if (main.GetCoins() != coins)
        {
            coins = main.GetCoins();

            textCoin.GetComponent<TMPro.TextMeshProUGUI>().text = coins.ToString();
        }
    }
}
