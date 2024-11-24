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
    float autoResetTime = 10f;

    int scoreQueue = 0;

    bool autoBowl = false;

    Material redBallMaterial;
    Material blueBallMaterial;

    public BowlingBall Ball { get { return ball; } }    
    public PinSetter PinSetter { get { return setter; } }
    public bool BallInAction {  get { return ballInAction; } }


    // Start is called before the first frame update
    void Awake()
    {
        ball = Instantiate(bowlingBallObject, 
            new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z - 8), 
            Quaternion.identity, 
            this.transform).GetComponent<BowlingBall>();
        setter = Instantiate(pinSetterObject,
            new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z + 15),
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
        autoBowl = true;
    }

    public void StopAutoBowl()
    {
        autoBowl = false;
    }

    public void ThrowBall(Vector3 location)
    {
        ballInAction = true;
        ball.ThrowBall(location);
    }

    public void ResetThrow()
    {
        scoreQueue += setter.CountPins();
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
    }

    public int ClaimPoints()
    {
        if (scoreQueue == 0) return 0; 

        int temp = scoreQueue;
        scoreQueue = 0;
        return temp;
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
