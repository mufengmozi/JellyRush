using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StageBound
{
    class StageBind
    {
        static string filePath = Application.dataPath + "/Match3Sample/Scripts/Stage/Stage1.xml";
        /// <summary>
        /// 给stage类赋值
        /// </summary>
        /// <returns></returns>
        public static List<Stage> LoadLevels()
        {
            List<Stage> Stage = new List<Stage>();
            Stage sta=new Stage();
            string[] stagestring = XmlHelper.ReadNodesName(filePath, "Stage");
            foreach (string s in stagestring)
            {
                switch (s)
                {
                    case "Gate":
                        {
                            sta.GateName = XmlHelper.FindByName(filePath, s);
                            sta.IsBoss = Boolean.Parse(XmlHelper.FindAttrByName(filePath, s, "IsBoss"));
                            break;
                        }
                    case "GateTarget":
                        {
                            if (XmlHelper.FindAttrByName(filePath, s, "value") == "1")
                            {
                                sta.TargetPoint = XmlHelper.FindByName(filePath, "GateTarget", "TargetPoint");
                                sta.GateTarget = 1;
                            }
                            if (XmlHelper.FindAttrByName(filePath, s, "value") == "2")
                            {
                                sta.GateTarget = 2;
                            }
                            if (XmlHelper.FindAttrByName(filePath, s, "value") == "3")
                            {
                                sta.GateTarget = 3;
                            }
                            if (XmlHelper.FindAttrByName(filePath, s, "value") == "4")
                            {
                                sta.GateTarget = 4;
                            }
                            break;
                        }
                    case "GateLimit":
                        {
                            if (XmlHelper.FindAttrByName(filePath, s, "value") == "1")
                            {
                                sta.MoveLimit = float.Parse(XmlHelper.FindByName(filePath, "GateLimit", "MoveLimit"));
                                sta.GateLimit = 1;
                            }
                            if (XmlHelper.FindAttrByName(filePath, s, "value") == "2")
                            {
                                sta.TimeLimit = float.Parse(XmlHelper.FindByName(filePath, "GateLimit", "TimeLimit"));
                                sta.GateLimit = 2;
                            }
                            if (XmlHelper.FindAttrByName(filePath, s, "value") == "3")
                            {
                                sta.HealLimit = float.Parse(XmlHelper.FindByName(filePath, "GateLimit", "HealLimit"));
                                sta.GateLimit = 3;
                            }
                            break;
                        }
                    case "NumberOfColor":
                        {
                            sta.NumberOfColor = int.Parse(XmlHelper.FindByName(filePath, "NumberOfColor"));
                            break;
                        }
                    case "ScoreTarget":
                        {
                            sta.ScoreTarget = (XmlHelper.FindByName(filePath, "ScoreTarget"));
                            break;
                        }
                    case "Map":
                        {
                            sta.Map = (XmlHelper.FindByName(filePath, "Map"));
                            break;
                        }
                }
            }
            Stage.Add(sta);
            return Stage;
        }
        /// <summary>
        /// 对TargetJelly进行赋值
        /// </summary>
        /// <returns></returns>
        public static List<Stage.TargetJelly> LoadTargetJelly()
        {
            List<Stage.TargetJelly> stj = new List<Stage.TargetJelly>();
            StageBound.Stage.TargetJelly tj = new Stage.TargetJelly();
            tj.type = XmlHelper.FindAttrByName(filePath, "GateTarget", "TargetJelly", "type");
            tj.number = XmlHelper.FindAttrByName(filePath, "GateTarget", "TargetJelly", "number");
            stj.Add(tj);
            return stj;
        }
        /// <summary>
        /// 对Obstracle进行赋值
        /// </summary>
        /// <returns></returns>
        public static List<Stage.Obstracle> LoadObstracle()
        {
            List<Stage.Obstracle> sos = new List<Stage.Obstracle>();
            StageBound.Stage.Obstracle ob = new Stage.Obstracle();
            ob.type = XmlHelper.FindAttrByName(filePath, "GateTarget", "Obstracle", "type");
            ob.number = XmlHelper.FindAttrByName(filePath, "GateTarget", "Obstracle", "number");
            sos.Add(ob);
            return sos;
        }

        /// <summary>
        /// 对Pass进行赋值
        /// </summary>
        /// <returns></returns>
        public static List<Stage.Pass> LoadPass()
        {
            List<Stage.Pass> spass = new List<Stage.Pass>();
            StageBound.Stage.Pass pass = new Stage.Pass();
            pass.type = XmlHelper.FindAttrByName(filePath, "GateTarget", "Pass", "type");
            pass.number = XmlHelper.FindAttrByName(filePath, "GateTarget", "Pass", "number");
            spass.Add(pass);
            return spass;
        }
        /// <summary>
        /// 给boss类添加信息
        /// </summary>
        /// <returns></returns>
        public static List<Stage.BOSS> LoadBoss()
        {
            List<Stage.BOSS> sboss = new List<Stage.BOSS>();
            Stage.BOSS boss = new Stage.BOSS();
            boss.BossName = XmlHelper.FindByName(filePath, "BOSS", "BossName");
            boss.BossHeal = Int32.Parse(XmlHelper.FindByName(filePath, "BOSS", "BossHeal"));
            boss.BossSkill = Int32.Parse(XmlHelper.FindByName(filePath, "BOSS", "BossSkill"));
            boss.BossTrigger = Int32.Parse(XmlHelper.FindByName(filePath, "BOSS", "BossTrigger"));
            sboss.Add(boss);
            return sboss;
        }
    }
}
