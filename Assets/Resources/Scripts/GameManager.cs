using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject bowlingBallObject;
    [SerializeField] GameObject pinSetterObject;

    BowlingBall ball;
    PinSetter pinSetter;
    bool waiting = false;

    UIManager ui;

    int score = 999;
    int weightCost = 20;
    int speedCost = 20;
    int rowUpgradeCost = 50;
    int autoBowlUpgradeCost = 200;

    bool autoBowl = false;
    bool queueingRowUpgrade = false;


    // Start is called before the first frame update
    void Start()
    {
        ball = Instantiate(bowlingBallObject).GetComponent<BowlingBall>();
        pinSetter = Instantiate(pinSetterObject).GetComponent<PinSetter>();

        ui = GetComponent<UIManager>();

        if (PlayerPrefs.HasKey("Score"))
        {
            score = PlayerPrefs.GetInt("Score");
            ball.AddSpeed(PlayerPrefs.GetInt("Speed") - 6);
            ball.AddWeight(PlayerPrefs.GetInt("Weight") - 6);

            weightCost = PlayerPrefs.GetInt("WeightCost");
            speedCost = PlayerPrefs.GetInt("SpeedCost");
            rowUpgradeCost = PlayerPrefs.GetInt("RowCost");
            autoBowl = PlayerPrefs.GetInt("AutoBowl") == 1;

            int rows = PlayerPrefs.GetInt("Rows");

            // adding new pins and such
            int rowsToAdd = rows - 4;
            pinSetter.AddRows(rowsToAdd);
            ExpandMap(rowsToAdd);

            if (autoBowl)
            {
                autoBowlUpgradeCost = -1;
                ui.DisableThrowButton();
            }
        }
        else
        {
            pinSetter.AddRows(4);
        }

        ball.RespawnBall();
        pinSetter.ResetPins();

        //GameObject.Find("FollowCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>()
        //    .m_Follow = ball.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (autoBowl && !waiting)
        {
            ThrowBall();
        }

        // reset
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("Speed", 6);
            PlayerPrefs.SetInt("Weight", 6);

            PlayerPrefs.SetInt("WeightCost", 20);
            PlayerPrefs.SetInt("SpeedCost", 20);
            PlayerPrefs.SetInt("RowCost", 50);
            PlayerPrefs.SetInt("Rows", 4);
            PlayerPrefs.SetInt("AutoBowl", 0);
            PlayerPrefs.Save();

            Application.Quit();
        }

        // saving and quitting
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.SetInt("Speed", ball.BallSpeed);
            PlayerPrefs.SetInt("Weight", ball.Weight);

            PlayerPrefs.SetInt("WeightCost", weightCost);
            PlayerPrefs.SetInt("SpeedCost", speedCost);
            PlayerPrefs.SetInt("RowCost", rowUpgradeCost);
            PlayerPrefs.SetInt("Rows", pinSetter.Rows);
            PlayerPrefs.SetInt("AutoBowl", autoBowl ? 1 : 0);
            PlayerPrefs.Save();

            Application.Quit();
        }

        UpdateUI();
    }

    public void TryThrowBall()
    {
        if (!autoBowl && !waiting)
        {
            ThrowBall();
        }
    }

    void ThrowBall()
    {
        waiting = true;

        ball.ThrowBall();

        StartCoroutine(Reset());
    }

    void UpdateUI()
    {
        ui.UpdateButtonHighlights(
            score,
            weightCost,
            speedCost,
            rowUpgradeCost,
            autoBowlUpgradeCost
        );

        ui.SetSpeedCost(speedCost);
        ui.SetWeightCost(weightCost);
        ui.SetRowCost(rowUpgradeCost);
        ui.SetAutoBowlCost(autoBowlUpgradeCost);
        ui.SetScore(score);

        ui.DisplayBallStats(ball.BallSpeed, ball.Weight);

        if (autoBowl)
            ui.DisableThrowButton();
    }

    IEnumerator Reset()
    {
        ui.ShowLaunch(ball.GetSpeed());

        yield return new WaitForSeconds(5);

        score += pinSetter.CountPins();

        if (queueingRowUpgrade)
        {
            pinSetter.AddRows(1);
            ExpandMap(1);
            queueingRowUpgrade = false;
        }

        ball.RespawnBall();
        pinSetter.ResetPins();
        waiting = false;
    }

    public void BuySpeedUpgrade()
    {
        if (score < speedCost || (waiting && !autoBowl))
            return;

        score -= speedCost;
        speedCost += 5;
        ball.AddSpeed(1);
    }

    public void BuyWeightUpgrade()
    {
        if (score < weightCost || (waiting && !autoBowl))
            return;

        score -= weightCost;
        weightCost += 5;
        ball.AddWeight(1);
    }

    public void BuyRowUpgrade()
    {
        if (score < rowUpgradeCost || (waiting && !autoBowl) || queueingRowUpgrade)
            return;

        score -= rowUpgradeCost;
        rowUpgradeCost = (pinSetter.Rows + 1) * (pinSetter.Rows + 2) / 2 * 10;
        queueingRowUpgrade = true;
    }

    public void BuyAutoBowl()
    {
        if (score < autoBowlUpgradeCost || waiting)
            return;

        score -= autoBowlUpgradeCost;
        autoBowlUpgradeCost = -1;
        autoBowl = true;

        ui.DisableThrowButton();
    }

    void ExpandMap(int amt)
    {
        transform.Find("WallR").position += new Vector3(0.2f * amt, 0);
        transform.Find("WallL").position -= new Vector3(0.2f * amt, 0);
    }
}
