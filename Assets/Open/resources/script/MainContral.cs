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
        mousedrag();
    }
    /// <summary>
    /// 是否开启音乐
    /// </summary>
    public void Option()
    {
        GameObject Can = GameObject.Find("Canvas");
        if (Can.GetComponent<AudioSource>().volume == 1)
        {
            Can.GetComponent<AudioSource>().volume = 0;
        }
        else
        {
            Can.GetComponent<AudioSource>().volume = 1;
        }
    }
    /// <summary>
    /// 关闭游戏
    /// </summary>
    public void Quit()
    {
        Application.Quit();  
    }

    public void mousedrag()
    {
        if (Input.GetMouseButton(0))
        { 			// Check if there is a touch or mouse on some tile

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 30))
            {
                if(hit.transform.name=="Image(1)")
                {
                    hit.transform.position = Input.mousePosition;
                }
            }
        }
    }
    public void loadlevel()
    {
        Application.LoadLevel("Level");
    }
}
