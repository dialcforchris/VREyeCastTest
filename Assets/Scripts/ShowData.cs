using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShowData : MonoBehaviour
{

    [SerializeField]
    private LineRenderer line;
    [SerializeField]
    private LineRenderer los;
    [SerializeField]
    private TextInfo ti;
    [SerializeField]
    private Canvas stats;
    [SerializeField]
    private Text scene;
    [SerializeField]
    private Text usedOn;
    List<AnalyticData> dataList = new List<AnalyticData>();
    AnalyticData data;
    public int sessionNo;
    public int lineIndex=0;
    bool done = false;
    [SerializeField]
    private LineRenderer playerPos;
   
	// Use this for initialization
	void Start ()
    {
        dataList = Analytics.instance.ReturnSavedData();
        Debug.Log(dataList.Count);
        line.SetVertexCount(0);
        playerPos.SetVertexCount(0);
        los.SetVertexCount(2);
        int machines = 0;
        for (int i=0;i<dataList.Count-1;i++)
        {
            List<string> names = new List<string>();
            if (!names.Contains(dataList[i].pcName))
            {
                names.Add(dataList[i].pcName);
            }
            machines = names.Count;
        }

        usedOn.text = "Played on "+machines.ToString() + " Computer"+(machines>1? "s" : "");
        GetSession(dataList.Count-1);
        line.SetPosition(0, data.lookLocation[0]);
        playerPos.SetPosition(0, data.playerEyePos[0]);
        los.SetPosition(0, data.playerEyePos[0]);
        los.SetPosition(1, data.lookLocation[0]);
      
        TextInfo t = TextInfoPool.instance.PoolingText();
        t.info.text = "START";
        t.info.color = Color.green;
        t.info.rectTransform.position = new Vector3(data.lookLocation[0].x,data.lookLocation[0].y-1,data.lookLocation[0].z);

        stats.gameObject.SetActive(true);
      
    }

    public void GetSession(int _sessionNumber)
    {

        data = dataList[_sessionNumber];
        Debug.Log(data.pcName);
        
       line.SetVertexCount(data.lookLocation.Count-1);
        for (int i=0;i<data.lookLocation.Count-1;i++)
        {
            line.SetPosition(i, data.lookLocation[0]);
        }
        playerPos.SetVertexCount(data.playerEyePos.Count - 1);
        for (int i=0;i<data.playerEyePos.Count-1;i++)
        {
            playerPos.SetPosition(i, data.playerEyePos[0]);
        }
        TextInfo tnfo = TextInfoPool.instance.PoolingText();
        tnfo.info.text = "Player Pos";
        tnfo.info.color = Color.cyan;
        tnfo.info.rectTransform.position = data.playerEyePos[0];
        sessionNo = _sessionNumber;
        ShowObjectInfo();
        if (data.lookAt==null)
        {
            Debug.Log("data.lookat didn't work");
        }
        else
        Debug.Log("lookat count "+data.lookAt.Count);
        if (data.time==null)
        {
            Debug.Log("time didnlt work");
        }
        else
        {
            foreach (CustomKVP kvp in data.time)
            {
                scene.text = "Scene Name: "+kvp.key + "\nTime in Scene: " + kvp.value;
             //   Debug.Log("Scene name and Time spent "+kvp.key + " " + kvp.value);
            }
        }
    }

    void ShowObjectInfo()
    {
       foreach (CustomKVP kvp in data.lookAt)
        {
            GameObject g = GameObject.Find(kvp.key);
            TextInfo t = TextInfoPool.instance.PoolingText();
            t.info.text = "Object " + kvp.key+"\n Viewed for " +kvp.value.ToString();
            float offset = g.GetComponent<MeshCollider>().bounds.size.y;
            t.info.rectTransform.position = new Vector3(g.transform.position.x,g.transform.position.y+offset,g.transform.position.z);
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (!done)
        {
            StartCoroutine(AnimatePath());
        }
	}
    public void ChangeDataSet()
    {
        if (sessionNo>=dataList.Count-1)
        {
            sessionNo = 0;
        }
        else
        {
            sessionNo++;
        }
        done = false;
    }
   IEnumerator AnimatePath()
    {
        for (int i = 1; i < data.lookLocation.Count - 1;i++)
        {
            los.SetPosition(0, data.playerEyePos[i]);
           
            line.SetVertexCount(i+1);
            playerPos.SetVertexCount(i + 1);
            Vector3 start = data.lookLocation[i-1];
            Vector3 current = start;
            float move = Vector3.Distance(current, data.lookLocation[i]) + Time.fixedDeltaTime;
            while (current!=data.lookLocation[i])
            {
                current = Vector3.MoveTowards(current,data.lookLocation[i],move);
                line.SetPosition(i, current);
                los.SetPosition(1, current);
                playerPos.SetPosition(i, data.playerEyePos[i]);
                yield return new WaitForEndOfFrame();
            }
        }
        TextInfo t = TextInfoPool.instance.PoolingText();
        t.info.text = "END";
        t.info.color = Color.red;
        t.info.rectTransform.position = new Vector3(data.lookLocation[data.lookLocation.Count - 1].x,
            data.lookLocation[data.lookLocation.Count - 1].y - 1, data.lookLocation[data.lookLocation.Count - 1].z);
        done = true;
    }
}
