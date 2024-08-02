using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntrusoGameController : MonoBehaviour
{
    public static IntrusoGameController instance;

    [SerializeField] private CardsController cardsController;
    [SerializeField] private int numberOfErrors;
    [SerializeField] private TimerController timer;
    [SerializeField] private float points = 100;
    [SerializeField] private bool gameFinished = false;

    private void Start()
    {
        cardsController = FindObjectOfType<CardsController>();
        timer = FindObjectOfType<TimerController>();
        cardsController.GameController = this;
        points = 100;
        numberOfErrors = 0;
    }

    public double GetTimeElapsed()
    {
        return timer.GetTimeElapsed();
    }

    public void AddPlayerMiss()
    {
        numberOfErrors++;
        points -= 10;
    }

    public void StartTimer()
    {
        timer.BeginTimer();
    }

    public void StopTimer()
    {
        timer.EndTimer();
    }

    public int GetNumberOfPlayerMisses()
    {
        return numberOfErrors;
    }

    public float GetPoints()
    {
        return points;
    }

    public void FinishGame()
    {
        gameFinished = true;
    }

    public bool isGameFinished()
    {
        return gameFinished;
    }
}
