using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    Vector3 baseLocation;
    Rigidbody rb;
    Renderer renderer;
    PinType pinType;

    public bool down;
    public float ang;

    Material goldenMaterial;
    Material basicMaterial;

    public PinType type { get { return pinType; } }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();

        goldenMaterial = Resources.Load<Material>("Materials/Visual/Golden");
        basicMaterial = Resources.Load<Material>("Materials/Visual/BasicPin");
    }

    private void Update()
    {
        down = IsDown();
        ang = Mathf.Abs(transform.eulerAngles.x);
    }

    public void RespawnPin(float goldenOdds)
    {
        transform.position = baseLocation;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (Random.value < goldenOdds)
        {
            renderer.material = goldenMaterial;
            pinType = PinType.GOLDEN;
        }
        else
        {
            renderer.material = basicMaterial;
            pinType= PinType.BASIC;
        }
    }

    public void SetSpawnLocation(Vector3 location)
    {
        this.baseLocation = location;
    }

    public bool IsDown()
    {

        bool angleCheck = ValidFallAngle(Mathf.Abs(transform.eulerAngles.x)) || ValidFallAngle(Mathf.Abs(transform.eulerAngles.z));
        bool distCheck = Vector3.Distance(transform.position, this.baseLocation) > 1;

        return angleCheck || distCheck;
    }

    bool ValidFallAngle(float angle)
    {
        return angle > 30 && angle < 330;
    }
}

public enum PinType
{
    BASIC,
    GOLDEN
}