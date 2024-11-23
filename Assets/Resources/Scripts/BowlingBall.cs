using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBall : MonoBehaviour
{
    [SerializeField] float ballSpeed;
    [SerializeField] Vector3 startingPoint;

    [SerializeField] float ballPlacementVariance;   // distance in units
    [SerializeField] float throwAngleVariation;     // angle in degrees
    [SerializeField] float speedVariance;           // fraction of the speed

    Rigidbody rb;
    bool canThrow = true;

    public int BallSpeed { get { return Mathf.RoundToInt(ballSpeed); } }
    public int Weight { get { return Mathf.RoundToInt(rb.mass); } }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("VeL " + rb.velocity.magnitude);
        //Debug.Log("Ang " + rb.angularVelocity);
    }

    public void RespawnBall()
    {
        if (!canThrow)
        {
            transform.position = startingPoint + new Vector3(Randomize(ballPlacementVariance), 0);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.rotation = Quaternion.identity;

            canThrow = true;
        }
    }

    public void ThrowBall()
    {
        if (canThrow)
        {
            Vector3 headingTo = new Vector3(0, 0, 4) - transform.position;
            float distance = headingTo.magnitude;
            var direction = headingTo / distance;

            rb.velocity = Quaternion.AngleAxis(Randomize(throwAngleVariation), Vector3.up) *
                direction * (ballSpeed + Random.value * ballSpeed * Randomize(speedVariance));
            // rb.angularVelocity = new Vector3(0, 0.1f, 0.9f) * 100000;

            canThrow = false;

            // drawing a line
            // var lineRenderer = new GameObject("line").AddComponent<LineRenderer>();
            // lineRenderer.startWidth = 0.01f;
            // lineRenderer.positionCount = 2;
            // lineRenderer.useWorldSpace = true;
            // lineRenderer.endColor = Color.white;
            // lineRenderer.startColor = Color.white;

            // lineRenderer.SetPosition(0, transform.position);
            // lineRenderer.SetPosition(1, new Vector3(0, 0.5f, 4));
        }
    }

    // Generates a random value with range [-val, val)
    float Randomize(float val)
    {
        return Random.value * (2 * val) - val;
    }

    public void AddSpeed(int amt)
    {
        if (amt < 0) return;
        this.ballSpeed += amt;
    }

    public void AddWeight(int amt)
    {
        if (amt < 0) return;
        rb.mass += amt;
    }

    public void IncreaseAccuracy()
    {
        throwAngleVariation -= 0.1f;
    }

    public float GetSpeed()
    {
        return rb.velocity.z;
    }
}
