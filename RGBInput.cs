using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RGBInput : MonoBehaviour
{

    private Color color;
    [SerializeField] GameObject panel;
    [SerializeField] Image p;
    private float r, g, b;

    // Update is called once per frame
    void Update()
    {
        color = new Color(r/255, g/255, b/255, 1);
        Debug.Log(r + " " + g + " " + b);
        panel.GetComponent<UnityEngine.UI.Image>().color = color;
    }

    public void UpdateR(float f)
    {
        r = f;
    }

    public void UpdateG(float f)
    {
        g = f;
    }

    public void UpdateB(float f)
    {
        b = f;
    }

    public Color Color()
    {
        return color;
    }
}
