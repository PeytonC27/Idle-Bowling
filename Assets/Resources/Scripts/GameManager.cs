using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject bowlingBallObject;
    [SerializeField] GameObject pinSetterObject;

    BowlingBall ball;
    PinSetter pinSetter;
    bool waiting = false;

    UIManager ui;
    List<Upgrade> upgrades;

    int score = 999;

    bool autoBowl = false;
    int queuedRowUpgrades = 0;


    // Start is called before the first frame update
    void Start()
    {
        ball = Instantiate(bowlingBallObject).GetComponent<BowlingBall>();
        pinSetter = Instantiate(pinSetterObject).GetComponent<PinSetter>();
        ui = GetComponent<UIManager>();

        InitializeUpgrades();
        pinSetter.AddRows(4);

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
            upgrades,
            score
        );

        ui.UpdateCosts(
            upgrades
        );

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

        if (queuedRowUpgrades > 0)
        {
            pinSetter.AddRows(queuedRowUpgrades);
            ExpandMap(queuedRowUpgrades);
            queuedRowUpgrades = 0;
        }

        ball.RespawnBall();
        pinSetter.ResetPins();
        waiting = false;
    }

    public void TryUpgrade(string name)
    {
        var upgrade = upgrades.Find(u => u.name == name);
        upgrade?.ApplyUpgrade(ref score);
    }

    void ExpandMap(int amt)
    {
        transform.Find("WallR").position += new Vector3(0.2f * amt, 0);
        transform.Find("WallL").position -= new Vector3(0.2f * amt, 0);
    }

    void InitializeUpgrades()
    {
        upgrades = new List<Upgrade>()
        {
            new Upgrade("Speed", "Increase Speed", 20, x => x + 5, () =>
            {
                ball.AddSpeed(1);
            }),
            new Upgrade("Weight", "Increase Weight", 20, x => x + 5, () =>
            {
                ball.AddWeight(1);
            }),
            new Upgrade("Row", "Add Extra Pins", 100, x => (int) (1.5 * x + x), () =>
            {
                queuedRowUpgrades++;
            }),
            new Upgrade("AutoBowl", "Unlock Auto-Bowl", 250, x => int.MaxValue, () =>
            {
                autoBowl = true;
                ui.DisableThrowButton();
            })
        };
    }
}
