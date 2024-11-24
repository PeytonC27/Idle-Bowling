using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class UIManager : MonoBehaviour
{
    PinDisplay pinDisplay;

    TMP_Text scoreboard;
    TMP_Text strikeScoreboard;
    TMP_Text ballStats;
    TMP_Text launchStats;

    Toggle autoToggle;

    GameObject[] upgradeButtons;

    Color cantAffordColor = new Color(0.6f, 0.6f, 0.6f);

    [SerializeField] bool a;

    // Start is called before the first frame update
    void Start()
    {
        scoreboard = GameObject.Find("Score").GetComponent<TMP_Text>();
        strikeScoreboard = GameObject.Find("StrikeScore").GetComponent<TMP_Text>();
        ballStats = GameObject.Find("BallStats").GetComponent<TMP_Text>();
        launchStats = GameObject.Find("LaunchStats").GetComponent<TMP_Text>();

        autoToggle = GameObject.Find("AutoToggle").GetComponent<Toggle>();

        pinDisplay = GameObject.Find("PinDisplay").GetComponent<PinDisplay>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupButtonUI(List<Upgrade> upgrades, Action<string> UpgradeFunction)
    {
        upgradeButtons = new GameObject[upgrades.Count];

        for (int i = 0; i < upgrades.Count; i++)
        {
            string upgradeName = upgrades[i].name;

            upgradeButtons[i] = GameObject.Find("Button" + i).gameObject;
            upgradeButtons[i].GetComponent<Button>().onClick.AddListener(delegate { UpgradeFunction(upgradeName); });

        }
    }

    public void UpdateButtonHighlights(List<Upgrade> upgrades, int score, int strikeScore, bool allowedToUpgrade)
    {
        if (upgradeButtons.Length != upgrades.Count)
            return;

        for (int i = 0; i < upgradeButtons.Length; i++)
        {

            int scoreToUse = upgrades[i].costType == CostType.SCORE ? score : strikeScore;
            HighlightButton(upgradeButtons[i], scoreToUse, upgrades[i].cost, allowedToUpgrade);
        }
    }

    public void UpdateCosts(List<Upgrade> upgrades)
    {
        if (upgradeButtons.Length != upgrades.Count)
            return;

        for (int i = 0; i < upgradeButtons.Length; i++)
            SetButtonText(upgradeButtons[i], upgrades[i]);
    }

    /// <summary>
    /// Highlights a button based on the score parameter compared to the comparison parameter.
    /// 
    /// If currentScore is -1 or if it's less than the comparison, the button will be gray
    /// </summary>
    /// <param name="button"></param>
    /// <param name="currentScore"></param>
    /// <param name="comparison"></param>
    void HighlightButton(GameObject button, int currentScore, int comparison, bool allowedToUpgrade)
    {
        if (currentScore < comparison || !allowedToUpgrade)
            SetButtonColor(button, cantAffordColor);
        else
            SetButtonColor(button, Color.white);
    }

    void SetButtonColor(GameObject button, Color color)
    {
        button.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// Sets an upgrade button's text with its cost
    /// </summary>
    /// <param name="button"></param>
    /// <param name="upgradeText"></param>
    /// <param name="cost"></param>
    void SetButtonText(GameObject button, Upgrade upgrade)
    {
        string upgradeCostType = upgrade.costType == CostType.SCORE ? "P" : "X";

        string c = upgrade.cost != int.MaxValue ? $"({upgrade.cost} {upgradeCostType})" : "";
        string l = upgrade.level < upgrade.maxLevel ? $"[{upgrade.level}]" : "[MAXED]";

        button.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{l} {upgrade.upgradeText} {c}";
    }

    public void SetScores(int score, int strikeScore)
    {
        scoreboard.text = score.ToString() + " P";
        strikeScoreboard.text = strikeScore.ToString() + " X";
    }

    public void DisplayBallStats(Lane lane, float goldOdds)
    {
        ballStats.text = "Weight: " + Math.Round(lane.Ball.Weight, 1)
            + "\nAvg Speed: " + Math.Round(lane.Ball.BallSpeed, 1)
            + "\nAccuracy: " + (Math.Round(1 / (lane.Ball.ThrowAngleVariance + 1), 4) * 100) + "%"
            + "\nBall Radius: " + Math.Round(lane.Ball.BallRadius, 2)
            + "\nGold Pin Odds: " + Mathf.Round(goldOdds * 100) + "%"
            + "\n\nRegular Pin Mult: " + lane.regularPinMultiplier
            + "\nStrike Mult: " + lane.strikeMultiplier
            + "\nGold Pin Mult: " + lane.goldMultiplier;
    }

    public void ShowLaunch(float speedOfLaunch)
    {
        launchStats.text = "Ball Speed: " + Math.Round(speedOfLaunch, 2);
    }

    public void UpdatePinDisplay(PinSetter setter)
    {
        pinDisplay.UpdateDisplay(setter.Pins);
    }

    public bool WantsAuto()
    {
        return autoToggle.isOn;
    }
}
