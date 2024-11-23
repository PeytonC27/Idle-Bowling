using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    Vector3 baseLocation;
    Rigidbody rb;

    public bool down;
    public float ang;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        down = IsDown();
        ang = Mathf.Abs(transform.eulerAngles.x);
    }

    public void RespawnPin()
    {
        transform.position = baseLocation;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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
