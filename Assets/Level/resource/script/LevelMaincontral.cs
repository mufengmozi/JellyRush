using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using LevelContral;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelMaincontral : MonoBehaviour {

    public GameObject level;
    private List<Level> m_levels;
	// Use this for initialization
    void Start()
    {
        //获取关卡
        m_levels = LevelSystem.LoadLevels();
        //动态生成关卡
        foreach (Level l in m_levels)
        {
            GameObject prefab = GameObject.Find(l.Name);
            //数据绑定
            DataBind(prefab, l);
        }
        //人为解锁第二个关卡
        //在实际游戏中玩家需要满足一定条件方可解锁关卡
        //此处仅作为演示
        //LevelSystem.SetLevels("level1", true);
    }


    /// <summary>
    /// 数据绑定
    /// </summary>
    void DataBind(GameObject go, Level level)
    {
        if (level.UnLock)
        {
            go.GetComponent<Button>().interactable = true;
        }
        else
        {
            go.GetComponent<Button>().interactable = false;
        }
    }
    public void ChoiceLevel()
    {
        int levelcount = Int32.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(EventSystem.current.currentSelectedGameObject.name.Length - 1, 1));
        PlayerPrefs.SetInt("Level", levelcount);
        Debug.Log(PlayerPrefs.GetInt("Level"));
        Application.LoadLevel(2);
    }
}
