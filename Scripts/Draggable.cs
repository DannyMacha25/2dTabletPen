using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Draggable : MonoBehaviour
{
    [SerializeField] KeyCode activationKey = KeyCode.LeftControl;
    [SerializeField] KeyCode deletionKey = KeyCode.X;

    [SerializeField] KeyCode rotLeft = KeyCode.LeftArrow;
    [SerializeField] KeyCode rotRight = KeyCode.RightArrow;
    [SerializeField] KeyCode panUp = KeyCode.UpArrow;
    [SerializeField] KeyCode panDown = KeyCode.DownArrow;

    [SerializeField] float speed = .50f;

    private Vector3 mOffset;
    private float mZCoord;
    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    private void OnMouseDrag() // Must make work for pen too
    {
        if (Input.GetKey(activationKey)) 
        {
            transform.position = GetMouseWorldPos() + mOffset;
        }
    }

    private void Update()
    {
        if (Pen.current.tip.isPressed) // Not fully functional
        {
            transform.position = GetMouseWorldPos() + mOffset;
        } 
    }

    private void OnMouseOver()
    {
        if (!Input.GetKey(activationKey)) { return; }

        if (Input.GetKey(deletionKey))
        {
            Destroy(this.gameObject);
        }   

        // Rotation
        if (Input.GetKey(rotLeft))
        {
            this.transform.Rotate(-Vector3.up * speed);
        }

        if (Input.GetKey(rotRight))
        {
            this.transform.Rotate(Vector3.up * speed);
        }

        // Scale
        if (Input.GetKey(KeyCode.UpArrow))
        {
            var scale = this.transform.localScale;
            this.transform.localScale = new Vector3(scale.x + .05f, scale.y, scale.z);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            var scale = this.transform.localScale;
            this.transform.localScale = new Vector3(scale.x - .05f, scale.y, scale.z);
        }
    }
}
