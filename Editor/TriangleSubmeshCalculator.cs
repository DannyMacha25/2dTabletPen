using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Jobs;
using UnityEditor;
using UnityEngine.InputSystem.HID;
using System;

public class TriangleSubmeshCalculator : EditorWindow
{
    public UnityEngine.Object _model;
    private Mesh _mesh;
    public int _currentIndex = 0;

    private bool _currentlyCalculating = false;
    [MenuItem("Window/EditorTest")]
    public static void OpenWindow()
    {
        GetWindow<TriangleSubmeshCalculator>("Submesh Cacher");
    }

    private void Update()
    {
        //Debug.Log("AAAAA");
        if (_currentlyCalculating)
        {
            int meshIndex = DetermineMeshIndex(_mesh, _currentIndex);
            Debug.Log("Triangle " + _currentIndex + " is on submesh " + meshIndex);
            _currentIndex++;
            if (_currentIndex == _mesh.triangles.Length)
            {
                _currentlyCalculating = false;
            }
        }
    }

    // Update frame for gui
    private void OnGUI()
    {
        //Thread t = new Thread(new ThreadStart(CalculateMeshes));

        GUILayout.Label("The Ultimate Submesh Calculator :DDDD");
        bool button = GUILayout.Button("Calculate");
        _model = EditorGUILayout.ObjectField(_model, typeof(Mesh), true);
        if (_model != null)
        {
            _mesh = (Mesh)_model;
            EditorGUI.ProgressBar(new Rect(2, 75, position.width - 5, 20), _currentIndex / _mesh.triangles.Length, _currentIndex + "/" + _mesh.triangles.Length);
        }

        if (_model != null && button)
        {
            _currentIndex = 0;
            _currentlyCalculating = true;
            //t.Start()
        }


    }
    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void CacheMesh(Mesh m)
    {
        int subMeshCount = m.subMeshCount;
        List<int[]> trianglesInSubmeshes = new List<int[]>();
        for(int i = 0; i < subMeshCount; i++)
        {
            trianglesInSubmeshes.Add(m.GetTriangles(i));
        }
    }

    private int DetermineMeshIndex(Mesh m, int triangleIndex)
    {
        int submeshIndex = 0;
        int triangleCount = 0;

        for (int i = 0; i < m.subMeshCount; i++)
        {
            var triangles = m.GetTriangles(i);
            triangleCount += triangles.Length;
            if (triangleIndex < triangleCount)
            {
                submeshIndex = i;
                break;
            }
        }

        //Debug.Log("Mesh Index: " + submeshIndex);
        return submeshIndex;
    }


}
