using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    PinDisplay pinDisplay;

    TMP_Text scoreboard;
    TMP_Text ballStats;
    TMP_Text launchStats;

    Toggle autoToggle;

    GameObject speedButton, weightButton, addAccuracyButton, addSizeButton, addCompanionButton, addGoldOddsButton;
    GameObject[] upgradeButtons;

    Color cantAffordColor = new Color(0.6f, 0.6f, 0.6f);

    [SerializeField] bool a;

    // Start is called before the first frame update
    void Start()
    {
        scoreboard = GameObject.Find("Score").GetComponent<TMP_Text>();
        ballStats = GameObject.Find("BallStats").GetComponent<TMP_Text>();
        launchStats = GameObject.Find("LaunchStats").GetComponent<TMP_Text>();

        speedButton = GameObject.Find("AddSpeed").gameObject;
        weightButton = GameObject.Find("AddWeight").gameObject;
        addAccuracyButton = GameObject.Find("AddAccuracy").gameObject;
        addSizeButton = GameObject.Find("AddSize").gameObject;
        addCompanionButton = GameObject.Find("AddCompanion").gameObject;
        addGoldOddsButton = GameObject.Find("AddGoldOdds").gameObject;

        autoToggle = GameObject.Find("AutoToggle").GetComponent<Toggle>();

        pinDisplay = GameObject.Find("PinDisplay").GetComponent<PinDisplay>();

        upgradeButtons = new GameObject[] {
            speedButton,
            weightButton,
            addAccuracyButton,
            addSizeButton,
            addCompanionButton,
            addGoldOddsButton
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateButtonHighlights(List<Upgrade> upgrades, int score, bool allowedToUpgrade)
    {
        if (upgradeButtons.Length != upgrades.Count)
            return;

        for (int i = 0; i < upgradeButtons.Length; i++)
            HighlightButton(upgradeButtons[i], score, upgrades[i].cost, allowedToUpgrade);
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
        string s = upgrade.cost != int.MaxValue ? upgrade.cost.ToString() : "";
        string level = upgrade.level < upgrade.maxLevel ? upgrade.level.ToString() : "MAXED";
        button.transform.GetChild(0).GetComponent<TMP_Text>().text = "[" + level + "] " + upgrade.upgradeText + " (" + s + ")";
    }

    public void SetScore(int score)
    {
        scoreboard.text = score.ToString();
    }

    public void DisplayBallStats(Lane lane, float goldOdds)
    {
        ballStats.text = "Weight: " + Math.Round(lane.Ball.Weight, 1) 
            + "\nAvg Speed: " + Math.Round(lane.Ball.BallSpeed, 1)
            + "\nAccuracy: " + (Math.Round(1/lane.Ball.ThrowAngleVariance, 4) * 100) + "%"
            + "\nBall Radius: " + Math.Round(lane.Ball.BallRadius, 2)
            + "\nGold Pin Odds: " + Mathf.Round(goldOdds * 100) + "%";
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
