using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PenInput : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
        if (Pen.current.tip.wasPressedThisFrame)
        {
            Debug.Log("Pressed this frame");
        }

        if (Pen.current.tip.isPressed)
        {
            Debug.Log("Pressed");
            Vector2 pos = Pen.current.position.ReadValue();
            Debug.Log(pos);
        }

        Debug.Log(Pen.current.displayName);
    }
}
