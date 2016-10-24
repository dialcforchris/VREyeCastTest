using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextInfo : MonoBehaviour
{

    public Text info;
    [SerializeField] private GameObject player;
    [SerializeField]
    float offset = 5;

 
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

	// Update is called once per frame
	void Update ()
    {
        Vector3 fudge = Camera.main.transform.position - transform.position;
        info.rectTransform.LookAt(Camera.main.transform.position -fudge);
        info.rectTransform.rotation = Camera.main.transform.rotation;
	}
}
