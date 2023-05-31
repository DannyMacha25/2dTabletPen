using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using System;

public class Marker : MonoBehaviour
{
    enum Tool
    {
        Pen,
        Eraser,
        ColorPicker
    }

    // Public editables
    [SerializeField] private int _penSize = 5;
    [SerializeField] private Color _color;
    [SerializeField] private RGBInput _colorInput;
    [SerializeField] bool _acceptMouseInput = false;
    [SerializeField] TextMeshProUGUI _sizeText;
    // Be sure the panels are in order of PEN, ERASER, COLOR PICKER
    [SerializeField] GameObject[] _toolPanels;

    // Colors for UI
    private Color _toolSelectedColor = new Color(248 / 255f, 1f, 117 / 255f, 1f);
    private Color _toolUnSelectedColor = new Color(1f, 1f, 1f, 1f);

    // Private Fields
    private Color[] _colors;
    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    private Plane _plane = new Plane(Vector3.up, 0);

    private int _eraserSize = 5;
    private Tool _currentTool = Tool.Pen;

    // Undo specific fields
    private CappedStack<WhiteboardState> _wbStateStack = new CappedStack<WhiteboardState>(10);
    void Start()
    {
        //_renderer = _tip.GetComponent<Renderer>();

        _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();
        _sizeText.text = _penSize.ToString();
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

        // Change Tool NOTE: Add mouse interaction with UI in future
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            ChangeTool(Tool.Pen);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            ChangeTool(Tool.Eraser);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            ChangeTool(Tool.ColorPicker);
        }
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            Undo();
        }
        if (!Keyboard.current.leftCtrlKey.isPressed)
        {
            switch (_currentTool)
            {
                case Tool.Pen:
                    Draw(); break;

                case Tool.Eraser:
                    Erase(); break;
                case Tool.ColorPicker:
                    PickColor(); break;
            }
        }
    }

    private void ChangeColor(Color c)
    {
        _color = c;
        _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();
    }
    private void ChangePenSize(int change)
    {
        switch(_currentTool)
        {
            case Tool.Pen:
                _penSize += change;
                _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();

                if (_penSize < 1) { _penSize = 1; }

                _sizeText.text = _penSize.ToString();
                break;
            case Tool.Eraser:
                _eraserSize += change;
                _colors = Enumerable.Repeat(_color, _eraserSize * _eraserSize).ToArray();

                if (_eraserSize < 1) { _eraserSize = 1; }

                _sizeText.text = _eraserSize.ToString();
                break;
        }
    }
    private void ChangeTool(Tool t)
    {
        _currentTool = t;
        switch(t)
        {
            case Tool.Pen:
                _toolPanels[0].GetComponent<UnityEngine.UI.Image>().color = _toolSelectedColor;
                _toolPanels[1].GetComponent<UnityEngine.UI.Image>().color = _toolUnSelectedColor;
                _toolPanels[2].GetComponent<UnityEngine.UI.Image>().color = _toolUnSelectedColor;
                _sizeText.text = _penSize.ToString();
                break;
            case Tool.Eraser:
                _toolPanels[0].GetComponent<UnityEngine.UI.Image>().color = _toolUnSelectedColor;
                _toolPanels[1].GetComponent<UnityEngine.UI.Image>().color = _toolSelectedColor;
                _toolPanels[2].GetComponent<UnityEngine.UI.Image>().color = _toolUnSelectedColor;
                _sizeText.text = _eraserSize.ToString();
                break;
            case Tool.ColorPicker:
                _toolPanels[0].GetComponent<UnityEngine.UI.Image>().color = _toolUnSelectedColor;
                _toolPanels[1].GetComponent<UnityEngine.UI.Image>().color = _toolUnSelectedColor;
                _toolPanels[2].GetComponent<UnityEngine.UI.Image>().color = _toolSelectedColor;
                _sizeText.text = " ";
                break;
        }
    }
    // Pen
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
            Debug.Log(_touch.textureCoord.x + " " + _touch.textureCoord.y);

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
                    _whiteboard.drawTexture.SetPixels(x, y, _penSize, _penSize, _colors);
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);

                        // Set pixels
                        _whiteboard.drawTexture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                    }

                    //transform.rotation = _lastTouchRot;

                    _whiteboard.drawTexture.Apply();
                }

                if(!_touchedLastFrame)
                {
                    var wbState = new WhiteboardState(_whiteboard, _whiteboard.drawTexture);
                    _wbStateStack.Push(wbState);
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

    // Eraser
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

                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_eraserSize / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_eraserSize / 2));

                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x)
                {
                    return;
                }

                if (_touchedLastFrame)
                {
                    Color[] blankColors = new Color[_eraserSize * _eraserSize];
                    //Array.Fill<Color>(blankColors, new Color(0, 0, 0, 0)); //.NET 2.1
                    FillArray<Color>(blankColors, new Color(0, 0, 0, 0)); //not .NET 2.1
                    _whiteboard.drawTexture.SetPixels(x, y, _eraserSize, _eraserSize, blankColors);

                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);

                        // Set pixels
                        _whiteboard.drawTexture.SetPixels(lerpX, lerpY, _eraserSize, _eraserSize, blankColors);
                    }

                    //transform.rotation = _lastTouchRot;

                    _whiteboard.drawTexture.Apply();
                }

                if (!_touchedLastFrame)
                {
                    var wbState = new WhiteboardState(_whiteboard, _whiteboard.drawTexture);
                    _wbStateStack.Push(wbState);
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

    // Color Picker
    private void PickColor()
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
                Whiteboard wb = _touch.transform.GetComponent<Whiteboard>();
                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchPos.x * wb.textureSize.x - (_penSize / 2));
                var y = (int)(_touchPos.y * wb.textureSize.y - (_penSize / 2));

                if (y < 0 || y > wb.textureSize.y || x < 0 || x > wb.textureSize.x)
                {
                    return;
                }

                var color = wb.drawTexture.GetPixel(x, y);

                ChangeColor(color);
                _colorInput.UpdateColor(color);
            }
        }
    }

    private void Undo()
    {
        var prevState = _wbStateStack.Pop();
        if (prevState == null)
        {
            Debug.Log("Twas Null");
            return;
        }

        Graphics.CopyTexture(prevState.texture, prevState.wb.drawTexture);
        prevState.wb.drawTexture.Apply();
        prevState = null;
    }

    /**
     * Functions for the RGB sliders
     */
    public void ChangeToPen()
    {
        ChangeTool(Tool.Pen);
    }

    public void ChangeToEraser()
    {
        ChangeTool(Tool.Eraser);
    }

    public void ChangeToColorPicker()
    {
        ChangeTool(Tool.ColorPicker);
    }

    private void FillArray<T>(T[] arr, T element)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = element;
        }
    }

    private class WhiteboardState
    {
        public Whiteboard wb { get; }
        public Texture2D texture { get; }

        public WhiteboardState(Whiteboard whb)
        {
            wb = whb;
            texture = new Texture2D(whb.drawTexture.width, whb.drawTexture.height);
            Graphics.CopyTexture(whb.drawTexture, texture);
            texture.Apply();
        }

        public WhiteboardState(Whiteboard whb, Texture2D tex)
        {
            wb = whb;
            texture = new Texture2D(tex.width, tex.height);
            Graphics.CopyTexture(tex, texture);
            texture.Apply();
        }

    }

    
}


