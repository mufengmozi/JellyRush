/* Copyright (c) Vander Amaral
 * I've created this code based on a lot of Match3 Codes I found on the Internet
 * I tried to make it the best and easy to change.
 * You can polish it even more, and add functions to it if you bought.
 * It is NOT ok to sell this code, you can only sell the final product ie. the final game
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{

    /// <summary>
    /// 定义面板状态
    /// </summary>
    public enum BState
    {  						//State of the Board
        PLAYING,
        DROP_JEWELS,
        SWAPPING,
        GAMEOVER,
        RESETING
    };
    public static int CurrentlyMovingJewels; 	//控制宝石的移动. Controlled by Jewel.cs
    protected BState theBoardState;				//标示当前面板的状态	
    protected bool isActive;					//标示当前面板是否激活			
    protected bool canSwap;						//标示当前是否可以移动		
    protected string xmlpath;
    /// <summary>
    /// 列表用于记录宝石应该被破坏还是保存
    /// </summary>
    private List<GameObject> jewelRemains;		//列表用于记录宝石应该被破坏还是保存
    private bool needToDropJewels;				//Controls the Dropping Jewels
    /// <summary>
    /// 记录消除了多少个宝石（可用于计算分数）
    /// </summary>
    private int totalRemovedJewels;
    public GameObject SelectedJewel;			//Can be used as Prefab holder for highlighting a selected jewel
    private Jewel tip;							//Control the Jewel Alpha hint
    public AudioClip SwapSound;					//SwapSound
    public AudioClip explodeSound;				//ExplodeSound
    public AudioClip match3Sound;				//Match3Sound
    private bool needToCheckCascades = false;	//判断是否需要检查宝石当前状态
    private bool mouseLeftDown = false;			//检查鼠标按键是否抬起
    private int totalTriplets = 0;				//连3的计数
    private int movesLeft = 0;                  //记录当前可以移动的步数

    #region 特殊方块使用参数
    /// <summary>
    /// 判断是否存在方块消
    /// </summary>
    protected bool SquareMade;
    /// <summary>
    /// 计算方块消的数量
    /// </summary>
    protected int SquareMadeCount = 0;
    /// <summary>
    /// 判断是否存在横向连四消除
    /// </summary>
    protected bool FourMadeH;
    /// <summary>
    /// 计算横向四连消数量
    /// </summary>
    protected int FourMadeHCount = 0;
    /// <summary>
    /// 判断是否存在纵向连四消除
    /// </summary>
    protected bool FourMadeV;
    /// <summary>
    /// 计算纵向四连消数量
    /// </summary>
    protected int FourMadeVCount = 0;
    /// <summary>
    /// 判断是否存在连五消除
    /// </summary>
    protected bool FiveMade;
    /// <summary>
    /// 计算五连消数量
    /// </summary>
    protected int FiveMadeCount = 0;
    /// <summary>
    /// 判断是否存在TL消除
    /// </summary>
    protected bool TLMade;
    /// <summary>
    /// 计算TL消除数量
    /// </summary>
    protected int TLMadeCount = 0;
    /// <summary>
    /// 获取特殊方块的种类
    /// </summary>
    protected int jellytype = -1;
    /// <summary>
    /// 获取特殊方块作用种类
    /// </summary>
    protected int jellySpetype = -1;
    /// <summary>
    /// 横向四消特殊宝石列
    /// </summary>
    private List<GameObject> FourHJewels;
    /// <summary>
    /// 纵向四消特殊宝石列
    /// </summary>
    private List<GameObject> FourVJewels;
    /// <summary>
    /// 五消特殊宝石列
    /// </summary>
    private List<GameObject> FiveJewels;
    /// <summary>
    /// 方块特殊宝石列
    /// </summary>
    private List<GameObject> SquareJewels;
    /// <summary>
    /// TL特殊宝石列
    /// </summary>
    private List<GameObject> TLJewels;
    #endregion

    public GameObject Rock;						//Can be used as Rock or special item 
    public GameObject[] Obs;                     //这个是障碍物
    public GameObject[] planes;                 //这个是底部的面板
    public GameObject[] jewels;                //这是用在游戏上的宝石至少需要3种不同的
    public GameObject[] jewelsSpe;                 //这是用在游戏上的宝石至少需要3种不同的
    /// <summary>
    /// 确定该位置存在虚拟宝石
    /// </summary>
    public static GameObject[,] jewelMapPosition;  //Hold's the jewels virtual position 
    public static GameObject[,] planeMapPosition;  //Hold's the plane virtual position 
    public static GameObject[,] obsMapPosition;    //Hold's the obstruction virtual position
    private struct theSwap
    {				//helper for the swap I grab this part of the code from some websites
        public bool twiceClick;				//handles the double click
        public int jewelAx, jewelAy;		//handles the Jewel A position to make the swap
        public int jewelBx, jewelBy;		//handles the Jewel B position to make the swap
        public bool isActive;				//the swap is active?

        //给上述变量初始化
        public void Init()
        {				//Resets the variables above to restart the swap
            jewelAx = jewelAy = -1;
            jewelBx = jewelBy = -1;
            isActive = false;
            twiceClick = false;
        }
    };
    private theSwap swapping;				//if the player is swapping	
    private bool validateSwap;				//Validate the swap	
    public int boardSize;					// That's the board size, needs to be greater than 3
    private float delayDrop;				// 控制掉落宝石的延时

    private int boardSizex;                 //新面板的X值
    private int boardSizey;                 //新面板的Y值

    private List<GameObject> hintJewels;	//controls the Hint Jewels	//提示宝石的序列
    private float hintTimer;				//hint timer
    private float hintDelay;				//hint timer delay

    private int baseScore;					//that's the base score of the game I've set it to 100 but you can change
    private int longestChain;				//controls the longest chain
    protected int mouseClickX, mouseClickY;	//controls the mouseclick based on the jewel position

    private int skullInitial = 100;  		//how many points the skull will reduce消除骷髅减分

    public Texture2D flash;					//Texture for the Screen Flash
    public static bool flashIt;				//Flashes the Screen if true
    private float aFlash = 1;				//Alpha that controls the flash timer 闪烁时间

    private float ScoreBoostTimer;			//You can use this variable to boost your score for some time
    private int ScoreBoostValue;			//value if you want to use the scoreboost
    public Transform TheBoard;				//that's the board GameObject where the board will be placed

    GameObject Can; //得到ugui中canvas并进行操纵

    /// <summary>
    /// 程序入口
    /// </summary>
    void Start()
    {
        Reset();
        Debug.Log(PlayerPrefs.GetInt("Level"));
    }
    /// <summary>
    /// 面板状态展示
    /// </summary>
    void Text()
    {
        Text[] text = Can.GetComponentsInChildren<Text>();
        for (int i = 0; i < text.Length; i++)
        {
            switch (text[i].name)
            {
                case "isActive":
                    text[i].text = text[i].name + ":" + isActive;
                    break;
                case "canSwap":
                    text[i].text = text[i].name + ":" + canSwap;
                    break;
                case "BState":
                    text[i].text = text[i].name + ":" + theBoardState;
                    break;
                case "validateSwap":
                    text[i].text = text[i].name + ":" + validateSwap;
                    break;
                case "needToDropJewels":
                    text[i].text = text[i].name + ":" + needToDropJewels;
                    break;
                case "jewelBx":
                    text[i].text = text[i].name + ":" + swapping.jewelBx;
                    break;
                case "jewelBy":
                    text[i].text = text[i].name + ":" + swapping.jewelBy;
                    break;
                case "swapping.isActive":
                    text[i].text = text[i].name + ":" + swapping.isActive;
                    break;
            }
        }
    }

    void adapt()
    {
        GameObject cam = GameObject.Find("Main Camera");
    }

    /// <summary>
    /// 定义面板
    /// </summary>
    public void Reset()
    {
        xmlpath = Application.dataPath + "/Match3Sample/Scripts/GateCon.xml";
        Can = GameObject.Find("Canvas");
        if (boardSize < 3) boardSize = 8;
        theBoardState = BState.RESETING;
        jewelMapPosition = new GameObject[boardSize, boardSize];
        planeMapPosition = new GameObject[boardSize, boardSize];
        obsMapPosition = new GameObject[boardSize, boardSize];
        mouseClickY = -1;
        mouseClickX = -1;
        CurrentlyMovingJewels = 0;
        needToDropJewels = false;
        totalRemovedJewels = 0;
        validateSwap = false;
        hintTimer = 0;
        hintDelay = 5;
        baseScore = 100;
        longestChain = 0;
        isActive = true;
        canSwap = true;
        hintJewels = new List<GameObject>(16);
        jewelRemains = new List<GameObject>(16);
        FourHJewels = new List<GameObject>(16);
        FourVJewels = new List<GameObject>(16);
        FiveJewels = new List<GameObject>(16);
        SquareJewels = new List<GameObject>(16);
        TLJewels = new List<GameObject>(16);
        // Randomize the jewels
        do
        {
            ResetPlane();
            ResetObs();
            ResetJewelss();
        }
        while ((movesLeft = HowManyMovesLeft()) == 0);

        theBoardState = BState.PLAYING;
    }
    /// <summary>
    /// 定义好面板后开始给里边填充宝石
    /// </summary>
    private void ResetPlane()
    {
        string gate = XmlHelper.FindByName(xmlpath, "Map");
        Array arr = gate.Split(';');
        string line;
        boardSizex = arr.Length;
        GameObject pl;
        GameObject objectType1;
        GameObject planePrefab;
        for (int i = 0; i < arr.Length; i++)
        {
            Match m = Regex.Match(arr.GetValue(i).ToString(), @"\[([\s\S]*?)\]");
            if (m.Success)
            {
                line = m.ToString().Substring(m.ToString().IndexOf("[") + 1, (m.ToString().IndexOf("]") - m.ToString().IndexOf("[")) - 1);
                Array arrl = line.Split(',');
                for (int j = arrl.Length - 1; j >= 0; j--)
                {
                    if (arrl.GetValue(arrl.Length - 1 - j).ToString() != "0")
                    {
                        pl = (GameObject)planeMapPosition[j, i];
                        if (pl == null)
                        {
                            // 排布的时候要避免出现3个的情况
                            objectType1 = planes[0];//添加一个背景实体
                            //获取到宝石后实例化一个宝石预设体
                            planePrefab = (GameObject)Instantiate(objectType1, new Vector3(j, i, 0.01f), planes[0].transform.localRotation);
                            planeMapPosition[j, i] = planePrefab;//放入对应坐标
                            planePrefab.transform.parent = TheBoard;
                            planePrefab.transform.localPosition = new Vector3(j, i, 0.01f);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 配置障碍信息
    /// </summary>
    public void ResetObs()
    {
        string gate = XmlHelper.FindByName(xmlpath, "Map");
        Array arr = gate.Split(';');
        string line;
        boardSizex = arr.Length;
        GameObject obs;
        GameObject obsType1;
        GameObject obsPrefab;
        for (int i = 0; i < arr.Length; i++)
        {
            Match m = Regex.Match(arr.GetValue(i).ToString(), @"\[([\s\S]*?)\]");
            if (m.Success)
            {
                line = m.ToString().Substring(m.ToString().IndexOf("[") + 1, (m.ToString().IndexOf("]") - m.ToString().IndexOf("[")) - 1);
                Array arrl = line.Split(',');
                for (int j = arrl.Length - 1; j >= 0; j--)
                {
                    if (arrl.GetValue(arrl.Length - 1 - j).ToString().Length == 3)
                    {
                        obs = (GameObject)obsMapPosition[j, i];
                        if (obs == null)
                        {
                            obsType1 = Obs[0];
                            obsPrefab = (GameObject)Instantiate(obsType1, new Vector3(j, i, -0.01f), Obs[0].transform.localRotation);
                            obsMapPosition[j, i] = obsPrefab;//放入对应坐标 = obsPrefab;//放入对应坐标
                            obsPrefab.transform.parent = TheBoard;
                            obsPrefab.transform.localPosition = new Vector3(j, i, -0.01f);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 定义好面板后开始给里边填充宝石
    /// </summary>
    private void ResetJewels()
    {
        int x, y;
        //布置面板宝石之前先删除所有的宝石
        //for (y = 0; y < boardSize; y++)
        //{
        //    for (x = 0; x < boardSize; x++)
        //    {
        //        GameObject j = (GameObject)jewelMapPosition[x, y];
        //        Material ma = j.GetComponent<Renderer>().material;
        //        if (ma.mainTexture.name == "Jewels")
        //        {
        //            Destroy(jewelMapPosition[x, y]);
        //            jewelMapPosition[x, y] = null;
        //        }
        //    }
        //}
        //开始布置宝石1列1列的排版
        for (y = 0; y < boardSize; y++)
        {
            for (x = 0; x < boardSize; x++)
            {
                GameObject j = (GameObject)jewelMapPosition[x, y];
                GameObject pl = (GameObject)planeMapPosition[x, y];
                GameObject ob = (GameObject)obsMapPosition[x, y];

                if (j == null && pl != null && ob == null)
                {
                    // 排布的时候要避免出现3个的情况

                    GameObject objectType1 = jewels[UnityEngine.Random.Range(0, jewels.Length - 3)];//随机添加一个宝石
                    //获取到宝石后实例化一个宝石预设体
                    GameObject jewelPrefab = (GameObject)Instantiate(objectType1, new Vector3(x, y, 0), jewels[0].transform.localRotation);
                    jewelMapPosition[x, y] = jewelPrefab;//放入对应坐标
                    jewelPrefab.transform.parent = TheBoard;
                    jewelPrefab.transform.localPosition = new Vector3(x, y, 0);
                    //循环判断连3情况
                    while (true)
                    {
                        if (CountMatch3() == 0) break;//判断是否存在连3的情况，不存在跳出循环，添加下一组
                        //存在连3情况：销毁当前宝石，重新实例化一个新宝石
                        Destroy(jewelMapPosition[x, y]);
                        GameObject objectType2 = jewels[UnityEngine.Random.Range(0, jewels.Length)];
                        GameObject jewelPrefab2 = (GameObject)Instantiate(objectType2, new Vector3(x, y, 0), jewels[0].transform.localRotation);
                        jewelMapPosition[x, y] = jewelPrefab2;
                        jewelPrefab2.transform.parent = TheBoard;
                        jewelPrefab2.transform.localPosition = new Vector3(x, y, 0);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 定义好面板后开始给里边填充宝石
    /// </summary>
    private void ResetJewelss()
    {
        string gate = XmlHelper.FindByName(xmlpath, "Map");
        Array arr = gate.Split(';');
        GameObject objectType1;
        string line;
        for (int i = 0; i < arr.Length; i++)
        {
            Match m = Regex.Match(arr.GetValue(i).ToString(), @"\[([\s\S]*?)\]");
            if (m.Success)
            {
                line = m.ToString().Substring(m.ToString().IndexOf("[") + 1, (m.ToString().IndexOf("]") - m.ToString().IndexOf("[")) - 1);
                Array arrl = line.Split(',');
                for (int j = arrl.Length - 1; j >= 0; j--)
                {
                    GameObject je = (GameObject)jewelMapPosition[j, i];
                    GameObject pl = (GameObject)planeMapPosition[j, i];
                    GameObject ob = (GameObject)obsMapPosition[j, i];
                    int no = arrl.Length - 1 - j;
                    if (je == null && pl != null && ob == null)
                    {
                        if (arrl.GetValue(no).ToString().Length == 4)
                        {
                            int JewelType = Int32.Parse(arrl.GetValue(no).ToString().Substring(3, arrl.GetValue(no).ToString().Length - 3));
                            int SpeType = Int32.Parse(arrl.GetValue(no).ToString().Substring(0, arrl.GetValue(no).ToString().Length - 3));
                            MadeSpe(j, i, JewelType, SpeType);
                        }
                        else
                        {
                            if (arrl.GetValue(no).ToString() == "3")
                            {
                                objectType1 = jewels[3];//添加一个特定宝石
                            }
                            else
                            {
                                objectType1 = jewels[UnityEngine.Random.Range(0, jewels.Length - 3)];//随机添加一个宝石
                            }
                            GameObject jewelPrefab = (GameObject)Instantiate(objectType1, new Vector3(j, i, 0), jewels[0].transform.localRotation);
                            jewelMapPosition[j, i] = jewelPrefab;//放入对应坐标
                            jewelPrefab.transform.parent = TheBoard;
                            jewelPrefab.transform.localPosition = new Vector3(j, i, 0);
                            //循环判断连3情况
                            while (true)
                            {
                                if (CountMatch3() == 0) break;//判断是否存在连3的情况，不存在跳出循环，添加下一组
                                //存在连3情况：销毁当前宝石，重新实例化一个新宝石
                                Destroy(jewelMapPosition[j, i]);
                                GameObject objectType2 = jewels[UnityEngine.Random.Range(0, jewels.Length)];
                                GameObject jewelPrefab2 = (GameObject)Instantiate(objectType2, new Vector3(i, j, 0), jewels[0].transform.localRotation);
                                jewelMapPosition[j, i] = jewelPrefab2;
                                jewelPrefab2.transform.parent = TheBoard;
                                jewelPrefab2.transform.localPosition = new Vector3(j, i, 0);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 逻辑流程
    /// </summary>
    void Update()
    {
        Text();
        if (theBoardState == BState.RESETING) return; //查看面板当前状态

        UpdateJewelRemains();  						//更新宝石状态
        //判断面板是否处于激活状态
        if (!isActive) return; 						//面板处于移动状态时处于未激活状态
        //判断当前可移动的宝石有几个
        //if (CurrentlyMovingJewels > 0) return;

        // delayDrop时间后宝石开始掉落 
        if (delayDrop > 0)
        {
            delayDrop -= (float)Time.deltaTime;
            return;
        }
        // 如果移动宝石大于0
        if (CurrentlyMovingJewels > 0)
            theBoardState = BState.DROP_JEWELS;
        else
        {
            if (needToCheckCascades)
            {
                CheckForCascades();
                needToCheckCascades = false;
            }
            //Debug.Log(swapping.jewelBx + "" + swapping.jewelBy);
            if (validateSwap)//出现了交换情况
            { 			// 交换以后验证宝石位置是否存在连3
                int count = CountMatch3();
                if (count == 0)
                {
                    SwapJewels(swapping);	// 如果不存在就换回去
                    // Play a sound
                    //do something due illegal move
                }
                else
                {
                    RemoveTriplets(count);
                }
                validateSwap = false;
            }
            if (swapping.isActive)
                theBoardState = BState.SWAPPING;
            else
                theBoardState = BState.PLAYING;
        }

        if (theBoardState != BState.PLAYING && theBoardState != BState.SWAPPING) return; // 判断用户是否可以进行交互

        if (theBoardState == BState.PLAYING)
        { 	// Increase the hint timer if there's no action on the board
            hintTimer += (float)Time.deltaTime;
            if (hintTimer >= hintDelay)			// Reset the hint delay
                ShowRandomHint();
        }

        if (Input.GetMouseButton(0))
        { 			// Check if there is a touch or mouse on some tile

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 30))
            {
                if (hit.transform.tag != "Untagged")
                {
                    mouseClickX = (int)hit.transform.localPosition.x;
                    mouseClickY = (int)hit.transform.localPosition.y;
                    //SelectedJewel.transform.position = new Vector3(hit.transform.position.x,hit.transform.position.y,-1.02f);
                }
                else
                {
                    mouseClickY = -1;
                    mouseClickX = -1;
                    //SelectedJewel.transform.position = new Vector3(-6,-6,-1.02f);
                }
            }
        }

        //if (swapping.twiceClick == true && swapping.isActive)
        //{
        //    swapping.jewelBx = mouseClickX;
        //    swapping.jewelBy = mouseClickY;
        //    //Debug.Log("di san ci"+swapping.jewelBx + "" + swapping.jewelBy);
        //}

        if (canSwap && mouseClickX > -1 && Input.GetMouseButton(0))
        { // Start to swap if some jewel is clicked

            if (theBoardState != BState.SWAPPING && !mouseLeftDown)
            {
                swapping.Init();
                swapping.isActive = true;
                swapping.jewelAx = mouseClickX;
                swapping.jewelAy = mouseClickY;
                theBoardState = BState.SWAPPING;
                mouseLeftDown = true;
            }
            else
            {
                //Debug.Log("di si ci"+swapping.jewelBx + "" + swapping.jewelBy);
                swapping.jewelBx = mouseClickX;
                swapping.jewelBy = mouseClickY;

                int dx = swapping.jewelAx - swapping.jewelBx;
                int dy = swapping.jewelAy - swapping.jewelBy;
                if (Mathf.Abs(dx) > 1 || Mathf.Abs(dy) > 1)
                {
                    swapping.isActive = false;
                    theBoardState = BState.PLAYING;
                }
            }
        }

        if (canSwap && Input.GetMouseButton(0))
        { // When the mouse button or touch is released..
            mouseLeftDown = false;
            if (tip)
            {
                tip.alpha = false;
                tip = null;
            }
            if (theBoardState == BState.SWAPPING)
            {
                // If we have released on the same jewel use alternate swapping method (two clicks)
                if (mouseClickX == swapping.jewelAx && mouseClickY == swapping.jewelAy)
                {
                    //swapping.twiceClick = true;
                }
                else if (!IsLegalSwap())
                { 			//Cancels Swap
                    swapping.isActive = false;
                    theBoardState = BState.PLAYING;
                    //SelectedJewel.transform.localPosition = new Vector3(-6,-6,-1.02f);
                }
                else if (!swapping.twiceClick)
                { 		//Swap
                    GetComponent<AudioSource>().pitch = 1;
                    GetComponent<AudioSource>().PlayOneShot(SwapSound);
                    swapping.isActive = false;
                    SwapJewels(swapping);
                    validateSwap = true;				// Need to validate it
                    //SelectedJewel.transform.localPosition = new Vector3(-6,-6,-1.02f);
                }
                mouseClickY = -1;
                mouseClickX = -1;
                //SelectedJewel.transform.localPosition = new Vector3(-6,-6,-1.02f);
            }
        }

        if (needToDropJewels && delayDrop <= 0)
        { // Check for drop jewels
            needToDropJewels = false;
            CheckForFallingJewels();
        }

        if (CurrentlyMovingJewels > 0)
        {
            //do something when the jewels are moving
        }
        //Debug.Log("di wu ci"+swapping.jewelBx + "" + swapping.jewelBy);
    }

    #region 暂时不用的面板分布系统
    private void ResetJewelse()
    {
        string gate = XmlHelper.FindByName(xmlpath, "Map");
        Array arr = gate.Split(';');
        string line;
        GameObject je;//用于记录宝石的位置
        GameObject pl;//用于记录面板的位置
        GameObject obs;//用于记录障碍的位置
        GameObject objectType1;//记录宝石的种类
        GameObject jewelPrefab;
        GameObject objectType2;
        GameObject jewelPrefab2;
        GameObject obsType1;
        GameObject obsPrefab;
        for (int j = 0; j < arr.Length; j++)
        {
            Match m = Regex.Match(arr.GetValue(j).ToString(), @"\[([\s\S]*?)\]");
            if (m.Success)
            {
                line = m.ToString().Substring(m.ToString().IndexOf("[") + 1, (m.ToString().IndexOf("]") - m.ToString().IndexOf("[")) - 1);
                Array arrl = line.Split(',');
                for (int i = arrl.Length - 1; i >= 0; i--)
                {
                    je = (GameObject)jewelMapPosition[i, j];
                    pl = (GameObject)planeMapPosition[i, j];
                    obs = (GameObject)obsMapPosition[i, j];
                    int no = arrl.Length - 1 - i;
                    if (arrl.GetValue(no).ToString().Substring(arrl.GetValue(no).ToString().Length - 1, 1) != "0")
                    {
                        if (je == null && pl != null)
                        {
                            // 排布的时候要避免出现3个的情况
                            objectType1 = jewels[UnityEngine.Random.Range(0, jewels.Length)];//随机添加一个宝石
                            //获取到宝石后实例化一个宝石预设体
                            jewelPrefab = (GameObject)Instantiate(objectType1, new Vector3(i, j, 0), jewels[0].transform.localRotation);
                            jewelMapPosition[i, j] = jewelPrefab;//放入对应坐标
                            jewelPrefab.transform.parent = TheBoard;
                            jewelPrefab.transform.localPosition = new Vector3(i, j, 0);
                            //循环判断连3情况
                            while (true)
                            {
                                if (CountMatch3() == 0) break;//判断是否存在连3的情况，不存在跳出循环，添加下一组
                                //存在连3情况：销毁当前宝石，重新实例化一个新宝石
                                Destroy(jewelMapPosition[i, j]);
                                objectType2 = jewels[UnityEngine.Random.Range(0, jewels.Length)];
                                jewelPrefab2 = (GameObject)Instantiate(objectType2, new Vector3(i, j, 0), jewels[0].transform.localRotation);
                                je = jewelPrefab2;
                                jewelPrefab2.transform.parent = TheBoard;
                                jewelPrefab2.transform.localPosition = new Vector3(i, j, 0);
                            }
                        }
                    }

                    switch (arrl.GetValue(no).ToString().Length)
                    {
                        case 2:
                            {
                                if (arrl.GetValue(no).ToString().Substring(0, arrl.GetValue(no).ToString().Length - 1) != "0")
                                {
                                    pl.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Plane/square");
                                }
                                break;
                            }
                        case 3:
                            {
                                if (arrl.GetValue(no).ToString().Substring(1, arrl.GetValue(no).ToString().Length - 2) != "0")
                                {
                                    pl.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Plane/square");
                                }
                                if (arrl.GetValue(no).ToString().Substring(0, arrl.GetValue(no).ToString().Length - 2) != "0")
                                {
                                    obsType1 = Obs[0];
                                    obsPrefab = (GameObject)Instantiate(obsType1, new Vector3(i, j, -0.01f), Obs[0].transform.localRotation);
                                    obs = obsPrefab;//放入对应坐标

                                    obsPrefab.transform.parent = TheBoard;
                                    obsPrefab.transform.localPosition = new Vector3(i, j, -0.01f);
                                }
                                break;
                            }
                        case 4:
                            {
                                if (arrl.GetValue(no).ToString().Substring(2, arrl.GetValue(no).ToString().Length - 3) != "0")
                                {
                                    pl = (GameObject)planeMapPosition[i, j];
                                    pl.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Plane/square");
                                }
                                if (arrl.GetValue(no).ToString().Substring(1, arrl.GetValue(no).ToString().Length - 3) != "0")
                                {

                                }
                                if (arrl.GetValue(no).ToString().Substring(0, arrl.GetValue(no).ToString().Length - 3) != "0")
                                {

                                }
                                break;
                            }
                        case 5:
                            {
                                if (arrl.GetValue(no).ToString().Substring(3, arrl.GetValue(no).ToString().Length - 4) != "0")
                                {
                                    pl = (GameObject)planeMapPosition[i, j];
                                    pl.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Plane/square");
                                }
                                if (arrl.GetValue(no).ToString().Substring(2, arrl.GetValue(no).ToString().Length - 4) != "0")
                                {

                                }
                                if (arrl.GetValue(no).ToString().Substring(1, arrl.GetValue(no).ToString().Length - 4) != "0")
                                {

                                }
                                if (arrl.GetValue(no).ToString().Substring(0, arrl.GetValue(no).ToString().Length - 4) != "0")
                                {

                                }
                                break;
                            }
                    }
                }
            }
        }
    }
    #endregion
    /// <summary>
    /// 消除该种类的一定数量的宝石
    /// </summary>
    /// <param name="tagType">宝石种类</param>
    /// <param name="howMuch">数量</param>
    public void DestroyJewelType(string tagType, int howMuch)
    {
        // Destroy the old jewels
        int isRemoveJewel = 0;

        if (howMuch == 0)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (IsJewelAt(x, y))
                    {
                        if (jewelMapPosition[x, y].tag == tagType)
                        {
                            RemoveJewelAt(x, y);
                            isRemoveJewel++;
                        }
                    }
                }
            }
        }
        else
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (IsJewelAt(x, y))
                    {
                        if (jewelMapPosition[x, y].tag == tagType && isRemoveJewel < howMuch)
                        {
                            RemoveJewelAt(x, y);
                            isRemoveJewel++;
                        }
                    }
                }
            }
        }


        if (isRemoveJewel > 0)
        {  //Was something removed?
            needToDropJewels = true;
            delayDrop = 0.75f;//0.75f;
            canSwap = false;
            GetComponent<AudioSource>().PlayOneShot(explodeSound);
            CameraShake.shakeFor(0.5f, 0.1f); //Shake the screen
            flashIt = true;
        }

    }
    /// <summary>
    /// 当没有可以移动的宝石的时候
    /// 重置宝石
    /// </summary>
    private void PopulateWithNewJewels()
    {
        CurrentlyMovingJewels = 0;
        theBoardState = BState.PLAYING;
        needToDropJewels = false;
        canSwap = true;

        // Destroy the old jewels
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (IsJewelAt(x, y))
                {
                    GameObject j = (GameObject)jewelMapPosition[x, y];
                    Material ma = j.GetComponent<Renderer>().material;
                    if (ma.mainTexture.name == "Jewels")
                    {
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jewelMapPosition[x, y] = null;
                        jj.Die();
                        jewelRemains.Add(j);
                    }
                }
            }
        }
        GetComponent<AudioSource>().PlayOneShot(explodeSound);

        // Randomize new jewels
        do
        {
            ResetJewels();
        }
        while ((movesLeft = HowManyMovesLeft()) == 0);

        // Hide them from the view, and start an animation that drops down the
        // jewels, piece by piece
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                GameObject pl = (GameObject)planeMapPosition[x, y];
                GameObject ob = (GameObject)obsMapPosition[x, y];
                if (pl != null && ob == null)
                {
                    GameObject j = jewelMapPosition[x, y];
                    Jewel jj = j.gameObject.GetComponent<Jewel>();
                    j.transform.localPosition = new Vector3(j.transform.localPosition.x, j.transform.localPosition.y - boardSize, j.transform.localPosition.z);
                    jj.Move(3, boardSize);
                }
            }
        }
    }

    /// <summary>
    /// 随机显示一个提醒宝石
    /// </summary>
    private void ShowRandomHint()
    {
        if (hintJewels.Count == 0) return;		// No moves left

        if (tip)
        {
            hintTimer = 0;
            return;
        }

        if (!tip)
        {

            GameObject tipGO = (GameObject)hintJewels[UnityEngine.Random.Range(0, hintJewels.Count)];

            do
            {
                tipGO = (GameObject)hintJewels[UnityEngine.Random.Range(0, hintJewels.Count)];
            }
            while (tipGO.tag == "Rock");
            tip = (Jewel)tipGO.gameObject.GetComponent<Jewel>();
            tip.alpha = true;
        }

        hintTimer = 0;
    }
    /// <summary>
    /// 交换宝石的位置
    /// </summary>
    /// <param name="swap"></param>
    private void SwapJewels(theSwap swap)
    {
        //Debug.Log("di er ci"+swapping.jewelBx + "" + swapping.jewelBy);
        int dirA, dirB;//定义两个宝石的交换方向：1左；2右；3上；4下
        bool verifyRock = false;//判断是否是个石头
        bool[,] markedForRemoval = new bool[boardSize, boardSize];

        if (swap.jewelAx > swap.jewelBx)
        {
            dirA = 2;
            dirB = 1;
        }
        else if (swap.jewelBx > swap.jewelAx)
        {
            dirA = 1;
            dirB = 2;
        }
        else if (swap.jewelAy > swap.jewelBy)
        {
            dirA = 4;
            dirB = 3;
        }
        else
        {
            dirA = 3;
            dirB = 4;
        }

        Jewel objIn;
        //如果这个宝石的tag是石头、则verifyRock=true
        if (jewelMapPosition[swap.jewelAx, swap.jewelAy].tag == "Rock" || jewelMapPosition[swap.jewelBx, swap.jewelBy].tag == "Rock")
        {
            verifyRock = true;
        }
        else
        {
            verifyRock = false;
        }

        //如果不是石头就进行交换
        if (!verifyRock)
        { //控制这个交换
            objIn = jewelMapPosition[swap.jewelAx, swap.jewelAy].gameObject.GetComponent<Jewel>();
            objIn.Move(dirA);

            objIn = jewelMapPosition[swap.jewelBx, swap.jewelBy].gameObject.GetComponent<Jewel>();
            objIn.Move(dirB);

            GameObject j = jewelMapPosition[swap.jewelAx, swap.jewelAy];
            jewelMapPosition[swap.jewelAx, swap.jewelAy] = jewelMapPosition[swap.jewelBx, swap.jewelBy];
            jewelMapPosition[swap.jewelBx, swap.jewelBy] = j;
        }

        //获取宝石A的情况(0:横4\1:5\2:方块\3:纵4\4:TL\-1:普通)
        int SpeA = CheckSpe(swap.jewelAx, swap.jewelAy);
        //获取宝石A的情况(0:横4\1:5\2:方块\3:纵4\4:TL\-1:普通)
        int SpeB = CheckSpe(swap.jewelBx, swap.jewelBy);

        #region 宝石A是5连消  1
        if (SpeA == 1)
        {
            if (SpeB == 0)
            {
                string tagname = jewelMapPosition[swap.jewelBx, swap.jewelBy].tag;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (jewelMapPosition[x, y].tag == tagname)
                        {
                            GameObject jA = jewelMapPosition[x, y];
                            jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsH");
                            SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                            markedForRemoval[x, y] = true;

                        }
                    }
                }
                markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
            }
            if (SpeB == 1)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        markedForRemoval[x, y] = true;
                    }
                }
                markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
            }
            if (SpeB == 2)
            {
                string tagname = jewelMapPosition[swap.jewelBx, swap.jewelBy].tag;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (jewelMapPosition[x, y].tag == tagname)
                        {
                            GameObject jA = jewelMapPosition[x, y];
                            jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsSqu");
                            SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                            markedForRemoval[x, y] = true;
                        }
                    }
                }
                markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
            }
            if (SpeB == 3)
            {
                string tagname = jewelMapPosition[swap.jewelBx, swap.jewelBy].tag;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (jewelMapPosition[x, y].tag == tagname)
                        {
                            GameObject jA = jewelMapPosition[x, y];
                            jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsV");
                            SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                            markedForRemoval[x, y] = true;
                        }
                    }
                }
                markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
            }
            if (SpeB == 4)
            {
                string tagname = jewelMapPosition[swap.jewelBx, swap.jewelBy].tag;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (jewelMapPosition[x, y].tag == tagname)
                        {
                            GameObject jA = jewelMapPosition[x, y];
                            jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsTL");
                            SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                            markedForRemoval[x, y] = true;
                        }
                    }
                }
                markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
            }
            if (SpeB == -1)
            {
                string tagname = jewelMapPosition[swap.jewelBx, swap.jewelBy].tag;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (jewelMapPosition[x, y].tag == tagname)
                        {
                            markedForRemoval[x, y] = true;
                            SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                        }
                    }
                }
                markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
            }
            //消除一次
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    //如果该点可以消除
                    if (markedForRemoval[x, y])
                        RemoveJewelAt(x, y);
                }
            }
            needToDropJewels = true;
            delayDrop = 0.3f;
            canSwap = false;
        }
        #endregion
        #region 宝石A横4消除  0
        if (SpeA == 0)
        {
            if (SpeB == 0 || SpeB == 3)
            {
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(swap.jewelBx, i) && markedForRemoval[swap.jewelBx, i] == false)
                        markedForRemoval[swap.jewelBx, i] = true;
                }
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(i, swap.jewelBy) && markedForRemoval[i, swap.jewelBy] == false)
                        markedForRemoval[i, swap.jewelBy] = true;
                }
            }
            if (SpeB == 1)
            {
                string tagname = jewelMapPosition[swap.jewelAx, swap.jewelAy].tag;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (jewelMapPosition[x, y].tag == tagname)
                        {
                            GameObject jA = jewelMapPosition[x, y];
                            int vh = UnityEngine.Random.Range(1, 2);
                            if (vh == 1)
                                jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsH");
                            else
                                jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsV");
                            SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                        }
                    }
                }
            }
            if (SpeB == 2)
            {
                int px = UnityEngine.Random.Range(0, boardSize - 1);
                int py = UnityEngine.Random.Range(0, boardSize - 1);
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(px, i) && markedForRemoval[px, i] == false)
                        markedForRemoval[px, i] = true;
                }
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(i, py) && markedForRemoval[i, px] == false)
                        markedForRemoval[i, px] = true;
                }
            }
            //if (SpeB == 4)
            //{
            //    string tagname = jewelMapPosition[swap.jewelBx, swap.jewelBy].tag;
            //    for (int y = 0; y < boardSize; y++)
            //    {
            //        for (int x = 0; x < boardSize; x++)
            //        {
            //            if (jewelMapPosition[x, y].tag == tagname)
            //            {
            //                GameObject jA = jewelMapPosition[x, y];
            //                jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsTL");
            //                SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
            //            }
            //        }
            //    }
            //}

            //消除一次
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    //如果该点可以消除
                    if (markedForRemoval[x, y])
                        RemoveJewelAt(x, y);
                }
            }
            if (SpeB != -1)
            {
                needToDropJewels = true;
                delayDrop = 0.3f;
                canSwap = false;
            }
        }
        #endregion
        #region 宝石A方块消除 2
        if (SpeA == 2)
        {
            if (SpeB == 0 || SpeB == 3)
            {
                int px = UnityEngine.Random.Range(0, boardSize - 1);
                int py = UnityEngine.Random.Range(0, boardSize - 1);
                markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
                markedForRemoval[swap.jewelBx, swap.jewelBy] = true;
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(px, i))
                    {
                        if (markedForRemoval[px, i] == false)
                            markedForRemoval[px, i] = true;
                    }
                }
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(i, py))
                    {
                        if (markedForRemoval[i, px] == false)
                            markedForRemoval[i, px] = true;
                    }
                }
            }
            if (SpeB == 1)
            {
                string tagname = jewelMapPosition[swap.jewelAx, swap.jewelAy].tag;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (jewelMapPosition[x, y].tag == tagname)
                        {
                            GameObject jA = jewelMapPosition[x, y];
                            jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsSqu");
                            SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                        }
                    }
                }
            }
            if (SpeB == 2)
            {
                int px = UnityEngine.Random.Range(1, boardSize - 2);
                int py = UnityEngine.Random.Range(1, boardSize - 2);
                int px1 = UnityEngine.Random.Range(1, boardSize - 2);
                int py1 = UnityEngine.Random.Range(1, boardSize - 2);
                int px2 = UnityEngine.Random.Range(1, boardSize - 2);
                int py2 = UnityEngine.Random.Range(1, boardSize - 2);
                //markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
                //markedForRemoval[swap.jewelBx, swap.jewelBy] = true;
                for (int i = 0; i <= 4; i++)
                {
                    //Debug.Log(i);
                    if (i == 0)
                    {
                        if (IsJewelAt(px, py))
                        {
                            if (markedForRemoval[px, py] == false)
                                markedForRemoval[px, py] = true;
                        }
                        if (IsJewelAt(px1, py1))
                        {
                            if (markedForRemoval[px1, py1] == false)
                                markedForRemoval[px1, py1] = true;
                        }
                        if (IsJewelAt(px2, py2))
                        {
                            if (markedForRemoval[px2, py2] == false)
                                markedForRemoval[px2, py2] = true;
                        }
                    }
                    if (i == 1)
                    {
                        //markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
                        //markedForRemoval[swap.jewelBx, swap.jewelBy] = true;
                        if (IsJewelAt(px + 1, py))
                        {
                            if (markedForRemoval[px + 1, py] == false)
                                markedForRemoval[px + 1, py] = true;
                        }
                        if (IsJewelAt(px1 + 1, py1))
                        {
                            if (markedForRemoval[px1 + 1, py1] == false)
                                markedForRemoval[px1 + 1, py1] = true;
                        }
                        if (IsJewelAt(px2 + 1, py2))
                        {
                            if (markedForRemoval[px2 + 1, py2] == false)
                                markedForRemoval[px2 + 1, py2] = true;
                        }
                    }
                    if (i == 2)
                    {
                        //markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
                        //markedForRemoval[swap.jewelBx, swap.jewelBy] = true;
                        if (IsJewelAt(px - 1, py) && markedForRemoval[px - 1, py] == false)
                            markedForRemoval[px - 1, py] = true;
                        if (IsJewelAt(px1 - 1, py1) && markedForRemoval[px1 - 1, py1] == false)
                            markedForRemoval[px1 - 1, py1] = true;
                        if (IsJewelAt(px2 - 1, py2) && markedForRemoval[px2 - 1, py2] == false)
                            markedForRemoval[px2 - 1, py2] = true;
                    }
                    if (i == 3)
                    {
                        //markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
                        //markedForRemoval[swap.jewelBx, swap.jewelBy] = true;
                        if (IsJewelAt(px, py + 1))
                        {
                            if (markedForRemoval[px, py + 1] == false)
                                markedForRemoval[px, py + 1] = true;
                        }
                        if (IsJewelAt(px1, py1 + 1))
                        {
                            if (markedForRemoval[px1, py1 + 1] == false)
                                markedForRemoval[px1, py1 + 1] = true;
                        }
                        if (IsJewelAt(px2, py2 + 1))
                        {
                            if (markedForRemoval[px2, py2 + 1] == false)
                                markedForRemoval[px2, py2 + 1] = true;
                        }
                    }
                    if (i == 4)
                    {
                        //markedForRemoval[swap.jewelAx, swap.jewelAy] = true;
                        //markedForRemoval[swap.jewelBx, swap.jewelBy] = true;
                        if (IsJewelAt(px, py - 1))
                        {
                            if (markedForRemoval[px, py - 1] == false)
                                markedForRemoval[px, py - 1] = true;
                        }
                        if (IsJewelAt(px1, py1 - 1))
                        {
                            if (markedForRemoval[px1, py1 - 1] == false)
                                markedForRemoval[px1, py1 - 1] = true;
                        }
                        if (IsJewelAt(px2, py2 - 1))
                        {
                            if (markedForRemoval[px2, py2 - 1] == false)
                                markedForRemoval[px2, py2 - 1] = true;
                        }
                    }
                }
            }


            //消除一次
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    //如果该点可以消除
                    if (markedForRemoval[x, y])
                        RemoveJewelAt(x, y);
                }
            }


            if (SpeB != -1)
            {
                needToDropJewels = true;
                delayDrop = 0.3f;
                canSwap = false;
            }
        }
        #endregion
        #region 宝石A纵4消除  3
        if (SpeA == 3)
        {
            if (SpeB == 0 || SpeB == 3)
            {
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(swap.jewelBx, i) && markedForRemoval[swap.jewelBx, i] == false)
                        markedForRemoval[swap.jewelBx, i] = true;
                }
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(i, swap.jewelBy) && markedForRemoval[i, swap.jewelBy] == false)
                        markedForRemoval[i, swap.jewelBy] = true;
                }
            }
            if (SpeB == 1)
            {
                string tagname = jewelMapPosition[swap.jewelAx, swap.jewelAy].tag;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (jewelMapPosition[x, y].tag == tagname)
                        {
                            GameObject jA = jewelMapPosition[x, y];
                            int vh = UnityEngine.Random.Range(1, 2);
                            if (vh == 1)
                                jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsH");
                            else
                                jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsV");
                            SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                        }
                    }
                }
            }
            if (SpeB == 2)
            {
                int px = UnityEngine.Random.Range(0, boardSize - 1);
                int py = UnityEngine.Random.Range(0, boardSize - 1);
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(px, i) && markedForRemoval[px, i] == false)
                        markedForRemoval[px, i] = true;
                }
                for (int i = 0; i < boardSize; i++)
                {
                    if (IsJewelAt(i, py) && markedForRemoval[i, px] == false)
                        markedForRemoval[i, px] = true;
                }
            }
            //if (SpeB == 4)
            //{
            //    string tagname = jewelMapPosition[swap.jewelBx, swap.jewelBy].tag;
            //    for (int y = 0; y < boardSize; y++)
            //    {
            //        for (int x = 0; x < boardSize; x++)
            //        {
            //            if (jewelMapPosition[x, y].tag == tagname)
            //            {
            //                GameObject jA = jewelMapPosition[x, y];
            //                jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsTL");
            //                SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
            //            }
            //        }
            //    }
            //}

            //消除一次
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    //如果该点可以消除
                    if (markedForRemoval[x, y])
                        RemoveJewelAt(x, y);
                }
            }
            if (SpeB != -1)
            {
                needToDropJewels = true;
                delayDrop = 0.3f;
                canSwap = false;
            }
        }
        #endregion
        #region 宝石ATL消除   4
        //if (SpeA == 4)
        //{
        //    if (SpeB == 0 || SpeB == 3)
        //    {
        //        for (int i = 0; i < boardSize; i++)
        //        {
        //            if (IsJewelAt(swap.jewelBx, i) && markedForRemoval[swap.jewelBx, i] == false)
        //                markedForRemoval[swap.jewelBx, i] = true;
        //        }
        //        for (int i = 0; i < boardSize; i++)
        //        {
        //            if (IsJewelAt(i, swap.jewelBy) && markedForRemoval[i, swap.jewelBy] == false)
        //                markedForRemoval[i, swap.jewelBy] = true;
        //        }
        //    }
        //    if (SpeB == 1)
        //    {
        //        string tagname = jewelMapPosition[swap.jewelAx, swap.jewelAy].tag;
        //        for (int y = 0; y < boardSize; y++)
        //        {
        //            for (int x = 0; x < boardSize; x++)
        //            {
        //                if (jewelMapPosition[x, y].tag == tagname)
        //                {
        //                    GameObject jA = jewelMapPosition[x, y];
        //                    int vh = UnityEngine.Random.Range(1, 2);
        //                    if (vh == 1)
        //                        jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsH");
        //                    else
        //                        jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsV");
        //                    SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
        //                }
        //            }
        //        }
        //    }
        //    if (SpeB == 2)
        //    {
        //        int px = UnityEngine.Random.Range(0, boardSize - 1);
        //        int py = UnityEngine.Random.Range(0, boardSize - 1);
        //        for (int i = 0; i < boardSize; i++)
        //        {
        //            if (IsJewelAt(px, i) && markedForRemoval[px, i] == false)
        //                markedForRemoval[px, i] = true;
        //        }
        //        for (int i = 0; i < boardSize; i++)
        //        {
        //            if (IsJewelAt(i, py) && markedForRemoval[i, px] == false)
        //                markedForRemoval[i, px] = true;
        //        }
        //    }
        //    //if (SpeB == 4)
        //    //{
        //    //    string tagname = jewelMapPosition[swap.jewelBx, swap.jewelBy].tag;
        //    //    for (int y = 0; y < boardSize; y++)
        //    //    {
        //    //        for (int x = 0; x < boardSize; x++)
        //    //        {
        //    //            if (jewelMapPosition[x, y].tag == tagname)
        //    //            {
        //    //                GameObject jA = jewelMapPosition[x, y];
        //    //                jA.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsTL");
        //    //                SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //消除一次
        //    for (int y = 0; y < boardSize; y++)
        //    {
        //        for (int x = 0; x < boardSize; x++)
        //        {
        //            //如果该点可以消除
        //            if (markedForRemoval[x, y])
        //                RemoveJewelAt(x, y);
        //        }
        //    }
        //    needToDropJewels = true;
        //    delayDrop = 0.3f;
        //    canSwap = false;
        //}
        #endregion
    }

    /// <summary>
    /// 判断是否为合法的交换
    /// </summary>
    /// <returns></returns>
    public bool IsLegalSwap()
    {
        if (swapping.jewelAx < 0 || swapping.jewelBx < 0) return false;
        if (jewelMapPosition[swapping.jewelAx, swapping.jewelAy] == null || jewelMapPosition[swapping.jewelBx, swapping.jewelBy] == null) return false;

        int dx = swapping.jewelAx - swapping.jewelBx;
        int dy = swapping.jewelAy - swapping.jewelBy;
        if (Mathf.Abs(dx) > 1 || Mathf.Abs(dy) > 1) return false;
        if ((Mathf.Abs(dx) == 1 && dy == 0) || (Mathf.Abs(dy) == 1 && dx == 0)) return true;
        return false;
    }

    /// <summary>
    /// 计算这个缺口有多深
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <param name="checkUp">ture则为中间的缺口、false是边界的缺口</param>
    /// <returns></returns>
    private int HowBigHoleAt(int x, int y, bool checkUp)
    {
        int length = 0;
        if (checkUp)
        {
            for (int hy = y; hy >= 0; hy--)
            {
                GameObject ob = (GameObject)obsMapPosition[x, hy];//障碍
                GameObject pl = (GameObject)planeMapPosition[x, hy];//面板
                if (!IsJewelAt(x, hy))
                {
                    if (ob == null)
                    {
                        if (pl != null)
                            length++;
                        else
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                    break;
            }
        }
        else
        {
            for (int hy = y; hy < boardSize; hy++)
            {
                GameObject ob = (GameObject)obsMapPosition[x, hy];//障碍
                GameObject pl = (GameObject)planeMapPosition[x, hy];//面板
                if (!IsJewelAt(x, hy))
                {
                    if (ob == null)
                    {
                        //if (pl != null)
                        //    length++;
                        //else
                        //    break;
                        length++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                    break;
            }
        }
        return length;
    }

    /// <summary>
    /// 掉落宝石,(x,y)是要掉落宝石的坐标
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <param name="slots">缺口</param>
    private int ShiftJewelsDown(int x, int y, int slots)
    {
        //for (int yy = y; yy >= 0; yy--)
        //{
        //    GameObject pl = (GameObject)planeMapPosition[x, yy];
        //    if (pl != null)
        //    {
        //        if (PlaneExist(x, yy + slots))
        //        {
        //            jewelMapPosition[x, yy + slots] = jewelMapPosition[x, yy];
        //            if (jewelMapPosition[x, yy + slots] != null)
        //            {
        //                Jewel objIn = jewelMapPosition[x, yy].GetComponent<Jewel>();
        //                objIn.Move(3, slots);
        //            }
        //            jewelMapPosition[x, yy] = null;
        //        }

        //    }
        //}
        /*先省略掉试试*/
        //GameObject pl = (GameObject)planeMapPosition[x, y];
        //if (pl != null)
        //{
        if (PlaneExist(x, y) && !ObsExist(x, y) && IsJewelExist(x, y))
        {
            jewelMapPosition[x, y + slots] = jewelMapPosition[x, y];
            if (jewelMapPosition[x, y + slots] != null)
            {
                Jewel objIn = jewelMapPosition[x, y].GetComponent<Jewel>();
                objIn.Move(3, slots);
            }
            jewelMapPosition[x, y] = null;
        }
        //else
        //{
        //    if (x < 7)//从右边的地方拉一个下来
        //    {
        //        if (PlaneExist(x + 1, y + slots - 1) && !ObsExist(x + 1, y + slots - 1) && IsJewelExist(x + 1, y + slots - 1))
        //        {
        //            jewelMapPosition[x, y + slots] = jewelMapPosition[x + 1, y + slots - 1];
        //            if (jewelMapPosition[x, y + slots] != null)
        //            {
        //                Jewel objIn = jewelMapPosition[x, y + slots].GetComponent<Jewel>();
        //                objIn.Move(5, slots);
        //            }
        //            jewelMapPosition[x + 1, y + slots - 1] = null;
        //        }
        //    }
        //    else
        //    {
        //        if (PlaneExist(x - 1, y + slots - 1) && !ObsExist(x - 1, y + slots - 1) && IsJewelExist(x - 1, y + slots - 1))
        //        {
        //            jewelMapPosition[x, y + slots] = jewelMapPosition[x - 1, y + slots - 1];
        //            if (jewelMapPosition[x, y + slots] != null)
        //            {
        //                Jewel objIn = jewelMapPosition[x, y + slots].GetComponent<Jewel>();
        //                objIn.Move(6, slots);
        //            }
        //            jewelMapPosition[x - 1, y + slots - 1] = null;
        //        }
        //    }
        //}
        return slots;
        //}
    }
    /// <summary>
    /// 判断当前坐标下是否存在面板
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>存在返回true</returns>
    private bool PlaneExist(int x, int y)
    {
        if (x < 0 || y < 0 || x > boardSize - 1 || y > boardSize - 1) return false;
        else
        {
            GameObject pl = (GameObject)planeMapPosition[x, y];
            if (pl == null)
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// 判断当前坐标下是否存在面板
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>存在返回true</returns>
    private bool ObsExist(int x, int y)
    {
        if (x < 0 || y < 0 || x > boardSize - 1 || y > boardSize - 1) return false;
        else
        {
            GameObject ob = (GameObject)obsMapPosition[x, y];
            if (ob == null)
                return false;
            else
                return true;
        }
    }
    /// <summary>
    /// 检验降落的宝石
    /// </summary>
    public void CheckForFallingJewels()
    {
        for (int x = 0; x < boardSize; x++)
        {			// 从左到右从下到上的检验每一列的宝石
            int totalSlots = 0;
            for (int y = boardSize - 1; y > 0; y--)
            {	// search for holes
                GameObject pl = (GameObject)planeMapPosition[x, y];
                GameObject ob = (GameObject)obsMapPosition[x, y];
                int slotsd;
                if (!IsJewelAt(x, y) && pl != null && ob == null)
                { 				//如果存在缺口计算缺口有多深
                    int slots = HowBigHoleAt(x, y, true);
                    totalSlots += slots;
                    y -= slots;
                    slotsd = ShiftJewelsDown(x, y, slots); 		// 在已知的缺口中掉落每一个宝石
                    y += slotsd;
                    //else
                    //{
                    //    if (y >0)
                    //    {
                    //        while (PlaneExist(x, y) == false )
                    //        {
                    //            add++;
                    //            y = y - 1;
                    //            if (PlaneExist(x, y)&&!ObsExist(x,y))
                    //            {
                    //                while (IsJewelAt(x, y) == false)
                    //                {
                    //                    if (y > 0)
                    //                    {
                    //                        abb++;
                    //                        y = y - 1;
                    //                    }
                    //                    else
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //                ShiftJewelsDown(x, y, slots + add + abb);
                    //            }
                    //        }
                    //    }
                    //}
                    //if (add != 0)
                    //{
                    //    y = y + add + slots + abb;
                    //}
                    //else
                    //{
                    //    y += slots;
                    //}

                    needToCheckCascades = true;
                }
            }
        }

        for (int x = 0; x < boardSize; x++)
        {			// Check the top row for empty slots

            if (!IsJewelAt(x, 0))
            {
                int slots = HowBigHoleAt(x, 0, false);	// 如果存在缺口计算缺口有多深

                for (int y = 0; y < slots; y++)
                {		// Create a new jewel, position it out of the screen to fall down
                    GameObject pl = (GameObject)planeMapPosition[x, y];
                    if (pl != null)
                    {
                        GameObject jewelPrefab = (GameObject)Instantiate(jewels[UnityEngine.Random.Range(0, jewels.Length)], new Vector3(x, y - slots, -1), jewels[0].transform.rotation);
                        jewelPrefab.transform.parent = TheBoard;
                        jewelPrefab.transform.localPosition = new Vector3(x, y - slots, 0);

                        Jewel objIn = jewelPrefab.gameObject.GetComponent<Jewel>();
                        objIn.Move(3, slots);

                        jewelMapPosition[x, y] = jewelPrefab;
                        needToCheckCascades = true;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 判断是否存在消除
    /// </summary>
    public void CheckForCascades()
    {
        int total = RemoveTripletsAft();//判断是否完成消除
        if (total == 0)
        { 						// 没有可消除的情况
            totalTriplets = 0;
            canSwap = true;

            movesLeft = HowManyMovesLeft();// 计算当前可以移动的步数

            if (movesLeft == 0)
            {				// 使用这个如果你想结束的时候就没有左移
                PopulateWithNewJewels();	// 在这种情况下再生新的珠宝
            }
        }
    }

    /// <summary>
    /// 确认该位置是否可以放置宝石
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    /// <returns>不能放置返回false</returns>
    public bool IsJewelAt(int x, int y)
    {
        //x、y坐标不能小于0，不能超越边界，边界从0开始计数
        if (x < 0 || y < 0 || x > boardSize - 1 || y > boardSize - 1) return false;
        return jewelMapPosition[x, y] != null;
    }

    /// <summary>
    /// 确认该位置是否存在宝石
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    /// <returns>不能放置返回false</returns>
    public bool IsJewelExist(int x, int y)
    {
        //if (!IsJewelAt(x, y)) return false;
        bool ExistJelly;
        if (x >= 0 && x < boardSize && y >= 0 && y < boardSize)
        {
            if (jewelMapPosition[x, y] != null)
            {
                ExistJelly = true;
            }
            else
            {
                ExistJelly = false;
            }
        }
        else
        {
            ExistJelly = false;
        }
        return ExistJelly;
    }

    /// <summary>
    /// 消除当前坐标下的宝石
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    public void RemoveJewelAt(int x, int y)
    {
        if (!IsJewelAt(x, y)) return;
        if (x >= 0 && x < boardSize && y >= 0 && y < boardSize && PlaneExist(x, y))
        {
            GameObject j = (GameObject)jewelMapPosition[x, y];
            jewelMapPosition[x, y] = null;
            Jewel jj = j.gameObject.GetComponent<Jewel>();//获取到Jewel脚本
            jj.Die();
            jewelRemains.Add(j);
            totalRemovedJewels++;
            RemoveObs(x, y);
        }
    }

    /// <summary>
    /// 消除当前坐标下的宝石
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    public void RemoveObs(int x, int y)
    {
        if (ObsExist(x + 1, y))
        {
            GameObject o = (GameObject)obsMapPosition[x+1, y];
            obsMapPosition[x+1, y] = null;
            obstracle ob = o.gameObject.GetComponent<obstracle>();//获取到obstracle脚本
            ob.Die();
        }
        if (ObsExist(x - 1, y))
        {
            GameObject o = (GameObject)obsMapPosition[x - 1, y];
            obsMapPosition[x - 1, y] = null;
            obstracle ob = o.gameObject.GetComponent<obstracle>();//获取到obstracle脚本
            ob.Die();
        }
        if (ObsExist(x, y + 1))
        {
            GameObject o = (GameObject)obsMapPosition[x, y + 1];
            obsMapPosition[x, y + 1] = null;
            obstracle ob = o.gameObject.GetComponent<obstracle>();//获取到obstracle脚本
            ob.Die();
        }
        if (ObsExist(x, y - 1))
        {
            GameObject o = (GameObject)obsMapPosition[x, y - 1];
            obstracle ob = o.gameObject.GetComponent<obstracle>();//获取到obstracle脚本
            ob.Die();
            obsMapPosition[x, y - 1] = null;
        }
    }

    /// <summary>
    /// 更新宝石状态
    /// 
    /// </summary>
    public void UpdateJewelRemains()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject j in jewelRemains)
        {
            Jewel jj = j.gameObject.GetComponent<Jewel>();
            if (jj.Dead)
                toRemove.Add(j);
        }

        foreach (GameObject j in toRemove)
            jewelRemains.Remove(j);
        toRemove.Clear();
    }

    /// <summary>
    /// 判断是否存在连3
    /// </summary>
    /// <returns value='triple'>若不为0则存在连3情况</returns>
    public int CountMatch3()
    {
        int triple = 0;

        #region 横消
        for (int y = 0; y < boardSize; y++)
        { 		// Horizontal
            int counter = 1;
            for (int x = 1; x < boardSize; x++)
            {
                //判断该位置可放置，水平前一个位置可放置并且二者tag相同
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag)
                {
                    counter++;
                    if (counter >= 3 && jewelMapPosition[x, y].tag != "Rock")
                    {
                        triple++;					// 存在连3情况，计数+1
                    }
                }
                else
                {
                    counter = 1;
                }
            }
        }
        #endregion

        #region 判断竖消
        for (int x = 0; x < boardSize; x++)
        {		// Vertical
            int counter = 1;
            for (int y = 1; y < boardSize; y++)
            {
                if (IsJewelAt(x, y) && IsJewelAt(x, y - 1) && jewelMapPosition[x, y].tag == jewelMapPosition[x, y - 1].tag)
                {
                    counter++;
                    if (counter >= 3 && jewelMapPosition[x, y].tag != "Rock")
                    {
                        triple++;					// Three or more were found
                    }
                }
                else
                {
                    counter = 1;
                }
            }
        }
        #endregion

        #region 判断方块消
        for (int y = 0; y < boardSize; y++)
        { 		// 横向检查每一个X
            int counter = 1;
            for (int x = 0; x < boardSize; x++)
            {
                //判断该位置可放置，水平前一个位置可放置并且二者tag相同
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && IsJewelAt(x, y - 1) && IsJewelAt(x - 1, y - 1) && (jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag)
                    && (jewelMapPosition[x - 1, y].tag == jewelMapPosition[x, y - 1].tag) && (jewelMapPosition[x, y - 1].tag == jewelMapPosition[x - 1, y - 1].tag))
                {
                    counter = 4;
                    if (counter == 4 && jewelMapPosition[x, y].tag != "Rock")
                    {
                        triple = 6;					//当前存在方块消情况、triple计数为6
                    }
                }
                else
                {
                    counter = 1;
                }
            }
        }
        #endregion

        return triple;
    }

    /// <summary>
    /// 连3消除后计算积分
    /// </summary>
    /// <param name="jewels">消除宝石个数</param>
    /// <param name="start">开始位置</param>
    /// <param name="end">结束位置</param>
    /// <param name="lineRow">行数</param>
    /// <param name="horizontal">是否水平</param>
    private void ScoreMatch3(int jewels, int start, int end, int lineRow, bool horizontal, bool rectangle)
    {
        int tripletScore = (jewels - 2) * baseScore;
        string itemMatched = "";
        bool foundSkull = false;
        totalTriplets++;
        //如果连3超过1个
        if (totalTriplets > 1)
            tripletScore += 30 * (totalTriplets - 1);//多一个加30分

        if (totalTriplets > longestChain)			// 计算长链
            longestChain = totalTriplets;
        //播放声音文件
        GetComponent<AudioSource>().pitch = 1;
        GetComponent<AudioSource>().pitch = 1 + (totalTriplets * 0.2f);

        if (horizontal)
        {
            // Horizontal match 3 or more
            PointCount(jewels, tripletScore, start, lineRow, end, jewelMapPosition[start, lineRow].tag, new Vector2(start + 1, lineRow));
            if (jewelMapPosition[start, lineRow].tag == "bomb") foundSkull = true;//如果是骷髅
            itemMatched = jewelMapPosition[start, lineRow].tag;
            if (foundSkull)
                PointsAni(-skullInitial, new Vector2(start + 1, lineRow));
            else
                PointsAni(tripletScore, new Vector2(start + 1, lineRow));
        }
        else
        {
            if (!rectangle)
            {
                // Vertical match 3 or more
                PointCount(jewels, tripletScore, lineRow, start, end, jewelMapPosition[lineRow, start].tag, new Vector2(lineRow, start + 1));
                if (jewelMapPosition[lineRow, start].tag == "bomb") foundSkull = true;
                itemMatched = jewelMapPosition[lineRow, start].tag;
                if (foundSkull)
                    PointsAni(-skullInitial, new Vector2(lineRow, start + 1));
                else
                    PointsAni(tripletScore, new Vector2(lineRow, start + 1));
            }
        }

        if (rectangle)
        {
            PointCount(jewels, tripletScore, lineRow, start, end, jewelMapPosition[lineRow, start].tag, new Vector2(lineRow, start + 1));
            if (jewelMapPosition[lineRow, start].tag == "bomb") foundSkull = true;
            itemMatched = jewelMapPosition[lineRow, start].tag;
            if (foundSkull)
                PointsAni(-skullInitial, new Vector2(lineRow, start + 1));
            else
                PointsAni(tripletScore, new Vector2(lineRow, start + 1));
        }

        //if totalTriplets is greater than 4 lets explode some skulls and put some effect on the screen
        if (totalTriplets == 4)
        {
            DestroyJewelType("bomb", 0);
            CameraShake.shakeFor(1f, 0.1f);
            flashIt = true;
        }
        //Special effects on the screen 
        //Show messages when the user matches triplets
        Transform camPos = Camera.main.transform;
        if (totalTriplets > 1 && !foundSkull && !GameObject.FindGameObjectWithTag("MMsg"))
        {
            //Show the message img according to the pts
            GameObject msg = (GameObject)Instantiate(Resources.Load("Prefabs/puzzleUI/MatchMessage"));
            msg.transform.position = new Vector3(camPos.position.x, camPos.position.y, camPos.position.z + 6f);
            msg.transform.rotation.SetLookRotation(camPos.position);

            //Loads the Textures
            if (totalTriplets == 2)
                msg.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/2Dtextures/P1");
            if (totalTriplets == 3)
                msg.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/2Dtextures/P2");
            if (totalTriplets == 4)
                msg.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/2Dtextures/P3");
            if (totalTriplets >= 5)
                msg.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/2Dtextures/P4");
        }

        //Special effects on the screen 
        //Show messages when the user matches 3 or more skulls
        if (foundSkull && !GameObject.FindGameObjectWithTag("MMsg"))
        {
            GameObject skullmsg = (GameObject)Instantiate(Resources.Load("Prefabs/puzzleUI/MatchMessage"));
            skullmsg.transform.position = new Vector3(camPos.position.x, camPos.position.y, camPos.position.z + 6f);
            skullmsg.transform.rotation.SetLookRotation(camPos.position);
            skullmsg.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/2Dtextures/P5");
        }

        if (foundSkull)
        {
            //some code here if found skull
        }

        if (tripletScore < 0) tripletScore = 0;

        if (!foundSkull)
        { //Count Score
            if (ScoreBoostTimer > Time.timeSinceLevelLoad)
            {
                // add more score here if you want to use the boost timer
                // TotalScore += tripletScore*ScoreBoostValue;
            }
            else
            {
                //TotalScore += tripletScore;
                ScoreBoostValue = 0;
            }
        }
    }

    //Use this function if you want to put some special effects on the Screen
    /// <summary>
    /// 在屏幕上添加特效
    /// </summary>
    /// <param name="jewels">消除的宝石数量</param>
    /// <param name="score">当前获得的分数</param>
    /// <param name="start">开始位置</param>
    /// <param name="lineRow">行数</param>
    /// <param name="end">结束位置</param>
    /// <param name="item">宝石类型</param>
    /// <param name="middle">中点</param>
    private void PointCount(int jewels, int score, int start, int lineRow, int end, string item, Vector2 middle)
    {

        int itemCh = 0;

        switch (item)
        {
            case "Item1":
                itemCh = 0;
                break;
            case "Item2":
                itemCh = 1;
                break;
            case "Item3":
                itemCh = 2;
                break;
            case "Item4":
                itemCh = 3;
                break;
            case "Item5":
                itemCh = 4;
                break;
            case "Item7":   //hp
                itemCh = 5;
                break;
            default:
                itemCh = -1;
                //just break if no options          
                break;
        }
        //如果是骷髅，播放爆炸动画
        if (item == "bomb")
        {  //if Skull
            GetComponent<AudioSource>().PlayOneShot(explodeSound);
            CameraShake.shakeFor(0.5f, 0.1f);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(match3Sound);
        }

        //Example
        //if(itemCh == 0) Debug.Log("you matched"+ jewels);
    }

    /// <summary>
    /// ???
    /// </summary>
    private void refreshAll()
    {
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (IsJewelAt(x, y))
                {
                    if (jewelMapPosition[x, y].transform.localPosition != new Vector3(x, y, -1))
                    {
                        Jewel jj = jewelMapPosition[x, y].GetComponent<Jewel>();
                        jj.MoveOut(new Vector3(x, y, -1));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 消除连3情况(主动消除)
    /// </summary>
    /// <returns></returns>
    private int RemoveTriplets(int count)
    {
        int triplets = count;
        if (triplets == 0) return 0;			// 消除完成
        //创建一个二维数组来记录当前位置是否可以消除
        bool[,] markedForRemoval = new bool[boardSize, boardSize];

        #region 横向连3消除
        for (int y = 0; y < boardSize; y++)
        {	// Horizontal
            int counter = 1;
            int startsAt = 0;
            int endsAt = -1;

            for (int x = 1; x < boardSize; x++)
            {
                //如果该位置和横向前一个位置都存在、并且二者的tag相同、并且该宝石tag不是rock
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag && jewelMapPosition[x, y].tag != "Rock")
                {
                    counter++;
                    endsAt = x;//新X坐标替换原坐标
                }
                else
                {
                    if (counter >= 3)
                    {			// Mark for removal
                        if (counter == 4)
                        {
                            FourMadeH = true;
                            FourMadeHCount++;
                            GameObject j = (GameObject)jewelMapPosition[x - 1, y];
                            Jewel jj = j.gameObject.GetComponent<Jewel>();
                            jellytype = jj.gettype(jewelMapPosition[x - 1, y].tag);
                            jellySpetype = jj.getSpeType("FourH");
                        }
                        if (counter == 5)
                        {
                            FiveMade = true;
                            FiveMadeCount++;
                            GameObject j = (GameObject)jewelMapPosition[x - 1, y];
                            Jewel jj = j.gameObject.GetComponent<Jewel>();
                            jellytype = jj.gettype(jewelMapPosition[x - 1, y].tag);
                            jellySpetype = jj.getSpeType("Five");
                        }
                        ScoreMatch3(counter, startsAt, endsAt, y, true, false);
                        for (int rx = startsAt; rx <= endsAt; rx++)
                        {
                            markedForRemoval[rx, y] = true;
                            SpeClear(rx, y, CheckSpe(rx, y), ref markedForRemoval);
                        }
                    }
                    counter = 1;
                    startsAt = x;
                    endsAt = 0;
                }
            }

            if (counter >= 3 && endsAt == boardSize - 1)
            {
                if (counter == 4)
                {
                    FourMadeH = true;
                    FourMadeHCount++;
                    GameObject j = (GameObject)jewelMapPosition[endsAt, y];
                    Jewel jj = j.gameObject.GetComponent<Jewel>();
                    jellytype = jj.gettype(jewelMapPosition[endsAt, y].tag);
                    jellySpetype = jj.getSpeType("FourH");
                }
                if (counter == 5)
                {
                    FiveMade = true;
                    FiveMadeCount++;
                    GameObject j = (GameObject)jewelMapPosition[endsAt, y];
                    Jewel jj = j.gameObject.GetComponent<Jewel>();
                    jellytype = jj.gettype(jewelMapPosition[endsAt, y].tag);
                    jellySpetype = jj.getSpeType("Five");
                }
                ScoreMatch3(counter, startsAt, endsAt, y, true, false);
                for (int rx = startsAt; rx <= endsAt; rx++)
                {
                    markedForRemoval[rx, y] = true;
                    SpeClear(rx, y, CheckSpe(rx, y), ref markedForRemoval);
                }
            }
        }
        #endregion

        #region 纵向连3消除
        for (int x = 0; x < boardSize; x++)
        {	// Vertical
            int counter = 1;
            int startsAt = 0;
            int endsAt = -1;

            for (int y = 1; y < boardSize; y++)
            {
                if (IsJewelAt(x, y) && IsJewelAt(x, y - 1) && jewelMapPosition[x, y].tag == jewelMapPosition[x, y - 1].tag && jewelMapPosition[x, y].tag != "Rock")
                {
                    counter++;
                    endsAt = y;
                }
                else
                {
                    if (counter >= 3)
                    { // Mark for removal
                        if (counter == 4)
                        {
                            FourMadeV = true;
                            FourMadeVCount++;
                            GameObject j = (GameObject)jewelMapPosition[x, y - 1];
                            Jewel jj = j.gameObject.GetComponent<Jewel>();
                            jellytype = jj.gettype(jewelMapPosition[x, y - 1].tag);
                            jellySpetype = jj.getSpeType("FourV");
                        }
                        if (counter == 5)
                        {
                            FiveMade = true;
                            FiveMadeCount++;
                            GameObject j = (GameObject)jewelMapPosition[x, y - 1];
                            Jewel jj = j.gameObject.GetComponent<Jewel>();
                            jellytype = jj.gettype(jewelMapPosition[x, y - 1].tag);
                            jellySpetype = jj.getSpeType("Five");
                        }
                        ScoreMatch3(counter, startsAt, endsAt, x, false, false);
                        for (int ry = startsAt; ry <= endsAt; ry++)
                        {
                            markedForRemoval[x, ry] = true;
                            SpeClear(x, ry, CheckSpe(x, ry), ref markedForRemoval);
                        }

                    }

                    counter = 1;
                    startsAt = y;
                    endsAt = 0;
                }

                if (counter >= 3 && endsAt == boardSize - 1)
                {
                    if (counter == 4)
                    {
                        FourMadeV = true;
                        FourMadeVCount++;
                        GameObject j = (GameObject)jewelMapPosition[x, endsAt];
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jellytype = jj.gettype(jewelMapPosition[x, endsAt].tag);
                        jellySpetype = jj.getSpeType("FourV");
                    }
                    if (counter == 5)
                    {
                        FiveMade = true;
                        FiveMadeCount++;
                        GameObject j = (GameObject)jewelMapPosition[x, endsAt];
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jellytype = jj.gettype(jewelMapPosition[x, endsAt].tag);
                        jellySpetype = jj.getSpeType("Five");
                    }
                    ScoreMatch3(counter, startsAt, endsAt, x, false, false);
                    for (int ry = startsAt; ry <= endsAt; ry++)
                    {
                        markedForRemoval[x, ry] = true;
                        SpeClear(x, ry, CheckSpe(x, ry), ref markedForRemoval);
                    }
                    startsAt = y;
                }
            }
        }
        #endregion

        #region 方块消
        for (int y = 0; y < boardSize; y++)
        {
            int counter = 1;
            int startsAt = 0;
            int endsAt = -1;
            for (int x = 0; x < boardSize; x++)
            {
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && IsJewelAt(x, y - 1) && IsJewelAt(x - 1, y - 1) && (jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag) && (jewelMapPosition[x - 1, y].tag == jewelMapPosition[x, y - 1].tag) && (jewelMapPosition[x, y - 1].tag == jewelMapPosition[x - 1, y - 1].tag))
                {
                    counter = 4;
                    if (counter == 4 && jewelMapPosition[x, y].tag != "Rock")
                    {
                        startsAt = y;
                        SquareMade = true;
                        SquareMadeCount++;
                        GameObject j = (GameObject)jewelMapPosition[x, y];
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jellytype = jj.gettype(jewelMapPosition[x, y].tag);
                        jellySpetype = jj.getSpeType("Square");

                        ScoreMatch3(counter, startsAt, endsAt, x, false, true);
                        markedForRemoval[x, y] = true;
                        markedForRemoval[x - 1, y] = true;
                        markedForRemoval[x, y - 1] = true;
                        markedForRemoval[x - 1, y - 1] = true;
                        SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                        SpeClear(x - 1, y, CheckSpe(x - 1, y), ref markedForRemoval);
                        SpeClear(x, y - 1, CheckSpe(x, y - 1), ref markedForRemoval);
                        SpeClear(x - 1, y - 1, CheckSpe(x - 1, y - 1), ref markedForRemoval);
                    }
                }
                else
                {
                    counter = 1;
                }
            }
        }
        #endregion

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                //如果该点可以消除
                if (markedForRemoval[x, y])
                    RemoveJewelAt(x, y);
            }
        }
        #region 在消除位置生成特殊宝石
        if (SquareMade == true)
        {
            if (jellytype != -1 && jellySpetype != -1)
            {
                MadeSpe(swapping.jewelBx, swapping.jewelBy, jellytype, jellySpetype);
                MadeSpe(swapping.jewelAx, swapping.jewelAy, jellytype, jellySpetype);
            }
            SquareMade = false;
            jellytype = -1;
            jellySpetype = -1;
            GameObject j = (GameObject)jewelMapPosition[swapping.jewelBx, swapping.jewelBy];
            jewelRemains.Remove(j);
        }

        if (FourMadeH == true)
        {
            if (jellytype != -1 && jellySpetype != -1)
            {
                MadeSpe(swapping.jewelBx, swapping.jewelBy, jellytype, jellySpetype);
                MadeSpe(swapping.jewelAx, swapping.jewelAy, jellytype, jellySpetype);
            }
            FourMadeH = false;
            jellytype = -1;
            jellySpetype = -1;
            GameObject j = (GameObject)jewelMapPosition[swapping.jewelBx, swapping.jewelBy];
            jewelRemains.Remove(j);
        }

        if (FourMadeV == true)
        {
            if (jellytype != -1 && jellySpetype != -1)
            {
                MadeSpe(swapping.jewelBx, swapping.jewelBy, jellytype, jellySpetype);
                MadeSpe(swapping.jewelAx, swapping.jewelAy, jellytype, jellySpetype);
            }
            FourMadeV = false;
            jellytype = -1;
            jellySpetype = -1;
            GameObject j = (GameObject)jewelMapPosition[swapping.jewelBx, swapping.jewelBy];
            jewelRemains.Remove(j);
        }

        if (FiveMade == true)
        {
            if (jellytype != -1 && jellySpetype != -1)
            {
                MadeSpe(swapping.jewelBx, swapping.jewelBy, jellytype, jellySpetype);
                MadeSpe(swapping.jewelAx, swapping.jewelAy, jellytype, jellySpetype);
            }
            FiveMade = false;
            jellytype = -1;
            jellySpetype = -1;
            GameObject j = (GameObject)jewelMapPosition[swapping.jewelBx, swapping.jewelBy];
            jewelRemains.Remove(j);
        }
        #endregion
        needToDropJewels = true;
        delayDrop = 0.3f;
        canSwap = false;

        return triplets;
    }

    /// <summary>
    /// 消除连3情况（被动消除）
    /// </summary>
    /// <returns></returns>
    private int RemoveTripletsAft()
    {
        int triplets = CountMatch3();
        if (triplets == 0) return 0;			// 消除完成
        //创建一个二维数组来存在当前位置是否可以消除
        bool[,] markedForRemoval = new bool[boardSize, boardSize];

        #region 横向连3消除
        for (int y = 0; y < boardSize; y++)
        {	// Horizontal
            int counter = 1;
            int startsAt = 0;
            int endsAt = -1;

            for (int x = 1; x < boardSize; x++)
            {
                //如果该位置和横向前一个位置都存在、并且二者的tag相同、并且该宝石tag不是rock
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag && jewelMapPosition[x, y].tag != "Rock")
                {
                    counter++;
                    endsAt = x;//新X坐标替换原坐标
                }
                else
                {
                    if (counter >= 3)
                    {			// Mark for removal
                        if (counter == 4)
                        {
                            FourMadeH = true;
                            FourMadeHCount++;
                            GameObject j = (GameObject)jewelMapPosition[x - 1, y];
                            Jewel jj = j.gameObject.GetComponent<Jewel>();
                            jellytype = jj.gettype(jewelMapPosition[x - 1, y].tag);
                            jellySpetype = jj.getSpeType("FourH");
                        }
                        if (counter == 5)
                        {
                            FiveMade = true;
                            FiveMadeCount++;
                            GameObject j = (GameObject)jewelMapPosition[x - 1, y];
                            Jewel jj = j.gameObject.GetComponent<Jewel>();
                            jellytype = jj.gettype(jewelMapPosition[x - 1, y].tag);
                            jellySpetype = jj.getSpeType("Five");
                        }
                        ScoreMatch3(counter, startsAt, endsAt, y, true, false);
                        for (int rx = startsAt; rx <= endsAt; rx++)
                        {
                            markedForRemoval[rx, y] = true;
                            SpeClear(rx, y, CheckSpe(rx, y), ref markedForRemoval);
                        }
                        if (FourMadeH == true)
                        {
                            FourHJewels.Add(jewelMapPosition[startsAt, y]);
                        }
                        if (FiveMade == true)
                        {
                            FiveJewels.Add(jewelMapPosition[startsAt, y]);
                        }
                    }

                    counter = 1;
                    startsAt = x;
                    endsAt = 0;
                }


                if (counter >= 3 && endsAt == boardSize - 1)
                {
                    if (counter == 4)
                    {
                        FourMadeH = true;
                        FourMadeHCount++;
                        GameObject j = (GameObject)jewelMapPosition[x - 1, y];
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jellytype = jj.gettype(jewelMapPosition[x - 1, y].tag);
                        jellySpetype = jj.getSpeType("FourH");
                    }
                    if (counter == 5)
                    {
                        FiveMade = true;
                        FiveMadeCount++;
                        GameObject j = (GameObject)jewelMapPosition[x - 1, y];
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jellytype = jj.gettype(jewelMapPosition[x - 1, y].tag);
                        jellySpetype = jj.getSpeType("Five");
                    }
                    ScoreMatch3(counter, startsAt, endsAt, y, true, false);
                    for (int rx = startsAt; rx <= endsAt; rx++)
                    {
                        markedForRemoval[rx, y] = true;
                        SpeClear(rx, y, CheckSpe(rx, y), ref markedForRemoval);
                    }
                }
            }
        }
        #endregion

        #region 纵向连3消除
        for (int x = 0; x < boardSize; x++)
        {	// Vertical
            int counter = 1;
            int startsAt = 0;
            int endsAt = -1;

            for (int y = 1; y < boardSize; y++)
            {
                if (IsJewelAt(x, y) && IsJewelAt(x, y - 1) && jewelMapPosition[x, y].tag == jewelMapPosition[x, y - 1].tag && jewelMapPosition[x, y].tag != "Rock")
                {
                    counter++;
                    endsAt = y;
                }
                else
                {
                    if (counter >= 3)
                    { // Mark for removal
                        if (counter == 4)
                        {
                            FourMadeV = true;
                            FourMadeVCount++;
                            GameObject j = (GameObject)jewelMapPosition[x, y - 1];
                            Jewel jj = j.gameObject.GetComponent<Jewel>();
                            jellytype = jj.gettype(jewelMapPosition[x, y - 1].tag);
                            jellySpetype = jj.getSpeType("FourV");
                        }
                        if (counter == 5)
                        {
                            FiveMade = true;
                            FiveMadeCount++;
                            GameObject j = (GameObject)jewelMapPosition[x, y - 1];
                            Jewel jj = j.gameObject.GetComponent<Jewel>();
                            jellytype = jj.gettype(jewelMapPosition[x, y - 1].tag);
                            jellySpetype = jj.getSpeType("Five");
                        }
                        ScoreMatch3(counter, startsAt, endsAt, x, false, false);
                        for (int ry = startsAt; ry <= endsAt; ry++)
                        {
                            markedForRemoval[x, ry] = true;
                            SpeClear(x, ry, CheckSpe(x, ry), ref markedForRemoval);
                        }
                        if (FourMadeV == true)
                        {
                            FourVJewels.Add(jewelMapPosition[x, startsAt]);
                        }
                        if (FiveMade == true)
                        {
                            FiveJewels.Add(jewelMapPosition[x, startsAt]);
                        }

                    }

                    counter = 1;
                    startsAt = y;
                    endsAt = 0;
                }

                if (counter >= 3 && endsAt == boardSize - 1)
                {
                    if (counter == 4)
                    {
                        FourMadeV = true;
                        FourMadeVCount++;
                        GameObject j = (GameObject)jewelMapPosition[x, y - 1];
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jellytype = jj.gettype(jewelMapPosition[x, y - 1].tag);
                        jellySpetype = jj.getSpeType("FourV");
                    }
                    if (counter == 5)
                    {
                        FiveMade = true;
                        FiveMadeCount++;
                        GameObject j = (GameObject)jewelMapPosition[x, y - 1];
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jellytype = jj.gettype(jewelMapPosition[x, y - 1].tag);
                        jellySpetype = jj.getSpeType("Five");
                    }
                    ScoreMatch3(counter, startsAt, endsAt, x, false, false);
                    for (int ry = startsAt; ry <= endsAt; ry++)
                    {
                        markedForRemoval[x, ry] = true;
                        SpeClear(x, ry, CheckSpe(x, ry), ref markedForRemoval);
                    }
                }
            }
        }
        #endregion

        #region 方块消
        for (int y = 0; y < boardSize; y++)
        {
            int counter = 1;
            int startsAt = 0;
            int endsAt = -1;

            for (int x = 0; x < boardSize; x++)
            {
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && IsJewelAt(x, y - 1) && IsJewelAt(x - 1, y - 1) && (jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag)
                    && (jewelMapPosition[x - 1, y].tag == jewelMapPosition[x, y - 1].tag) && (jewelMapPosition[x, y - 1].tag == jewelMapPosition[x - 1, y - 1].tag))
                {
                    counter = 4;
                    if (counter == 4 && jewelMapPosition[x, y].tag != "Rock")
                    {
                        SquareMade = true;
                        SquareMadeCount++;
                        GameObject j = (GameObject)jewelMapPosition[x, y];
                        Jewel jj = j.gameObject.GetComponent<Jewel>();
                        jellytype = jj.gettype(jewelMapPosition[x, y].tag);
                        jellySpetype = jj.getSpeType("Square");

                        ScoreMatch3(counter, startsAt, endsAt, x, false, true);
                        markedForRemoval[x, y] = true;
                        markedForRemoval[x - 1, y] = true;
                        markedForRemoval[x, y - 1] = true;
                        markedForRemoval[x - 1, y - 1] = true;
                        SpeClear(x, y, CheckSpe(x, y), ref markedForRemoval);
                        SpeClear(x - 1, y, CheckSpe(x - 1, y), ref markedForRemoval);
                        SpeClear(x, y - 1, CheckSpe(x, y - 1), ref markedForRemoval);
                        SpeClear(x - 1, y - 1, CheckSpe(x - 1, y - 1), ref markedForRemoval);
                        if (SquareMade == true)
                        {
                            SquareJewels.Add(jewelMapPosition[x, y]);
                        }
                    }
                }
                else
                {
                    counter = 1;
                }
            }
        }
        #endregion

        //标记的可消除宝石进行消除
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                //如果该点可以消除
                if (markedForRemoval[x, y])
                    RemoveJewelAt(x, y);
            }
        }
        #region 消除位置特殊宝石生成
        //生成方块特殊宝石
        if (SquareMade == true)
        {
            if (jellytype != -1 && jellySpetype != -1 && SquareJewels.Count != 0)
            {
                for (int i = 0; i < SquareJewels.Count; i++)
                {

                    MadeSpe(Convert.ToInt32(SquareJewels[i].transform.localPosition.x), Convert.ToInt32(SquareJewels[i].transform.localPosition.y), jellytype, jellySpetype);
                    GameObject j = (GameObject)jewelMapPosition[Convert.ToInt32(SquareJewels[i].transform.localPosition.x), Convert.ToInt32(SquareJewels[i].transform.localPosition.y)];
                    jewelRemains.Remove(j);
                    GameObject jj = (GameObject)jewelMapPosition[Convert.ToInt32(SquareJewels[i].transform.localPosition.x), Convert.ToInt32(SquareJewels[i].transform.localPosition.y)];
                    SquareJewels.Remove(jj);
                }
                SquareJewels.Clear();
            }
            SquareMade = false;
            jellytype = -1;
            jellySpetype = -1;
        }
        //生成横消4连宝石
        if (FourMadeH == true)
        {
            if (jellytype != -1 && jellySpetype != -1 && FourHJewels.Count != 0)
            {
                for (int i = 0; i < FourHJewels.Count; i++)
                {
                    MadeSpe(Convert.ToInt32(FourHJewels[i].transform.localPosition.x), Convert.ToInt32(FourHJewels[i].transform.localPosition.y), jellytype, jellySpetype);
                    GameObject j = (GameObject)jewelMapPosition[Convert.ToInt32(FourHJewels[i].transform.localPosition.x), Convert.ToInt32(FourHJewels[i].transform.localPosition.y)];
                    jewelRemains.Remove(j);
                    GameObject jj = (GameObject)jewelMapPosition[Convert.ToInt32(FourHJewels[i].transform.localPosition.x), Convert.ToInt32(FourHJewels[i].transform.localPosition.y)];
                    FourHJewels.Remove(jj);
                }
                FourHJewels.Clear();
            }
            FourMadeH = false;
            jellytype = -1;
            jellySpetype = -1;
        }
        //生成纵消四连宝石
        if (FourMadeV == true)
        {
            if (jellytype != -1 && jellySpetype != -1 && FourVJewels.Count != 0)
            {
                for (int i = 0; i < FourVJewels.Count; i++)
                {
                    MadeSpe(Convert.ToInt32(FourVJewels[i].transform.localPosition.x), Convert.ToInt32(FourVJewels[i].transform.localPosition.y), jellytype, jellySpetype);
                    GameObject j = (GameObject)jewelMapPosition[Convert.ToInt32(FourVJewels[i].transform.localPosition.x), Convert.ToInt32(FourVJewels[i].transform.localPosition.y)];
                    jewelRemains.Remove(j);
                    GameObject jj = (GameObject)jewelMapPosition[Convert.ToInt32(FourVJewels[i].transform.localPosition.x), Convert.ToInt32(FourVJewels[i].transform.localPosition.y)];
                    FourVJewels.Remove(jj);

                }
                FourVJewels.Clear();
            }
            FourMadeV = false;
            jellytype = -1;
            jellySpetype = -1;
        }
        //生成5连消宝石
        if (FiveMade == true)
        {
            if (jellytype != -1 && jellySpetype != -1 && FourVJewels.Count != 0)
            {
                for (int i = 0; i < FourVJewels.Count; i++)
                {
                    MadeSpe(Convert.ToInt32(FiveJewels[i].transform.localPosition.x), Convert.ToInt32(FiveJewels[i].transform.localPosition.y) - 1, jellytype, jellySpetype);
                    GameObject j = (GameObject)jewelMapPosition[Convert.ToInt32(FiveJewels[i].transform.localPosition.x), Convert.ToInt32(FiveJewels[i].transform.localPosition.y)];
                    jewelRemains.Remove(j);
                    GameObject jj = (GameObject)jewelMapPosition[Convert.ToInt32(FiveJewels[i].transform.localPosition.x), Convert.ToInt32(FiveJewels[i].transform.localPosition.y)];
                    FiveJewels.Remove(jj);
                }
                FiveJewels.Clear();
            }
            FiveMade = false;
            jellytype = -1;
            jellySpetype = -1;
        }
        #endregion
        needToDropJewels = true;
        delayDrop = 0.3f;
        canSwap = false;

        return triplets;
    }

    /// <summary>
    /// 计算当前可移动的数量
    /// </summary>
    /// <returns></returns>
    public int HowManyMovesLeft()
    {
        int moves = 0;//计算当前可移动步数
        hintJewels.Clear();//清除提示宝石list
        hintTimer = 0;//提示时间初始化为0

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (!IsJewelAt(x, y)) continue;
                // 横向移动一次宝石
                if (IsJewelAt(x + 1, y))
                {
                    GameObject swap = jewelMapPosition[x, y];
                    jewelMapPosition[x, y] = jewelMapPosition[x + 1, y];
                    jewelMapPosition[x + 1, y] = swap;
                    int m = CountMatch3();//判断是否存在消除
                    if (m > 0) //存在消除则把该位置记录到提示宝石list中
                    {
                        hintJewels.Add(jewelMapPosition[x, y]);
                        hintJewels.Add(jewelMapPosition[x + 1, y]);
                    }
                    moves += m;

                    // 把宝石交换回来
                    jewelMapPosition[x + 1, y] = jewelMapPosition[x, y];
                    jewelMapPosition[x, y] = swap;
                }

                // 纵向移动一次宝石
                if (IsJewelAt(x, y + 1))
                {
                    GameObject swap = jewelMapPosition[x, y];
                    jewelMapPosition[x, y] = jewelMapPosition[x, y + 1];
                    jewelMapPosition[x, y + 1] = swap;
                    int m = CountMatch3();
                    if (m > 0)
                    {
                        hintJewels.Add(jewelMapPosition[x, y]);
                        hintJewels.Add(jewelMapPosition[x, y + 1]);
                    }
                    moves += m;

                    // 宝石交换回来
                    jewelMapPosition[x, y + 1] = jewelMapPosition[x, y];
                    jewelMapPosition[x, y] = swap;
                }
            }
        }

        return moves;
    }

    /// <summary>
    /// 设置闪动
    /// </summary>
    void OnGUI()
    {
        if (flashIt)
        { //Just flashes the Screen
            aFlash -= 0.01f;
            GUI.color = new Color(1, 1, 1, aFlash);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), flash);
            if (aFlash < 0.01f)
            {
                flashIt = false;
                aFlash = 1;
            }
        }
    }

    /// <summary>
    /// 这个函数会显示当前消除的得分，在消除的位置
    /// </summary>
    /// <param name="a">分数</param>
    /// <param name="position">位置</param>
    void PointsAni(int a, Vector2 position)
    {
        GameObject msg2 = (GameObject)Instantiate(Resources.Load("Prefabs/puzzleUI/MatchMessage2"));
        msg2.transform.position = new Vector3(TheBoard.position.x - position.x, TheBoard.position.y - position.y, msg2.transform.position.z);
        MessageMatch m = msg2.GetComponent<MessageMatch>();
        TextMesh PointText = msg2.GetComponent<TextMesh>();
        if (a > 0) msg2.GetComponent<Renderer>().material.color = Color.green;
        if (a < 0) msg2.GetComponent<Renderer>().material.color = Color.red;
        m.direction = 1;
        m.wait = true; m.howLong = 0.3f;
        if (a > 0) PointText.text = "+" + a.ToString() + " pts"; else PointText.text = a.ToString() + " pts";
    }

    //Score boost 
    void ScoreBoost(int howMuch, int howLong)
    {
        ScoreBoostTimer = Time.timeSinceLevelLoad + howLong;
        ScoreBoostValue = howMuch;
    }
    /// <summary>
    /// 创建特殊宝石
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    /// <param name="type">宝石种类</param>
    /// <param name="spetype">特殊种类</param>
    void MadeSpe(int x, int y, int type, int spetype)
    {
        if (IsJewelExist(x, y) == false && jewelMapPosition[x, y] == null)
        {
            GameObject objectType1 = jewelsSpe[type];//随机添加一个宝石
            //获取到宝石后实例化一个宝石预设体
            GameObject jewelPrefab = (GameObject)Instantiate(objectType1, new Vector3(x, y, 0), jewels[0].transform.localRotation);
            switch (spetype)
            {
                case 0://横4
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsH");
                    break;
                case 1://连5
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsFive");
                    break;
                case 2://方块
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsSqu");
                    break;
                case 3://TL
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsTL");
                    break;
                case 4://纵4
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsV");
                    break;
            }
            Material mm = jewelPrefab.GetComponent<Renderer>().material;
            String name = mm.mainTexture.name;
            jewelMapPosition[x, y] = jewelPrefab;//放入对应坐标
            jewelPrefab.transform.parent = TheBoard;
            jewelPrefab.transform.localPosition = new Vector3(x, y, 0);
        }
    }
    /// <summary>
    /// 检测当前位置是否是特殊宝石
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>返回消除种类</returns>
    int CheckSpe(int x, int y)
    {
        int Spetype = -1;
        GameObject jewelPrefab = jewelMapPosition[x, y];
        Material mm = jewelPrefab.GetComponent<Renderer>().material;
        String name = mm.mainTexture.name;
        switch (name)
        {
            case "JewelsH":
                Spetype = 0;
                break;
            case "JewelsFive":
                Spetype = 1;
                break;
            case "JewelsSqu":
                Spetype = 2;
                break;
            case "JewelsV":
                Spetype = 3;
                break;
            case "JewelsTL":
                Spetype = 4;
                break;
            default:
                Spetype = -1;
                break;
        }
        return Spetype;
    }
    /// <summary>
    /// 特殊消除效果
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <param name="spetype">特殊宝石种类</param>
    /// <param name="markedForRemoval">确认该区域是否可以清除宝石</param>
    void SpeClear(int x, int y, int spetype, ref bool[,] markedForRemoval)
    {
        //横4宝石
        if (spetype == 0)
        {
            for (int i = 0; i < boardSize; i++)
            {
                if (IsJewelAt(i, y) && markedForRemoval[i, y] == false)
                    markedForRemoval[i, y] = true;
            }
        }
        //纵4宝石
        if (spetype == 3)
        {
            for (int i = 0; i < boardSize; i++)
            {
                if (IsJewelAt(x, i) && markedForRemoval[x, i] == false)
                    markedForRemoval[x, i] = true;
            }
        }
        //方块宝石
        if (spetype == 2)
        {
            int px = UnityEngine.Random.Range(0, boardSize - 1);
            int py = UnityEngine.Random.Range(0, boardSize - 1);
            for (int i = 0; i <= 4; i++)
            {
                if (i == 0)
                {
                    if (IsJewelAt(px, py))
                    {
                        if (markedForRemoval[px, py] == false)
                            markedForRemoval[px, py] = true;
                    }
                    else
                        continue;
                }
                if (i == 1)
                {
                    if (IsJewelAt(px + 1, py))
                    {
                        if (markedForRemoval[px + 1, py] == false)
                            markedForRemoval[px + 1, py] = true;
                    }
                    else
                        continue;
                }
                if (i == 2)
                {
                    if (IsJewelAt(px - 1, py))
                    {
                        if (markedForRemoval[px - 1, py] == false)
                            markedForRemoval[px - 1, py] = true;
                    }
                    else
                        continue;
                }
                if (i == 3)
                {
                    if (IsJewelAt(px, py + 1))
                    {
                        if (markedForRemoval[px, py + 1] == false)
                            markedForRemoval[px, py + 1] = true;
                    }
                    else
                        continue;
                }
                if (i == 4)
                {
                    if (IsJewelAt(px, py - 1))
                    {
                        if (markedForRemoval[px, py - 1] == false)
                            markedForRemoval[px, py - 1] = true;
                    }
                    else
                        continue;
                }
            }
        }
        //TL消
        if (spetype == 4)
        {

        }
    }
}
