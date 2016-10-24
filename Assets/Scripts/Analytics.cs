using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


/*
   This class is used to get information from vreyecast
   The idea is to pass through the hit object from the eyecast, then
   in this class count the time spent looking at the object
   */
public class Analytics : MonoBehaviour
{

    #region Singleton
    private static Analytics analytics = null;
    public static Analytics instance { get { return analytics; } }
    

    void Awake()
    {
        if (analytics == null)
        {
            analytics = this;
        }
        else
        {
            Destroy(this);
        }
        Load();
    }
    #endregion

    #region Containers
    Dictionary<string, float> lookAtDictionary = new Dictionary<string, float>();
    Dictionary<string, float> timeSpentInScenes = new Dictionary<string, float>();
    List<Vector3> lookAtLocation = new List<Vector3>();
    List<Vector3> playerLocation = new List<Vector3>();
    List<AnalyticData> dataList = new List<AnalyticData>();
    #endregion


    #region LoadSave
    string path = "/SavedDataListClassCKVP.VRA";
    FileStream file;
    BinaryFormatter binFor;
    #endregion

    Vector3 hitTemp = Vector3.zero;
    string hitName = string.Empty;
    string saveName = string.Empty;
    float time = 0;
    float saveTime = 0;
    [SerializeField]
    private bool viewData;
    [SerializeField]
    GameObject showData;
   
    void Start()
    {
       
        if(viewData)
        {
            showData.SetActive(true);
        }
        //else
        //{
        //    playerHeight = Camera.main.transform.position.y;
        //}
    }

 
    /// <summary>
    /// Public function to collect scene data and load next scene.
    /// This is so the developer can just call a single function to load a scene and 
    /// collect scene data
    /// </summary>
    /// <param name="_scene">name of scene to be loaded</param>
    public void LoadNextScene(string _scene)
    {
       string sceneName = SceneManager.GetActiveScene().name;
        float timeSpent = Time.timeSinceLevelLoad;
        if (timeSpentInScenes.ContainsKey(sceneName))
        {
            timeSpentInScenes[sceneName] += timeSpent;
        }
        else
        {
            timeSpentInScenes.Add(sceneName, timeSpent);
        }
        SceneManager.LoadScene(_scene);
    }

    /// <summary>
    /// Function designed to be used with VR raycast
    /// To be called if hit result != null
    /// </summary>
    /// <param name="_hit">Hit info of VR user look at</param>
    public void CollectInformation(RaycastHit _hit)
    {
        if (!viewData)
        {
            SaveLookPos(_hit.point);
            LookObject(_hit.collider.gameObject);
         //   SavePlayerPos();
        }
    }

    /// <summary>
    /// Adds relevant info to Dictionary<string,int>
    /// </summary>
    /// <param name="_name">target object name</param>
    /// <param name="_time">time spent looking at target object</param>
    void AddToDictionary(string _name, float _time)
    {
        if (!lookAtDictionary.ContainsKey(_name))
        {
            lookAtDictionary.Add(_name, _time);
            Debug.Log("object " + _name + " observed for " + _time);
        }
        else
        {
            lookAtDictionary[_name] += _time;
            Debug.Log("object " + _name + " observed for " + lookAtDictionary[_name]);
        }
    }

    /// <summary>
    /// Works out if the current target object is new.
    /// If so, saves old data to Dictionary<string,float> and resets time
    /// </summary>
    /// <param name="_viewedObj">Currently viewed Gameobject</param>
    void LookObject(GameObject _viewedObj)
    {
        if (_viewedObj.name != hitName)
        {
            if (_viewedObj.tag != "Player")
            {
                saveTime = time;
                saveName = hitName;
                hitName = _viewedObj.name;
                time = 0;
                if (saveName != string.Empty)
                {
                    AddToDictionary(saveName, saveTime);
                }
            }
        }
        else
        {
            hitName = _viewedObj.name;
            time += Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Saves the hitpoint of the raycast
    /// </summary>
    /// <param name="_look">focus of users gaze</param>
	private void SaveLookPos(Vector3 _look)
    {
        if (_look != hitTemp)
        {
            if (Vector3.Distance(_look, hitTemp) > 0.2f)
            {
                lookAtLocation.Add(_look);
                hitTemp = _look;
                SavePlayerPos();
            }
         ///   Debug.Log("look at location "+_look);
        }
    }

    /// <summary>
    /// Takes Camera Position and adds to a list
    /// </summary>
    private void SavePlayerPos()
    {
        playerLocation.Add(Camera.main.transform.position);
    }
    /// <summary>
    /// Loads saved player data
    /// called in Awake()
    /// </summary>
    void Load()
    {
        binFor = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + path))
        {
            
            file = File.Open(Application.persistentDataPath + path, FileMode.Open);
            string toLoad = (string)binFor.Deserialize(file);
            file.Close();
            DataList dl  = JsonUtility.FromJson <DataList>(toLoad);
            dataList = dl.dataList;
            Debug.Log("datalist size " + dataList.Count);
        }
        else
        {
            Debug.Log("no data");
        }
    }
    /// <summary>
    /// loads json data as original data class
    /// </summary>
    /// <param name="_JSON">the JSON string retrieved</param>
    public void LoadJSON(string _JSON)
    {
        DataList dl = JsonUtility.FromJson<DataList>(_JSON);
        dataList = dl.dataList;
        Debug.Log("datalist size " + dataList.Count);
    }
    /// <summary>
    /// saves new player data
    /// called in OnApplicationQuit()
    /// </summary>
    void Save()
    {
        timeSpentInScenes.Add(SceneManager.GetActiveScene().name, Time.timeSinceLevelLoad);
        List<CustomKVP> lookAtList = ConvertDicToList(lookAtDictionary);
        List<CustomKVP> timeSpentInList = ConvertDicToList(timeSpentInScenes);
        dataList.Add(new AnalyticData(lookAtList, timeSpentInList, lookAtLocation,playerLocation,SystemInfo.deviceName));
        DataList dl = new DataList(dataList);
        string toSave = JsonUtility.ToJson(dl);
        Debug.Log("saved " + dataList.Count) ;
        //Saves json data out as serialized data
        binFor = new BinaryFormatter();
        file = File.Create(Application.persistentDataPath + path);
        binFor.Serialize(file, toSave);
        file.Close();
    }

    /// <summary>
    /// Returns JSON string of gamedata
    /// </summary>
    /// <returns>JSON string</returns>
    public string ReturnJSONData()
    {
        timeSpentInScenes.Add(SceneManager.GetActiveScene().name, Time.timeSinceLevelLoad);
        List<CustomKVP> lookAtList = ConvertDicToList(lookAtDictionary);
        List<CustomKVP> timeSpentInList = ConvertDicToList(timeSpentInScenes);
        dataList.Add(new AnalyticData(lookAtList, timeSpentInList, lookAtLocation, playerLocation, SystemInfo.deviceName));
        DataList dl = new DataList(dataList);
        string toSave = JsonUtility.ToJson(dl);
        return toSave;
    }
    /// <summary>
    /// Converts Dictionary to serializable List<kvp>
    /// </summary>
    /// <param name="_list">Dictionary to convert</param>
    /// <returns>converted list<kvp></kvp></returns>
    List<CustomKVP> ConvertDicToList(Dictionary<string,float>_list)
    {
        List<CustomKVP> temp = new List<CustomKVP>();
        foreach(KeyValuePair<string,float>kvp in _list)
        {
            temp.Add(new CustomKVP(kvp.Key,kvp.Value));
        }
        return temp;
    }
    public List<AnalyticData> ReturnSavedData()
    {
        return dataList;
    }
    void OnApplicationQuit()
    {
        if (!viewData)
        {
            foreach(KeyValuePair<string,float>kvp in lookAtDictionary)
            {
                Debug.Log(kvp.Key + " " + kvp.Value);
            }
            Save();
        }
    }
   
}

[Serializable]
public class CustomKVP
{
    public string key;
    public float value;

    public CustomKVP(string _key,float _value)
    {
        key = _key;
        value = _value;
    }
}

[Serializable]
class DataList
{
    public List<AnalyticData> dataList;
    public DataList(List<AnalyticData>_list)
    {
        dataList = _list;
    }
}
[Serializable]
public class AnalyticData
{
    public List<CustomKVP> lookAt = new List<CustomKVP>();
    public List<CustomKVP> time = new List<CustomKVP>();
    public List<Vector3> lookLocation = new List<Vector3>();
    public List<Vector3> playerEyePos = new List<Vector3>();
    public string pcName;
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="_lookAt">object viewed and time</param>
    /// <param name="_time">scene name and time spent in it</param>
    /// <param name="_lookLocation">point of hit contact</param>
    public AnalyticData(List<CustomKVP> _lookAt, List<CustomKVP> _time, List<Vector3> _lookLocation,List<Vector3> _playerEyePos,string _deviceName)
    {
        lookAt = _lookAt;
        time = _time;
        lookLocation = _lookLocation;
        playerEyePos = _playerEyePos;
        pcName = _deviceName;
    }
}