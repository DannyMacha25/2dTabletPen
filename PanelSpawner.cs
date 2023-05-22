using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelSpawner : MonoBehaviour
{
    [SerializeField] GameObject panelPrefab;
    [SerializeField] GameObject panelPrefab3D;
    [SerializeField] KeyCode spawnButton = KeyCode.F;

    private Transform tf;
    void Start()
    {
        tf = this.GetComponent<Transform>();
    }

    void Update()
    {
        if(Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (Keyboard.current.leftCtrlKey.isPressed)
            {
                Spawn3DPanel();
            }
            else
            {
                SpawnPanel();
            }
        }
    }

    void SpawnPanel()
    {
        Vector3 rot = tf.rotation.eulerAngles;
        Instantiate(panelPrefab, tf.position, Quaternion.Euler(270,rot.y,0));
    }

    void Spawn3DPanel()
    {
        Vector3 rot = tf.rotation.eulerAngles;
        Instantiate(panelPrefab3D, tf.position, Quaternion.Euler(0, rot.y, 0));
    }
}
