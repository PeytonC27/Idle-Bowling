using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject lane;

    [SerializeField]
    Vector3[] companionLaneLocations;

    [SerializeField] float validThrowMousePlacementThreshold;

    UIManager ui;
    LineRenderer lineRenderer;

    Lane mainLane;
    List<Lane> companionLanes;
    List<Upgrade> upgrades;

    int score = 0;
    int strikeScore = 0;
    public float goldenOdds = 0.01f;

    float resetCooldownDuration = 3f;
    bool onResetCooldown = false;

    int companionBowlers = 0;



    // Start is called before the first frame update
    void Start()
    {
        mainLane = GameObject.Find("MainLane").GetComponent<Lane>();
        ui = GameObject.Find("UI").GetComponent<UIManager>();

        InitializeUpgrades();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.endColor = Color.white;
        lineRenderer.startColor = Color.black;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        companionLanes = new();
        ui.SetupButtonUI(upgrades, TryUpgrade);

        for (int i = 0; i < companionLaneLocations.Length; i++)
        {
            var newLane = Instantiate(lane,
                        companionLaneLocations[i],
                        Quaternion.identity,
                        this.transform)
            .GetComponent<Lane>();

            newLane.enabled = false;
            newLane.SetBallMaterial(i);
            companionLanes.Add(newLane);
            
        }
    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        var mousePlacement = Input.mousePosition;
        var screenCenter = Screen.width / 2f;

        bool validCursorPlacement = mousePlacement.x > screenCenter - 574 && mousePlacement.x < screenCenter + 574;

        bool groundCheck = Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")) && hit.collider.CompareTag("Ground") && validCursorPlacement;

        Debug.Log(mousePlacement);

        // drawing the line
        if (groundCheck && !mainLane.BallInAction)
        {
            lineRenderer.enabled = true;
            Vector3 headingTo = hit.point - mainLane.Ball.Position;
            var endPoint = mainLane.Ball.Position + headingTo.normalized * Mathf.Min(headingTo.magnitude, 3);
            endPoint.y = 0.5f;

            lineRenderer.SetPosition(0, mainLane.Ball.Position);
            lineRenderer.SetPosition(1, endPoint);
        }
        else
        {
            lineRenderer.enabled = false;
        }
        // throwing the ball
        if (Input.GetMouseButtonDown(0) && groundCheck && !mainLane.BallInAction && !mainLane.AutoBowlOn)
        {
            ThrowMainBall(hit.point);
            StartCoroutine(StartResetCooldown());
        }
        // resetting the pins
        if (Input.GetMouseButtonDown(1) && mainLane.BallInAction && !onResetCooldown && !mainLane.AutoBowlOn)
        {
            ResetMainThrow();
        }
        // quitting
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // automating your own lane
        if (ui.WantsAuto())
            mainLane.StartAutoBowl();
        else
            mainLane.StopAutoBowl();

        UpdateUI();
        ClaimAllPoints();
    }

    IEnumerator StartResetCooldown()
    {
        onResetCooldown = true;
        yield return new WaitForSeconds(resetCooldownDuration);
        onResetCooldown = false;
    }

    void ThrowMainBall(Vector3 location)
    {
        mainLane.ThrowBall(location);
        ui.ShowLaunch(mainLane.Ball.GetSpeed());
        lineRenderer.enabled = false;
    }

    void ResetMainThrow()
    {
        mainLane.ResetThrow();
        lineRenderer.enabled = true;
    }

    void UpdateUI()
    {
        ui.UpdateButtonHighlights(
            upgrades,
            score,
            strikeScore,
            !mainLane.BallInAction || mainLane.AutoBowlOn
        );

        ui.UpdateCosts(
            upgrades
        );

        ui.SetScores(score, strikeScore);
        ui.DisplayBallStats(mainLane, goldenOdds);

        ui.UpdatePinDisplay(mainLane.PinSetter);
    }

    void ClaimAllPoints()
    {
        mainLane.ClaimPoints(ref score, ref strikeScore);

        foreach (Lane companion in companionLanes)
            companion.ClaimPoints(ref score, ref strikeScore);
    }

    void StartCompanionBowler()
    {
        var newLane = companionLanes[companionBowlers];

        newLane.enabled = true;
        newLane.StartAutoBowl();

        companionBowlers++;
    }

    public void TryUpgrade(string name)
    {
        // you can not upgrade while the ball is in action
        if (mainLane.BallInAction && !ui.WantsAuto()) return;

        var upgrade = upgrades.Find(u => u.name == name);

        if (upgrade.costType == CostType.SCORE)
            upgrade?.ApplyUpgrade(ref score);
        else
            upgrade?.ApplyUpgrade(ref strikeScore);
    }

    void ApplyUpgradeToLanes(Action<Lane> action)
    {
        action.Invoke(mainLane);
        foreach (Lane companion in companionLanes)
            action.Invoke(companion);
    }

    void InitializeUpgrades()
    {
        upgrades = new List<Upgrade>()
        {
            new Upgrade("Speed", "Increase Speed", CostType.SCORE, 20, 100, x => x + 5, () =>
            {
                mainLane.Ball.AddSpeed(0.1f);
            }),
            new Upgrade("Weight", "Increase Weight", CostType.SCORE, 20, 100, x => x + 5, () =>
            {
                mainLane.Ball.AddWeight(0.1f);
            }),
            new Upgrade("Accuracy", "Increase Accuracy", CostType.SCORE, 20, 30, x => x + 10, () =>
            {
                mainLane.Ball.ModifyAngleVariance(-0.1f);
            }),
            new Upgrade("Size", "Increase Ball Size", CostType.SCORE, 50, 100, x => x + 25, () =>
            {
                mainLane.Ball.IncreaseRadius(0.005f);
            }),
            new Upgrade("Companion", "Friend", CostType.SCORE, 100, 2, x => x * 10, () =>
            {
                StartCompanionBowler();
            }),
            new Upgrade("GoldOdds", "Gold Pin Rate", CostType.SCORE, 10, 6, x => x * 5, () =>
            {
                goldenOdds *= 2;
            }),
            new Upgrade("GoldPinMult", "Gold Pin Mult.", CostType.STRIKES, 1, 100, x => x * 2, () =>
            {
                ApplyUpgradeToLanes(lane => lane.goldMultiplier *= 2);
            }),
            new Upgrade("StrikeScoreMult", "Strike Score Mult.", CostType.STRIKES, 1, 100, x => x + 5, () =>
            {
                ApplyUpgradeToLanes(lane => lane.strikeMultiplier += 0.5f);
            }),
            new Upgrade("ExplosivePin", "Unlock Explosive Pin", CostType.STRIKES, 1000, 1, x => int.MaxValue, () =>
            {
                Debug.Log("Yay!");
            }),
            new Upgrade("PinMult", "Regular Pin Mult.", CostType.STRIKES, 1, 100, x => x + 2, () =>
            {
                ApplyUpgradeToLanes(lane => lane.regularPinMultiplier += 1);
            })
        };
    }
}

public enum CostType
{
    SCORE,
    STRIKES
}
