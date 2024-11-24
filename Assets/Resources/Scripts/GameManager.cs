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

    UIManager ui;
    LineRenderer lineRenderer;

    Lane mainLane;
    List<Lane> companionLanes;
    List<Upgrade> upgrades;

    int score = 0;
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
    }

    // Update is called once per frame
    void Update()
    {
        bool inAuto = ui.WantsAuto();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        var mousePlacementRatio = Input.mousePosition / Screen.width;
        bool validCursorPlacement = mousePlacementRatio.x > 0.2f && mousePlacementRatio.x <= 0.8f;

        bool groundCheck = Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")) && hit.collider.CompareTag("Ground") && validCursorPlacement;

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
        if (Input.GetMouseButtonDown(0) && groundCheck && !mainLane.BallInAction && !inAuto)
        {
            ThrowMainBall(hit.point);
            StartCoroutine(StartResetCooldown());
        }
        // resetting the pins
        if (Input.GetMouseButtonDown(1) && mainLane.BallInAction && !onResetCooldown && !inAuto)
        {
            ResetMainThrow();
        }
        // quitting
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // automating your own lane
        if (inAuto)
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
            !mainLane.BallInAction
        );

        ui.UpdateCosts(
            upgrades
        );

        ui.SetScore(score);
        ui.DisplayBallStats(mainLane, goldenOdds);

        ui.UpdatePinDisplay(mainLane.PinSetter);
    }

    void ClaimAllPoints()
    {
        score += mainLane.ClaimPoints();

        foreach (Lane companion in companionLanes)
            score += companion.ClaimPoints();
    }

    void InitializeCompanionBowler()
    {
        var newLane = Instantiate(lane,
                        companionLaneLocations[companionBowlers],
                        Quaternion.identity,
                        this.transform)
            .GetComponent<Lane>();

        newLane.StartAutoBowl();
        newLane.SetBallMaterial(companionBowlers);
        companionLanes.Add(newLane);

        companionBowlers++;
    }

    public void TryUpgrade(string name)
    {
        // you can not upgrade while the ball is in action
        if (mainLane.BallInAction && !ui.WantsAuto()) return;

        var upgrade = upgrades.Find(u => u.name == name);
        upgrade?.ApplyUpgrade(ref score);
    }

    void InitializeUpgrades()
    {
        upgrades = new List<Upgrade>()
        {
            new Upgrade("Speed", "Increase Speed", 20, 100, x => x + 5, () =>
            {
                mainLane.Ball.AddSpeed(0.1f);
            }),
            new Upgrade("Weight", "Increase Weight", 20, 100, x => x + 5, () =>
            {
                mainLane.Ball.AddWeight(0.1f);
            }),
            new Upgrade("Accuracy", "Increase Accuracy", 20, 30, x => x + 10, () =>
            {
                mainLane.Ball.ModifyAngleVariance(-0.1f);
            }),
            new Upgrade("Size", "Increase Ball Size", 50, 50, x => x + 25, () =>
            {
                mainLane.Ball.IncreaseRadius(0.005f);
            }),
            new Upgrade("Companion", "Friend", 100, 2, x => x * 10, () =>
            {
                InitializeCompanionBowler();
            }),
            new Upgrade("GoldOdds", "Gold Pin Rate", 10, 6, x => x * 10, () =>
            {
                goldenOdds *= 2;
            })
        };
    }
}
