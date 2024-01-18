using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityMeshImporter;
using System.IO;
using System.Xml;
using Unity.VisualScripting;
using System.Security.Cryptography;

public class ModelSwitcher : MonoBehaviour
{
    /**
     *  I guess the idea for now is to have a resources folder with all of 
     * the desireable models, then load them in with Resources.Load();
     * https://docs.unity3d.com/ScriptReference/Resources.Load.html
     * 
     *  Maybe in the future instead of loading from the Resources, you can
     * select a file in the game, we can add it to a list of loaded models that
     * the user can chose from. Kind of like how someone may have quick access to
     * previously edited files in Photoshop.
     * ^ Would need to serialize
     * https://docs.unity3d.com/ScriptReference/AssetDatabase.CreateAsset.html (?)
     * 
     * If you want to store the models outside of Unity, you need to write your own importer
     * or find an importer online. Also entirely depends on the type of model (.fbx, .dae, ect...)
     */

    // TODO: Make a way to save coordinates/rotations for each model, for those that are never up right
    // TODO: Add images to each folder so it can be placed on the button -> Models/{model}/{month}/png.
    /*  NOTES:
     *  * Possible to go with a method out of the resource folder. To this (in my opinion) we would need to
     *  1. Designate a directory
     *  2. Replicate the loading and finding of the models with the new directory, would need to use MeshImporter
     *  
     *  * 
     */

    [SerializeField] private GameObject modelScrollContentHolder; // The content object under the scroll view hierarchy
    [SerializeField] private GameObject modelSelectionButtonPrefab; // The prefab of a button that will be copied
    [SerializeField] private GameObject monthScrollContentHolder; // The content object under the scroll view hierarchy

    [SerializeField] private Material transparentMaterial; // Needed to add to the whiteboard componenet

    [Header("Saving")]
    [SerializeField] private string savePath; // The folder to save models to

    private string parentPath = "Models/"; // Models/{name}/{month}/{name}
    private string jsonPath = "ModelInfo";

    private GameObject currentLoadedModel;

    private string currentModelName, currentModelMonth;
    // Start is called before the first frame update
    void Start()
    {
        // Load the JSON
        ModelInfo info = LoadModelInfoFromJSON(jsonPath);

        // Populate the model buttons 
        PopulateModelNameMenu(info);

        // Testing Model Importer
        LoadModelFromDirectory("C:\\Users\\piegu\\Documents\\HRI Work\\Models\\dae_models\\pinanski_1.dae");

        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SaveModel(currentModelName, currentModelMonth, savePath);
        }
    }

    /// <summary>
    /// Uses the MeshImporter library to load a model from the path and adds
    /// the necessary components needed for drawing.
    /// </summary>
    /// <param name="path"></param>
    private void LoadModelFromDirectory(string path)
    {
        List<GameObject> objs = new List<GameObject>();
        GameObject loadedObj = MeshImporter.Load(path);

        if (loadedObj == null)
        {
            throw new ModelNotFoundException("Model from " + path + " was not found");
        }

        loadedObj.GetChildGameObjects(objs);
        objs[0].GetChildGameObjects(objs); // Need to go two layers deep because a model spawns in like Scene/{name}/{model}

        objs[1].tag = "Whiteboard";
        objs[1].AddComponent<MeshCollider>();
        objs[1].AddComponent<Whiteboard>().transparentMaterial = transparentMaterial;
    }

    /// <summary>
    /// Creates a ModelInfo object with the resource path of a json file
    /// </summary>
    /// <param name="path"></param>
    /// <returns>ModelInfo (null if error)</returns>
    private ModelInfo LoadModelInfoFromJSON(string path)
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(path);
        string jsonString = "";
        if (jsonTextAsset != null)
        {
            jsonString = jsonTextAsset.text;
        } else
        {
            Debug.LogError("[Model Loader] JSON file not found");
            return null;
        }

        ModelInfo info = JsonUtility.FromJson<ModelInfo>(jsonString);

        return info;
    }

    /// <summary>
    /// Populate the model scroll view given a ModelInfo object
    /// </summary>
    /// <param name="modelinfo"></param>
    private void PopulateModelNameMenu(ModelInfo modelinfo)
    {
        foreach(Model m in modelinfo.models)
        {
            // Instantiate button and set parent as content in scroll view
            var btn = Instantiate(modelSelectionButtonPrefab);
            btn.transform.SetParent(modelScrollContentHolder.transform, false);

            Navigation n = Navigation.defaultNavigation;
            n.mode = Navigation.Mode.None;

            // Add PopulateModelDateMenu function to each button 
            btn.GetComponent<Button>().onClick.AddListener(delegate { PopulateModelDateMenu(m); });
            List<GameObject> btn_children = new List<GameObject>();
            btn.GetComponent<Button>().navigation = n;
            btn.GetChildGameObjects(btn_children);

            // Change Button name to the month
            TextMeshProUGUI tmp = btn_children[0].GetComponent<TextMeshProUGUI>();
         
            if(tmp != null)
            {
                tmp.SetText(m.name);
            }
        }
    }

    /// <summary>
    /// Populate the month scroll view given a Model object
    /// </summary>
    /// <param name="m"></param>
    public void PopulateModelDateMenu(Model m)
    {
        // Clear the other buttons first
        List<GameObject> monthButtons = new List<GameObject>();

        monthScrollContentHolder.GetChildGameObjects(monthButtons);

        if(monthButtons.Count > 0)
        {
            foreach(GameObject btn in monthButtons)
            {
                Destroy(btn);
            }
        }

        foreach(string month in m.months)
        {
            // Instantiate button and set parent as content in scroll view
            var btn = Instantiate(modelSelectionButtonPrefab);
            btn.transform.SetParent(monthScrollContentHolder.transform, false);

            Navigation n = Navigation.defaultNavigation;
            n.mode = Navigation.Mode.None;

            // Add LoadModel function to each button 
            btn.GetComponent<Button>().onClick.AddListener(delegate { LoadModel(m.name, month); });
            btn.GetComponent<Button>().navigation = n;
            List<GameObject> btn_children = new List<GameObject>();
            btn.GetChildGameObjects(btn_children);

            // Change Button name to the month
            TextMeshProUGUI tmp = btn_children[0].GetComponent<TextMeshProUGUI>();

            if (tmp != null)
            {
                tmp.SetText(month);
            }
        }
    }

    /// <summary>
    /// Loads a model from assets given the month and the name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="month"></param>
    public void LoadModel(string name, string month)
    {
        // Destroys currently loaded model
        if(currentLoadedModel != null)
        {
            Destroy(currentLoadedModel);
            currentLoadedModel = null;
        }

        // Load the model from Resources
        string path = String.Format("Models/{0}/{1}/{0}", name, month);
        GameObject obj = Resources.Load<GameObject>(path);

        // If the object isn't null, Instantiate
        if(obj != null)
        {
            currentLoadedModel = Instantiate(obj);

            // Add properties to draw on the object
            currentLoadedModel.tag = "Whiteboard";
            currentLoadedModel.AddComponent<MeshCollider>();
            currentLoadedModel.AddComponent<Whiteboard>().transparentMaterial = transparentMaterial;

            // Track the model as the current model
            currentModelName = name;
            currentModelMonth = month;
        } else
        {
            Debug.LogError(String.Format("The model path was null {0}", path));
        }


    }

    /// <summary>
    /// Takes the model name, month, and a save directory to save
    /// the model's dae and texture file. Saves the current draw state too.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="month"></param>
    /// <param name="savePath"></param>
    void SaveModel(string name, string month, string savePath) 
    {
        
        // Test the save path
        if(!System.IO.Directory.Exists(savePath))
        {
            System.IO.Directory.CreateDirectory(savePath);
            Debug.Log("Directory Created");
        }

        // Initialize XmlDoc
        XmlDocument modelXml = new XmlDocument();
        TextAsset xmlText = Resources.Load<TextAsset>(String.Format("Models/{0}/{1}/{0}text", name, month));

        if(xmlText == null)
        {
            ModelNotFoundException exc = new ModelNotFoundException(String.Format("The model {0} with the month {1} was not found.", name, month));
            throw exc;
        }

        modelXml.LoadXml(xmlText.text);

        XmlNode libraryImages = modelXml.ChildNodes[1];

        // Search for the 'library_images' node, its has the name of the texture file
        foreach (XmlNode node in modelXml.ChildNodes[1])
        {
            if (node.Name == "library_images")
            {
                libraryImages = node;
            }
        }

        // Initialize a new texture name and save the resulting xml
        string modelFileName = String.Format("{0}_{1}", name, month);

        libraryImages.ChildNodes[0].ChildNodes[0].InnerText = modelFileName + ".png";
        modelXml.Save(savePath + modelFileName + ".dae");

        // Texture Saving
        Texture2D modelTexture = currentLoadedModel.GetComponent<Renderer>().materials[1].mainTexture as Texture2D;

        Texture2D decompressed = modelTexture.DeCompress(); // Base Decomnpressed texture
        Texture2D drawnTexture = currentLoadedModel.GetComponent<Whiteboard>().drawTexture; // The overlayed drawn texture

        Texture2D combinedTexture = OverlayTextures(decompressed, drawnTexture); // Put the drawing texture over the bottom texture

        Byte[] data = combinedTexture.EncodeToPNG();
        
        File.WriteAllBytes(savePath + modelFileName + ".png", data);

        Debug.Log("Saved!");
    }

    // Debug Functions

    void TestSave()
    {
        string savePath = "C:\\Users\\piegu\\Documents\\HRI Work\\Models\\saved models from runtime\\";
        
        // DAE Parsing (This save function can only work with DAE's
        
        XmlDocument modelXml = new XmlDocument();

        modelXml.Load("C:\\Users\\piegu\\Documents\\HRI Work\\Models\\dae_models\\pinanski_1.dae");

        Debug.Log(modelXml.ChildNodes.Count);
        XmlNode libraryImages = modelXml.ChildNodes[1];

        foreach(XmlNode node in modelXml.ChildNodes[1])
        {
            if(node.Name == "library_images")
            {
                libraryImages = node;
            }
        }

        libraryImages.ChildNodes[0].ChildNodes[0].InnerText = "image.png";
        modelXml.Save(savePath + "model.dae");
        // Texture Saving
        Texture2D modelTexture = currentLoadedModel.GetComponent<Renderer>().materials[1].mainTexture as Texture2D;

        Texture2D decompressed = modelTexture.DeCompress(); // Base Decomnpressed texture
        Texture2D drawnTexture = currentLoadedModel.GetComponent<Whiteboard>().drawTexture; // The overlayed drawn texture

        Texture2D combinedTexture = OverlayTextures(decompressed, drawnTexture); // Put the drawing texture over the bottom texture

        Byte[] data = combinedTexture.EncodeToPNG();
        File.WriteAllBytes(savePath + "image.png", data);

        Debug.Log("Saved!");
    }
    public void TestFunction(string msg) { Debug.Log(msg); }
    
    /// <summary>
    /// Takes two textures and puts the top one over the bottom one and combining
    /// them into one single texture.
    /// </summary>
    /// <param name="textureBottom"></param>
    /// <param name="textureTop"></param>
    /// <returns></returns>
    Texture2D OverlayTextures(Texture2D textureBottom, Texture2D textureTop)
    {
        // Maybe error later if the two textures dont have the same size, or if top isnt smaller than bottom
        Texture2D newTexture = new Texture2D(textureBottom.width, textureTop.width);
        newTexture.SetPixels(textureBottom.GetPixels());

        for (int x = 0; x < textureTop.width; x++)
        {
            for (int y = 0; y < textureTop.height; y++)
            {
                Color c = textureTop.GetPixel(x, y);
                if(c.a > 0.0f)
                {
                    newTexture.SetPixel(x, y, c);
                }
            }
        }

        return newTexture;
    }



}

[Serializable]
class ModelInfo
{
    public List<Model> models;
}

[Serializable]
public struct Model
{
    public string name;
    public List<string> months;

}

/// <summary>
/// https://stackoverflow.com/questions/51315918/how-to-encodetopng-compressed-textures-in-unity
/// </summary>
public static class ExtensionMethod
{
    public static Texture2D DeCompress(this Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}

public class ModelNotFoundException : Exception
{
    public ModelNotFoundException()
    {
        
    }

    public ModelNotFoundException(string message) : base (message) { }
}
