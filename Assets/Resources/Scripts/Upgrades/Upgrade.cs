using System;
using System.Collections;
using UnityEngine;

public class Upgrade
{
    public string name;
    public string upgradeText;
    public int cost;
    public int level;
    public Func<int, int> IncrementFunction;

    public Action OnUpgrade;

    public Upgrade(string name, string upgradeText, int cost, Func<int, int> incrementFunction, Action onUpgrade)
    {
        this.name = name;
        this.upgradeText = upgradeText;
        this.cost = cost;
        this.level = 0;
        this.IncrementFunction = incrementFunction;
        this.OnUpgrade = onUpgrade;
    }

    bool CanUpgrade(int score)
    {
        return score >= cost;
    }

    public void ApplyUpgrade(ref int currentScore)
    {
        if (!CanUpgrade(currentScore)) return;

        currentScore -= cost;
        cost = IncrementFunction(cost);
        level++;
        OnUpgrade?.Invoke();
    }
}