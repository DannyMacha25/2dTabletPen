using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderTextValue : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeValue(float value)
    {
        _textMesh.text = (value * 10).ToString("0.00");
    }

    public void ChangeText(string s)
    {
        _textMesh.text = s;
    }

    public void ChangeText(int n)
    {
        _textMesh.text = n.ToString();
    }

    public void ChangeTextFromSliderToInt(float f)
    {
        ChangeText((int)f);
    }


}
