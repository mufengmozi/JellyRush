using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using LevelContral;

public static class LevelSystem
{
    /// <summary>
    /// 加载Xml文件 
    /// </summary>
    /// <returns>The levels.</returns>
    public static List<Level> LoadLevels()
    {
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        //如果本地存在配置文件则读取配置文件
        //否则在本地创建配置文件的副本
        //为了跨平台及可读可写，需要使用Application.persistentDataPath
        string filePath = Application.dataPath + "/Level/resource/script/levels.xml";
        XmlNodeList levelsNode = XmlHelper.FindAttrAll(filePath, "/levels/level");
        //初始化关卡列表
        List<Level> levels = new List<Level>();
        foreach (XmlElement xe in levelsNode)
        {
            Level l = new Level();
            l.ID = xe.GetAttribute("id");
            l.Name = xe.GetAttribute("name");
            //使用unlock属性来标识当前关卡是否解锁
            if (xe.GetAttribute("unlock") == "1")
            {
                l.UnLock = true;
            }
            else
            {
                l.UnLock = false;
            }
            levels.Add(l);
        }
        return levels;
    }

    /// <summary>
    /// 设置某一关卡的状态
    /// </summary>
    /// <param name="name">关卡名称</param>
    /// <param name="locked">是否解锁</param>
    public static void SetLevels(string name, bool unlock)
    {
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        string filePath = Application.persistentDataPath + "/levels.xml";
        xmlDoc.Load(filePath);
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList levelsNode = root.SelectNodes("/levels/level");
        foreach (XmlElement xe in levelsNode)
        {
            //根据名称找到对应的关卡
            if (xe.GetAttribute("name") == name)
            {
                //根据unlock重新为关卡赋值
                if (unlock)
                {
                    xe.SetAttribute("unlock", "1");
                }
                else
                {
                    xe.SetAttribute("unlock", "0");
                }
            }
        }
        //保存文件
        xmlDoc.Save(filePath);
    }
}
