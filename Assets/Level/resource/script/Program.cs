using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LevelContral
{
    class Program : MonoBehaviour
    {
        static GameObject Can; //得到ugui中canvas并进行操纵
        //关卡列表
        private List<Level> m_levels;

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
            LevelSystem.SetLevels("level1", true);
        }


        /// <summary>
        /// 数据绑定
        /// </summary>
        void DataBind(GameObject go, Level level)
        {   
            if (level.UnLock)
            {
                go.GetComponent<Button>().interactable = false;
            }
            else
            {
                go.GetComponent<Button>().interactable = true;
            }
        }
    }
}
