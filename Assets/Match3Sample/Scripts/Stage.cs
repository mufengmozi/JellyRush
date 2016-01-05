using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StageBound
{
    public class Stage
    {
        //关卡名称
        public string GateName;
        //判断是否boss关卡
        public bool IsBoss=false;
        //目标分数
        public string TargetPoint;
        //游戏胜利标识
        public int GateTarget;
        //目标果冻
        public struct TargetJelly
        {
            public string  type,number;
        }
        //目标障碍
        public struct Obstracle
        {
            public string type, number;
        }
        //目标传送
        public struct Pass
        {
            public string type, number;
        }
        //限制类型标识
        public int GateLimit;
        //限制步数
        public float TimeLimit;
        //限制数量
        public float MoveLimit;
        //生命
        public float HealLimit;
        //果冻的数量
        public int NumberOfColor;
        //记录达标分数
        public string ScoreTarget;
        //BOSS类
        public class BOSS
        {
            public string BossName;
            //boss技能代号
            public int BossSkill;
            //boss技能触发模式
            public int BossTrigger;
            //boss血量
            public int BossHeal;
        }
        //MAP信息
        public string Map;
    }
}
