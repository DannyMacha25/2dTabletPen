using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Runtime.CompilerServices;

public class Marker : MonoBehaviour
{
    enum Tool
    {
        Pen,
        Eraser,
        ColorPicker
    }

    [SerializeField] private int _penSize = 5;
    [SerializeField] private Color _color;
    [SerializeField] private RGBInput _colorInput;
    [SerializeField] bool _acceptMouseInput = false;


    private Color[] _colors;
    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    private Plane _plane = new Plane(Vector3.up, 0);

    private Tool _currentTool = Tool.Pen;
    void Start()
    {
        //_renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();
    }


    void Update()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        ChangeColor(_colorInput.Color());

        // Pen size
        if (Keyboard.current.leftBracketKey.wasPressedThisFrame)
        {
            ChangePenSize(-1);
        }

        if (Keyboard.current.rightBracketKey.wasPressedThisFrame)
        {
            ChangePenSize(1);
        }

        // Change Tool
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            _currentTool = Tool.Pen;
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            _currentTool = Tool.Eraser;
        }
        //var worldPos = Camera.main.ScreenToWorldPoint(pos);
        switch(_currentTool)
        {
            case Tool.Pen:
                Draw();  break;

            case Tool.Eraser:
                Erase(); break;
        }
    }

    private void ChangeColor(Color c)
    {
        _color = c;
        _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();
    }
    private void ChangePenSize(int change)
    {
        _penSize += change;
        _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();

        Debug.Log(_penSize);
    }
    private void Draw()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 worldPos = new Vector3(0, 0, 0);
        if (Pen.current.tip.isPressed || (_acceptMouseInput && Pointer.current.press.isPressed))
        {
            RaycastHit hitData;
            pos = Pen.current.position.ReadValue();
            if (_acceptMouseInput)
            {
                pos = Pointer.current.position.ReadValue();
            }
            pos.z = 1;
            var ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hitData, 1000))
            {
                worldPos = hitData.point;
            }

        }
        if (worldPos != Vector3.zero)
        {
            worldPos.z -= .1f;
            Debug.DrawRay(worldPos, Vector3.forward, Color.green, 100f);
        }

        if (Physics.Raycast(worldPos, Vector3.forward, out _touch, 10f) && worldPos != Vector3.zero)
        {

            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_penSize / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_penSize / 2));

                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x)
                {
                    return;
                }

                if (_touchedLastFrame)
                {
                    _whiteboard.texture.SetPixels(x, y, _penSize, _penSize, _colors);

                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);

                        // Set pixels
                        _whiteboard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                    }

                    //transform.rotation = _lastTouchRot;

                    _whiteboard.texture.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }

        _whiteboard = null;
        _touchedLastFrame = false;
    }

    private void Erase()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 worldPos = new Vector3(0, 0, 0);
        if (Pen.current.tip.isPressed || (_acceptMouseInput && Pointer.current.press.isPressed))
        {
            RaycastHit hitData;
            pos = Pen.current.position.ReadValue();
            if (_acceptMouseInput)
            {
                pos = Pointer.current.position.ReadValue();
            }
            pos.z = 1;
            var ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hitData, 1000))
            {
                worldPos = hitData.point;
            }

        }
        if (worldPos != Vector3.zero)
        {
            worldPos.z -= .1f;
            Debug.DrawRay(worldPos, Vector3.forward, Color.green, 100f);
        }

        if (Physics.Raycast(worldPos, Vector3.forward, out _touch, 10f) && worldPos != Vector3.zero)
        {

            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_penSize / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_penSize / 2));

                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x)
                {
                    return;
                }

                if (_touchedLastFrame)
                {
                    var originalColors = _whiteboard.originalTexture.GetPixels(x, y, _penSize, _penSize);
                    _whiteboard.texture.SetPixels(x, y, _penSize, _penSize, originalColors);

                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);

                        // Set pixels
                        originalColors = _whiteboard.originalTexture.GetPixels(lerpX, lerpY, _penSize, _penSize);
                        _whiteboard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, originalColors);
                    }

                    //transform.rotation = _lastTouchRot;

                    _whiteboard.texture.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }

        _whiteboard = null;
        _touchedLastFrame = false;
    }

    private class BrushStroke
    {
        public int _x { get; set; }
        public int _y { get; set; }
        public int _size { get; set; }
        public Color[] _colors { get; set; }
        public Whiteboard _whiteboard { get; set; }

        public BrushStroke(int x, int y, int size, Color[] c, Whiteboard whiteboard)
        {
            _x = x;
            _y = y;
            _size = size;
            _colors = c;
            _whiteboard = whiteboard;
        }
    }
}


