using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed, rotationSpeed;
    Transform tf;

    private const float MAX_X_ROTATION = 25f;
    private void Start()
    {
        tf = this.GetComponent<Transform>();
    }
    private void Update()
    {
        // NOTE: Current movement is frame based, might become a problem?
        // Movement
        if (Input.GetKey(KeyCode.W))
        {
            tf.position += tf.forward * speed;
        }

        if(Input.GetKey(KeyCode.S))
        {
            tf.position -= tf.forward * speed;
        }

        if(Input.GetKey(KeyCode.D))
        {
            tf.position += tf.right * speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            tf.position -= tf.right * speed;
        }

        // Rotation
        if (Input.GetKey(KeyCode.UpArrow))
        {
            tf.eulerAngles = new Vector3(ClampAngle(tf.eulerAngles.x - rotationSpeed, -MAX_X_ROTATION, MAX_X_ROTATION), tf.rotation.eulerAngles.y, tf.rotation.eulerAngles.z);

        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            tf.eulerAngles = new Vector3(ClampAngle(tf.eulerAngles.x + rotationSpeed, -MAX_X_ROTATION, MAX_X_ROTATION), tf.rotation.eulerAngles.y, tf.rotation.eulerAngles.z);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            tf.Rotate(new Vector3(0, -rotationSpeed, 0));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            tf.Rotate(new Vector3(0, rotationSpeed, 0));
        }

        tf.eulerAngles = new Vector3(tf.eulerAngles.x, tf.eulerAngles.y, 0f);

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
