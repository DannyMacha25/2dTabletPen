using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Texture2D originalTexture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    void Start()
    {
        // NOTE: When introducing custom textures, may have to set the texture to the already applied texture
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        originalTexture = new Texture2D((int)textureSize.x, (int)textureSize.y); // Again may need to modify this for other textures
        r.material.mainTexture = texture;
    }


}
