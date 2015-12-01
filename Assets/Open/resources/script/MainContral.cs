using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MainContral : MonoBehaviour {

   
	// Use this for initialization
	void Start () {
	
    }
	
	// Update is called once per frame
    void Update()
    {

    }

    public void Option()
    {
        GameObject Can = GameObject.Find("Canvas");
        Button bu = Can.GetComponentInChildren<Button>();
    }
}
