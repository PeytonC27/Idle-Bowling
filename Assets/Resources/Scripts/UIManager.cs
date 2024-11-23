using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    TMP_Text scoreboard;
    TMP_Text ballStats;
    TMP_Text launchStats;

    GameObject weightButton;
    GameObject speedButton;
    GameObject addRowButton;
    GameObject autoBowlButton;
    GameObject throwButton;

    Color cantAffordColor = new Color(0.6f, 0.6f, 0.6f);

    // Start is called before the first frame update
    void Start()
    {
        var canv = transform.Find("UI");

        scoreboard = canv.Find("Score").GetComponent<TMP_Text>();
        ballStats = canv.Find("BallStats").GetComponent<TMP_Text>();
        launchStats = canv.Find("LaunchStats").GetComponent<TMP_Text>();

        weightButton = canv.Find("AddWeight").gameObject;
        speedButton = canv.Find("AddSpeed").gameObject;
        addRowButton = canv.Find("AddRow").gameObject;
        autoBowlButton = canv.Find("AddAutoBowl").gameObject;
        throwButton = canv.Find("ThrowButton").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateButtonHighlights(int score, int weightCost, int speedCost, int addRowCost, int autoBowlCost)
    {
        HighlightButton(weightButton, score, weightCost);
        HighlightButton(speedButton, score, speedCost);
        HighlightButton(addRowButton, score, addRowCost);
        HighlightButton(autoBowlButton, score, autoBowlCost);
    }

    /// <summary>
    /// Highlights a button based on the score parameter compared to the comparison parameter.
    /// 
    /// If currentScore is -1 or if it's less than the comparison, the button will be gray
    /// </summary>
    /// <param name="button"></param>
    /// <param name="currentScore"></param>
    /// <param name="comparison"></param>
    void HighlightButton(GameObject button, int currentScore, int comparison)
    {
        if (currentScore < comparison || comparison == -1)
            SetButtonColor(button, cantAffordColor);
        else
            SetButtonColor(button, Color.white);
    }

    void SetButtonColor(GameObject button, Color color)
    {
        button.GetComponent<Image>().color = color;
    }

    public void SetWeightCost(int cost)
    {
        SetButtonText(weightButton, "Increase Weight", cost);
    }

    public void SetSpeedCost(int cost)
    {
        SetButtonText(speedButton, "Increase Speed", cost);
    }

    public void SetRowCost(int cost)
    {
        SetButtonText(addRowButton, "Add Extra Pins", cost);
    }

    public void SetAutoBowlCost(int cost)
    {
        SetButtonText(autoBowlButton, "Unlock Auto-Bowl", cost);
    }

    /// <summary>
    /// Sets an upgrade button's text with its cost
    /// </summary>
    /// <param name="button"></param>
    /// <param name="upgradeText"></param>
    /// <param name="cost"></param>
    void SetButtonText(GameObject button, string upgradeText, int cost)
    {
        string s = cost != -1 ? cost.ToString() : "MAX";
        button.transform.GetChild(0).GetComponent<TMP_Text>().text = upgradeText + " (" + s + ")";
    }

    public void SetScore(int score)
    {
        scoreboard.text = score.ToString();
    }

    public void DisplayBallStats(int speed, int weight)
    {
        ballStats.text = "Weight: " + weight + "\nAvg Speed: " + speed;
    }

    public void ShowLaunch(float speedOfLaunch)
    {
        launchStats.text = "Ball Speed: " + Math.Round(speedOfLaunch, 2);
    }

    public void DisableThrowButton()
    {
        if (throwButton != null)
            throwButton.GetComponent<Button>().interactable = false;
    }
}
