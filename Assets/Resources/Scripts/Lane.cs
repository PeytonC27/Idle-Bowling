using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [SerializeField] GameObject bowlingBallObject;
    [SerializeField] GameObject pinSetterObject;

    BowlingBall ball;
    PinSetter setter;
    GameManager manager;

    bool ballInAction = false;
    float autoResetTime = 6f;

    int scoreQueue = 0;
    int strikeQueue = 0;

    bool autoBowl = false;
    bool queuedAutoBowlChange = false;
    bool locked = false;

    Material redBallMaterial;
    Material blueBallMaterial;

    public int goldMultiplier = 10;
    public int regularPinMultiplier = 1;
    public float strikeMultiplier = 1;

    public BowlingBall Ball { get { return ball; } }    
    public PinSetter PinSetter { get { return setter; } }
    public bool BallInAction {  get { return ballInAction; } }
    public bool AutoBowlOn {  get { return autoBowl; } }


    // Start is called before the first frame update
    void Awake()
    {
        ball = Instantiate(bowlingBallObject, 
            new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z - 8), 
            Quaternion.identity, 
            this.transform).GetComponent<BowlingBall>();
        setter = Instantiate(pinSetterObject,
            new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z + 10),
            Quaternion.identity,
            this.transform).GetComponent<PinSetter>();
        manager = GetComponentInParent<GameManager>();

        setter.SetHeadpinPosition(setter.transform.position);

        setter.AddRows(4);
        ball.RespawnBall();
        setter.ResetPins(manager.goldenOdds);


        redBallMaterial = Resources.Load<Material>("Materials/Visual/Ball/RedBall");
        blueBallMaterial = Resources.Load<Material>("Materials/Visual/Ball/BlueBall");
    }

    // Update is called once per frame
    void Update()
    {
        if (autoBowl && !ballInAction)
        {
            StartCoroutine(AutoBowl());
        }
    }

    public void StartAutoBowl()
    {
        if (ballInAction && !autoBowl && !locked)
        {
            locked = true;
            StartCoroutine(WaitReset());
        }

        autoBowl = true;
        queuedAutoBowlChange = true;
    }

    public void StopAutoBowl()
    {
        queuedAutoBowlChange = false;
    }

    public void ThrowBall(Vector3 location)
    {
        ballInAction = true;
        ball.ThrowBall(location);
    }

    public void ResetThrow()
    {
        scoreQueue += setter.CalculateScore(goldMultiplier, strikeMultiplier, regularPinMultiplier);
        strikeQueue += setter.IsStrike() ? 1 : 0;
        ball.RespawnBall();
        setter.ResetPins(manager.goldenOdds);
        ballInAction = false;
    }

    IEnumerator AutoBowl()
    {
        ballInAction = true;
        ThrowBall(setter.HeadPinPosition);

        yield return new WaitForSeconds(autoResetTime);

        ResetThrow();
        ballInAction = false;
        autoBowl = queuedAutoBowlChange;
        locked = false;
    }

    IEnumerator WaitReset()
    {
        yield return new WaitForSeconds(autoResetTime);

        ResetThrow();
        ballInAction = false;
    }

    public void ClaimPoints(ref int score, ref int strikes)
    {
        if (scoreQueue == 0) return; 

        int temp1 = scoreQueue;
        int temp2 = strikeQueue;
        scoreQueue = 0;
        strikeQueue = 0;

        score += temp1;
        strikes += temp2;
    }

    public void SetBallMaterial(int num)
    {
        switch (num)
        {
            case 0:
                ball.ChangeMaterial(redBallMaterial); break;
            case 1:
                ball.ChangeMaterial(blueBallMaterial); break;
        }
    }
}
