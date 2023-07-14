using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsInit : MonoBehaviour
{
    [SerializeField] float movementSpeed = .15f, cameraSpeed = .50f;
    [SerializeField] GameObject movementSpeedGameObject, cameraSpeedGameObject;
    [SerializeField] TextMeshProUGUI movementSpeedValueLabel, cameraSpeedValueLabel;

    private Slider movementSpeedSlider, cameraSpeedSlider;
    void Start()
    {
        movementSpeedSlider = movementSpeedGameObject.GetComponentInChildren<Slider>();
        cameraSpeedSlider = cameraSpeedGameObject.GetComponentInChildren<Slider>();

        movementSpeedSlider.value = movementSpeed;
        cameraSpeedSlider.value = cameraSpeed;

        movementSpeedValueLabel.GetComponent<SliderTextValue>().ChangeValue(movementSpeed);
        cameraSpeedValueLabel.GetComponent<SliderTextValue>().ChangeValue(cameraSpeed);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
