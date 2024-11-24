using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinDisplay : MonoBehaviour
{
    Image[] displays;
    Color gold = new Color(1, 0.678f, 0);
    Color goldDown = new Color(145f / 255, 109f / 255, 34f / 255);

    Color basic = Color.white;
    Color basicDown = new Color(0.6f, 0.6f, 0.6f);

    // Start is called before the first frame update
    void Start()
    {
        displays = new Image[10];

        for (int i = 0; i < displays.Length; i++)
        {
            displays[i] = transform.Find("PinShow" + (i+1)).GetComponent<Image>();
        }
    }

    public void UpdateDisplay(Pin[] pins)
    {
        for (int i = 0;i < pins.Length;i++)
        {
            var pin = pins[i];
            switch (pins[i].type)
            {
                case PinType.BASIC:
                    displays[i].color = pin.IsDown() ? basicDown : basic; 
                    break;
                case PinType.GOLDEN:
                    displays[i].color = pin.IsDown() ? goldDown : gold; 
                    break;
            }
        }
    }
}
