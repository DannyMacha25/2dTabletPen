using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

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
    // TODO: Either make each model a prefab with drawable properties, or add those properties through script


    [SerializeField] private GameObject modelScrollContentHolder; // The content object under the scroll view hierarchy
    [SerializeField] private GameObject modelSelectionButtonPrefab;
    [SerializeField] private GameObject monthScrollContentHolder; // The content object under the scroll view hierarchy

    private string parentPath = "Models/"; // Models/{name}/{month}/{name}
    private string jsonPath = "ModelInfo";

    private GameObject currentLoadedModel;

    private GameObject currentModel;
    // Start is called before the first frame update
    void Start()
    {
        // Load the JSON
        ModelInfo info = LoadModelInfoFromJSON(jsonPath);

        Debug.Log(info.models[0].name);

        PopulateModelNameMenu(info);

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

            // Add PopulateModelDateMenu function to each button 
            btn.GetComponent<Button>().onClick.AddListener(delegate { PopulateModelDateMenu(m); });
            List<GameObject> btn_children = new List<GameObject>();
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

            // Add LoadModel function to each button 
            btn.GetComponent<Button>().onClick.AddListener(delegate { LoadModel(m.name, month); }); 
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

        string path = String.Format("Models/{0}/{1}/{0}", name, month);
        GameObject obj = Resources.Load<GameObject>(path);

        if(obj != null)
        {
            currentLoadedModel = Instantiate(obj);
        } else
        {
            Debug.LogError(String.Format("The model path was null {0}", path));
        }


    }


    public void TestFunction(string msg) { Debug.Log(msg); }
    
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
