using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextInfoPool : MonoBehaviour {
    public static TextInfoPool instance = null;
    List<TextInfo> textPool = new List<TextInfo>();
    [SerializeField]
    private TextInfo ti;
    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public TextInfo PoolingText()
    {
        for (int i = 0; i < textPool.Count; i++)
        {
            if (!textPool[i].isActiveAndEnabled)
            {
               textPool[i].enabled = true;
                return textPool[i];
            }
        }

       TextInfo t = Instantiate(ti);
        textPool.Add(t);
        return t;

    }
}

   
   
   

