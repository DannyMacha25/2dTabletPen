using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed, rotationSpeed;
    Transform tf;

    // Modular Controls
    KeyCode left = KeyCode.A;
    KeyCode back = KeyCode.S;
    KeyCode forward = KeyCode.W;
    KeyCode right = KeyCode.D;

    KeyCode rotLeft = KeyCode.LeftArrow;
    KeyCode rotRight = KeyCode.RightArrow;
    KeyCode panUp = KeyCode.UpArrow;
    KeyCode panDown = KeyCode.DownArrow;

    KeyCode ascend = KeyCode.Space;
    KeyCode descend = KeyCode.LeftShift;
    private const float MAX_X_ROTATION = 25f;
    private void Start()
    {
        tf = this.GetComponent<Transform>();
    }
    private void Update()
    {
        // NOTE: Current movement is frame based, might become a problem?
        // Movement
        if (Input.GetKey(forward))
        {
            tf.position += tf.forward * speed;
        }

        if(Input.GetKey(back))
        {
            tf.position -= tf.forward * speed;
        }

        if(Input.GetKey(right))
        {
            tf.position += tf.right * speed;
        }

        if (Input.GetKey(left))
        {
            tf.position -= tf.right * speed;
        }

        // Rotation
        if (Input.GetKey(panUp))
        {
            tf.eulerAngles = new Vector3(ClampAngle(tf.eulerAngles.x - rotationSpeed, -MAX_X_ROTATION, MAX_X_ROTATION), tf.rotation.eulerAngles.y, tf.rotation.eulerAngles.z);

        }

        if (Input.GetKey(panDown))
        {
            tf.eulerAngles = new Vector3(ClampAngle(tf.eulerAngles.x + rotationSpeed, -MAX_X_ROTATION, MAX_X_ROTATION), tf.rotation.eulerAngles.y, tf.rotation.eulerAngles.z);
        }

        if (Input.GetKey(rotLeft))
        {
            tf.Rotate(new Vector3(0, -rotationSpeed, 0));
        }

        if (Input.GetKey(rotRight))
        {
            tf.Rotate(new Vector3(0, rotationSpeed, 0));
        }

        tf.eulerAngles = new Vector3(tf.eulerAngles.x, tf.eulerAngles.y, 0f);

        // Height

        if(Input.GetKey(ascend))
        {
            tf.position += tf.up * speed;
        }

        if(Input.GetKey(descend))
        {
            tf.position -= tf.up * speed;
        }

    }

    // Thanks whydoidoit from 2014 https://answers.unity.com/questions/659932/how-do-i-clamp-my-rotation.html
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);

        bool inverse = false;
        var tmin = min;
        var tangle = angle;

        if (min > 180)
        {
            inverse = !inverse;
            tmin -= 180;
        }

        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }

        var result = !inverse ? tangle > tmin : tangle < tmin;
        if (!result)
            angle = min;

        inverse = false;
        tangle = angle;
        var tmax = max;

        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }

        if (max > 180)
        {
            inverse = !inverse;
            tmax -= 180;
        }

        result = !inverse ? tangle < tmax : tangle > tmax;
        if (!result)
            angle = max;
        return angle;
    }
}
