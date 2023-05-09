using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelSpawner : MonoBehaviour
{
    [SerializeField] GameObject panelPrefab;
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
            SpawnPanel();
        }
    }

    void SpawnPanel()
    {
        Vector3 rot = tf.rotation.eulerAngles;
        Instantiate(panelPrefab, tf.position, Quaternion.Euler(270,rot.y,0));
    }
}
