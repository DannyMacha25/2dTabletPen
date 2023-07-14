using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderTextValue : MonoBehaviour
{
    private float _value;
    private TextMeshProUGUI _textMesh;
    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _textMesh.text = (_value * 10).ToString("0.00");
    }

    public void ChangeValue(float value)
    {
        _value = value;
    }


}
