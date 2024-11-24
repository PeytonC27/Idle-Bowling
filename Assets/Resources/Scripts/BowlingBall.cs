using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBall : MonoBehaviour
{
    [SerializeField] float ballSpeed;

    [SerializeField] float ballPlacementVariance;   // distance in units
    [SerializeField] float throwAngleVariation;     // angle in degrees
    [SerializeField] float speedVariance;           // fraction of the speed

    Renderer ballRenderer;

    Vector3 baseLocation;
    Rigidbody rb;
    bool canThrow = true;
    float radius;

    public float BallSpeed { get { return ballSpeed; } }
    public float Weight { get { return rb.mass; } }
    public float ThrowAngleVariance { get { return throwAngleVariation; } }

    public Vector3 Position { get { return transform.position; } }

    public float BallRadius { get { return radius; } }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        radius = transform.localScale.x;
        baseLocation = transform.position;

        ballRenderer = GetComponent<Renderer>();
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
            transform.position = baseLocation + new Vector3(Randomize(ballPlacementVariance), 0);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.rotation = Quaternion.identity;

            canThrow = true;
        }
    }

    public void ThrowBall(Vector3 dir)
    {
        if (canThrow)
        {
            Vector3 headingTo = dir - Position;
            float distance = headingTo.magnitude;
            var direction = headingTo / distance;

            rb.velocity = Quaternion.AngleAxis(Randomize(throwAngleVariation), Vector3.up) *
                direction * (ballSpeed + ballSpeed * Randomize(speedVariance));
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

    public void AddSpeed(float amt)
    {
        if (amt < 0) return;
        this.ballSpeed += amt;
    }

    public void AddWeight(float amt)
    {
        if (amt < 0) return;
        rb.mass += amt;
    }

    public void ModifyAngleVariance(float amt)
    {
        throwAngleVariation += amt;
    }

    public void IncreaseRadius(float amt)
    {
        radius += amt;
        gameObject.transform.localScale = new Vector3(radius, radius, radius);
    }

    public float GetSpeed()
    {
        return rb.velocity.z;
    }

    public void ChangeMaterial(Material material)
    {
        ballRenderer.material = material;
    }
}
