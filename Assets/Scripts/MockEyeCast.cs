using UnityEngine;
using System.Collections;

public class MockEyeCast : MonoBehaviour
{
    [SerializeField]private float length;
    // Use this for initialization
   	// Update is called once per frame
	void Update ()
    {
        MockCast();
	}

    void MockCast()
    {
        Debug.DrawRay(transform.position, transform.forward * length, Color.blue);
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, length))
        {
            Analytics.instance.CollectInformation(hit);
            //For VR check if interactable and then do stuff
        }
    }
}
