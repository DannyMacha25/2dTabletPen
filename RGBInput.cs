using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RGBInput : MonoBehaviour
{

    private Color color;
    [SerializeField] GameObject panel, rSlider, gSlider, bSlider;
    [SerializeField] Image p;
    private float r, g, b;

    // Update is called once per frame
    void Update()
    {
        color = new Color(r/255, g/255, b/255, 1);
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

    public void UpdateColor(float fR, float fG, float fB)
    {
        r = fR * 255; g = fG * 255; b = fB * 255;
        rSlider.GetComponent<UnityEngine.UI.Slider>().value = r;
        gSlider.GetComponent<UnityEngine.UI.Slider>().value = g;
        bSlider.GetComponent<UnityEngine.UI.Slider>().value = b;
    }

    public void UpdateColor(Color c)
    {
        UpdateColor(c.r, c.g, c.b);

    }

    public Color Color()
    {
        return color;
    }
}
