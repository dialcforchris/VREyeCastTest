using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BarChart : MonoBehaviour {

    List<Bar> bars = new List<Bar>();
    [SerializeField] private Bar bar;
    float chartHeight;
    public float[] values;
    public float maxValue;
    public string[] names;
  
	// Use this for initialization
	void Start ()
    {
        chartHeight =  GetComponent<RectTransform>().sizeDelta.y;
        maxValue = values.Max();
        ShowGraph();
	}
	void ShowGraph()//List<CustomKVP> barData)
    {
        for (int i=0;i<values.Length;i++)//barData.Count;i++)
        {
            Bar b = (Bar)Instantiate(bar);
            b.transform.SetParent(transform);
            RectTransform trans = b.bar.GetComponent<RectTransform>();
            b.info.text = names[i];
            trans.position = transform.position;
            trans.rotation = transform.rotation;
//            trans.sizeDelta = new Vector2(trans.sizeDelta.x, chartHeight*barData[i].value);
            trans.sizeDelta = new Vector3(trans.sizeDelta.x, chartHeight * values[i]);

        }
    }
}
