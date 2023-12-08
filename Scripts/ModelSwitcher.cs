using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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


    // TODO: For easier compatability, make it so a controller can call this rather than have keycodes hard coded in here
    // TODO: Adding button through script https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.Button-onClick.html

    private string parentPath = "Models/"; // Models/{name}/{month}/{name}
    private string jsonPath = "ModelInfo";

    private GameObject currentModel;
    // Start is called before the first frame update
    void Start()
    {
        // Load the JSON
        ModelInfo info = LoadModelInfoFromJSON(jsonPath);

        GameObject[] models = Resources.LoadAll<GameObject>("Models/");
        //GameObject cafe = Resources.Load<GameObject>("Models/cafe/September/{name}");
        foreach(GameObject m in models)
        {
            if(m != null)
            {
                Debug.Log(m.name);
            } else
            {
                Debug.Log("Object was null");
            }
        }

    }

    private ModelInfo LoadModelInfoFromJSON(string path)
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(path);
        string jsonString = "";
        if (jsonTextAsset != null)
        {
            jsonString = jsonTextAsset.text;
        } else
        {
            return null;
        }

        ModelInfo info = JsonUtility.FromJson<ModelInfo>(jsonString);

        return info;
    }

    // TODO: Populate a menu with all of the models and their names
    private void PopulateModelNameMenu()
    {

    }

    // TODO: Whenever a model is clicked in the model menu, populate this menu with the possible dates,
    //  possibly take the name, maybe the index in the model info array
    public void PopulateModelDateMenu(string name)
    {

    }

    // TODO: Load and instantiate model into the scene given the name and month
    //  Probably unload the current model too.
    public void LoadModel(string name, string month)
    {

    }

    
}

[Serializable]
class ModelInfo
{
    public List<Model> models;
}

[Serializable]
struct Model
{
    public string name;
    public List<string> months;
}
