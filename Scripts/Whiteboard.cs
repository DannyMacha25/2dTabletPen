using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Texture2D drawTexture;

    public bool showDrawTexutre;

    [SerializeField] Material transparentMaterial; 
    public Vector2 textureSize = new Vector2(2048, 2048);
    void Start()
    {
        // NOTE: When introducing custom textures, may have to set the texture to the already applied texture
        var r = GetComponent<Renderer>();
        texture = r.material.mainTexture as Texture2D;
        if (texture == null)
        {
            texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        }

        drawTexture = new Texture2D((int)textureSize.x, (int)textureSize.y); // Again may need to modify this for other textures
        //drawTexture = new Texture2D((int)texture.Size().x, (int)texture.Size().x); // Again may need to modify this for other textures
        Color[] transparentPixels = new Color[drawTexture.width * drawTexture.height];

        for (int i = 0; i < transparentPixels.Length; i++)
        {
            transparentPixels[i] = Color.clear;
        }
        

        drawTexture.SetPixels(transparentPixels);
        drawTexture.Apply();

        // Set textures and materials
        r.material.mainTexture = texture;
        transparentMaterial.mainTexture = drawTexture;

        List<Material> materials = new List<Material>(r.materials);
        Material m = new Material(transparentMaterial);
        materials.Insert(0, m);
        //materials.Add(r.material);
        r.materials = materials.ToArray();
    }

    private void OnGUI()
    {
        if (showDrawTexutre)
        {
            GUI.DrawTexture(new Rect(0, 500, 256, 256), drawTexture, ScaleMode.ScaleToFit, false, 1);
        }
    }
}
