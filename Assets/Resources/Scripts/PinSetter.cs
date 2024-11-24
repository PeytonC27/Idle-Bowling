using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinSetter : MonoBehaviour
{
    [SerializeField] int rows = 4;
    [SerializeField] Vector3 headPinPosition;
    [SerializeField] GameObject pinPrefab;

    Pin[] pins;

    public int Rows { get { return rows; } }
    public Vector3 HeadPinPosition { get { return headPinPosition; }  }
    public Pin[] Pins { get { return pins; } }

    // Update is called once per frame
    void Update()
    {
    }

    public void ResetPins(float goldenOdds)
    {
        foreach (var pin in pins)
        {
            pin.RespawnPin(goldenOdds);
        }
    }

    public int CountPins()
    {
        int count = 0;
        int score = 0;
        foreach (var pin in pins)
        {
            if (pin.IsDown())
            {
                if (pin.type == PinType.GOLDEN)
                    score += 10;
                else
                    score += 1;
                count++;
            }

        }

        if (count == pins.Length)
            return score * 2;

        return score;
    }

    public void AddRows(int amt)
    {
        this.rows += amt;
        DestroyChildren();
        CalculatePinPlacement();
    }

    void DestroyChildren()
    {
        if (pins == null)
            return;

        foreach (var pin in pins)
        {
            Destroy(pin.gameObject);
        }
    }

    void CalculatePinPlacement()
    {
        int pinCount = (rows * (rows + 1)) / 2;
        pins = new Pin[pinCount];

        Vector3 currentSpawnPos = headPinPosition;
        Vector3 rowSpawnPos = headPinPosition;
        int index = 0;

        // right triangle values used via bowling pin setup (equaliateral triangle, all side lens 12)
        // b = 6, c = 12, a = 10.3923 or 6 * sqrt(3)
        // SCALED DOWN BY 30
        // x = 0.2, dist = 0.4, z = sqrt(3) / 5
        float hDist = 0.4f;                 // distance between pins horizontally
        float vDist = Mathf.Sqrt(3) / 5.0f; // distance between rows vertically


        for (int i = 1; i <= rows; i++)
        {
            for (int j = 0; j < i; j++)
            {
                pins[index] = Instantiate(pinPrefab, currentSpawnPos, Quaternion.identity, this.transform).GetComponent<Pin>();
                pins[index].SetSpawnLocation(currentSpawnPos);

                index++;

                currentSpawnPos += new Vector3(hDist, 0, 0);
            }


            // essentially, move the current spawn position to the next row, and save the
            // first pin of the next row to continue the loop backwards
            currentSpawnPos = rowSpawnPos + new Vector3(-(hDist / 2), 0, vDist);
            rowSpawnPos = currentSpawnPos;
        }

        Debug.Log("set");
    }
}
