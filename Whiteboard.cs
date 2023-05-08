using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Texture2D drawTexture;

    [SerializeField] Material transparentMaterial; 
    public Vector2 textureSize = new Vector2(2048, 2048);
    void Start()
    {
        // NOTE: When introducing custom textures, may have to set the texture to the already applied texture
        var r = GetComponent<Renderer>();
        if (texture == null)
        {
            texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        }
        drawTexture = new Texture2D((int)textureSize.x, (int)textureSize.y); // Again may need to modify this for other textures

        // Make draw texture all blank
        for (int y = 0; y < drawTexture.height; y++)
        {
            for (int x = 0; x < drawTexture.width; x++)
            {

                drawTexture.SetPixel(x, y, new Color(0,0,0,0));

            }
        }

        drawTexture.Apply();

        // Set textures and materials
        r.material.mainTexture = texture;
        transparentMaterial.mainTexture = drawTexture;

        List<Material> materials = new List<Material> ();
        Material m = new Material(transparentMaterial);
        materials.Add(m);
        materials.Add(r.material);
        r.SetMaterials(materials);
    }


}
