using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;

public class TextureDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 worldPos = new Vector3(0, 0, 0);
        if (Pointer.current.press.isPressed)
        {
            //Debug.Log("ahhhh");
            RaycastHit hitData;
            pos = Pointer.current.position.ReadValue();
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
            //Debug.DrawRay(worldPos, Vector3.forward, Color.green, 10f);
            //Debug.Log(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)));
        }

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue()), out hit) && worldPos != Vector3.zero) // Use Input.mouseposition for mouse input
        {
            Vector2 textureCoord = new Vector2(hit.textureCoord.x * 4000, hit.textureCoord.y * 4000);
            MeshCollider collider = hit.collider as MeshCollider; 
            //Debug.Log(textureCoord.ToString());
            Debug.Log(hit.triangleIndex + " / " + collider.sharedMesh.triangles.Length);

            int submeshIndex = 0;
            int triangleCount = 0;
            Debug.Log("Sub mesh count: " + collider.sharedMesh.subMeshCount);
            for (int i = 0; i < collider.sharedMesh.subMeshCount; i++)
            {
                var triangles = collider.sharedMesh.GetTriangles(i);
                triangleCount += triangles.Length;
                if (hit.triangleIndex < triangleCount)
                {
                    submeshIndex = i;
                    break;
                }
            }

            Debug.Log("Mesh Index: " + submeshIndex);
        }
    }
}
