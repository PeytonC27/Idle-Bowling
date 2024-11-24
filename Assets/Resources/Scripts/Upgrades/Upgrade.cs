using System;
using System.Collections;
using UnityEngine;

public class Upgrade
{
    public string name;
    public string upgradeText;
    public int cost;
    public int level;
    public int maxLevel;
    public Func<int, int> IncrementFunction;

    public Action OnUpgrade;

    public Upgrade(string name, string upgradeText, int cost, int maxLevel, Func<int, int> incrementFunction, Action onUpgrade)
    {
        this.name = name;
        this.upgradeText = upgradeText;
        this.cost = cost;
        this.maxLevel = maxLevel;
        this.level = 0;
        this.IncrementFunction = incrementFunction;
        this.OnUpgrade = onUpgrade;
    }

    bool CanUpgrade(int score)
    {
        return score >= cost && level < maxLevel;
    }

    public void ApplyUpgrade(ref int currentScore, bool zeroCost = false)
    {
        if (!CanUpgrade(currentScore)) return;

        currentScore -= zeroCost ? 0 : cost;
        cost = IncrementFunction(cost);
        level++;
        OnUpgrade?.Invoke();
    }
}