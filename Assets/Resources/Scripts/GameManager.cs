using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject bowlingBallObject;
    [SerializeField] GameObject pinSetterObject;

    LineRenderer lineRenderer;
    BowlingBall ball;
    PinSetter pinSetter;
    bool waiting = false;

    UIManager ui;
    List<Upgrade> upgrades;

    int score = 0;
    bool autoBowl = false;
    float goldenOdds = 0.01f;

    float resetCooldownDuration = 3f;
    bool onResetCooldown = false;


    // Start is called before the first frame update
    void Start()
    {
        ball = Instantiate(bowlingBallObject).GetComponent<BowlingBall>();
        pinSetter = Instantiate(pinSetterObject).GetComponent<PinSetter>();
        ui = GameObject.Find("UI").GetComponent<UIManager>();

        InitializeUpgrades();
        pinSetter.AddRows(4);
        ball.RespawnBall();
        pinSetter.ResetPins(goldenOdds);

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.endColor = Color.white;
        lineRenderer.startColor = Color.black;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    // Update is called once per frame
    void Update()
    {
        if (autoBowl && !waiting)
        {
            ThrowBall(pinSetter.HeadPinPosition);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        var mousePlacementRatio = Input.mousePosition / Screen.width;
        bool validCursorPlacement = mousePlacementRatio.x > 0.2f && mousePlacementRatio.x <= 0.8f;

        bool groundCheck = Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")) && hit.collider.CompareTag("Ground") && validCursorPlacement;

        // drawing the line
        if (groundCheck && !waiting)
        {
            lineRenderer.enabled = true;
            Vector3 headingTo = hit.point - ball.Position;
            var endPoint = ball.Position + headingTo.normalized * Mathf.Min(headingTo.magnitude, 3);
            endPoint.y = 0.5f;

            lineRenderer.SetPosition(0, ball.Position);
            lineRenderer.SetPosition(1, endPoint);
        }
        else
        {
            lineRenderer.enabled = false;
        }
        // throwing the ball
        if (Input.GetMouseButtonDown(0) && groundCheck && !waiting)
        {
            ThrowBall(hit.point);
            StartCoroutine(StartResetCooldown());
        }
        // resetting the pins
        if (Input.GetMouseButtonDown(1) && waiting && !onResetCooldown)
        {
            ResetThrow();
        }
        // quitting
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        UpdateUI();
    }

    IEnumerator StartResetCooldown()
    {
        onResetCooldown = true;
        yield return new WaitForSeconds(resetCooldownDuration);
        onResetCooldown = false;
    }

    void ThrowBall(Vector3 location)
    {
        waiting = true;

        ball.ThrowBall(location);
        ui.ShowLaunch(ball.GetSpeed());
        lineRenderer.enabled = false;
    }

    void ResetThrow()
    {
        score += pinSetter.CountPins();

        ball.RespawnBall();
        pinSetter.ResetPins(goldenOdds);
        waiting = false;
        lineRenderer.enabled = true;
    }

    void UpdateUI()
    {
        ui.UpdateButtonHighlights(
            upgrades,
            score,
            !waiting
        );

        ui.UpdateCosts(
            upgrades
        );

        ui.SetScore(score);
        ui.DisplayBallStats(ball);

        ui.UpdatePinDisplay(pinSetter);
    }

    public void TryUpgrade(string name)
    {
        // you can not upgrade while the ball is in action
        if (waiting) return;

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
            new Upgrade("Speed", "Increase Speed", 20, 100, x => x + 5, () =>
            {
                ball.AddSpeed(0.1f);
            }),
            new Upgrade("Weight", "Increase Weight", 20, 100, x => x + 5, () =>
            {
                ball.AddWeight(0.1f);
            }),
            new Upgrade("Accuracy", "Increase Accuracy", 20, 30, x => x + 10, () =>
            {
                ball.ModifyAngleVariance(-0.1f);
            }),
            new Upgrade("Size", "Increase Ball Size", 50, 50, x => (int)(x * 1.5), () =>
            {
                ball.IncreaseRadius(0.005f);
            }),
            new Upgrade("Row", "Add Extra Pins", 250, 11, x => x * 2, () =>
            {
                pinSetter.AddRows(1);
                ExpandMap(1);
            }),
        };
    }
}
