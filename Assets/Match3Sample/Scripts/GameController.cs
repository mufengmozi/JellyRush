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
    /// �������״̬
    /// </summary>
    public enum BState
    {  						//State of the Board
        PLAYING,
        DROP_JEWELS,
        SWAPPING,
        GAMEOVER,
        RESETING
    };
    public static int CurrentlyMovingJewels; 	//���Ʊ�ʯ���ƶ�. Controlled by Jewel.cs
    protected BState theBoardState;				//��ʾ��ǰ����״̬	
    protected bool isActive;					//��ʾ��ǰ����Ƿ񼤻�			
    protected bool canSwap;						//��ʾ��ǰ�Ƿ�����ƶ�		
    protected string xmlpath;
    /// <summary>
    /// �б����ڼ�¼��ʯӦ�ñ��ƻ����Ǳ���
    /// </summary>
    private List<GameObject> jewelRemains;		//�б����ڼ�¼��ʯӦ�ñ��ƻ����Ǳ���
    private bool needToDropJewels;				//Controls the Dropping Jewels
    /// <summary>
    /// ��¼�����˶��ٸ���ʯ�������ڼ��������
    /// </summary>
    private int totalRemovedJewels;
    public GameObject SelectedJewel;			//Can be used as Prefab holder for highlighting a selected jewel
    private Jewel tip;							//Control the Jewel Alpha hint
    public AudioClip SwapSound;					//SwapSound
    public AudioClip explodeSound;				//ExplodeSound
    public AudioClip match3Sound;				//Match3Sound
    private bool needToCheckCascades = false;	//�ж��Ƿ���Ҫ��鱦ʯ��ǰ״̬
    private bool mouseLeftDown = false;			//�����갴���Ƿ�̧��
    private int totalTriplets = 0;				//��3�ļ���
    private int movesLeft = 0;                  //��¼��ǰ�����ƶ��Ĳ���

    #region ���ⷽ��ʹ�ò���
    /// <summary>
    /// �ж��Ƿ���ڷ�����
    /// </summary>
    protected bool SquareMade;
    /// <summary>
    /// ���㷽����������
    /// </summary>
    protected int SquareMadeCount = 0;
    /// <summary>
    /// �ж��Ƿ���ں�����������
    /// </summary>
    protected bool FourMadeH;
    /// <summary>
    /// �����������������
    /// </summary>
    protected int FourMadeHCount = 0;
    /// <summary>
    /// �ж��Ƿ����������������
    /// </summary>
    protected bool FourMadeV;
    /// <summary>
    /// ������������������
    /// </summary>
    protected int FourMadeVCount = 0;
    /// <summary>
    /// �ж��Ƿ������������
    /// </summary>
    protected bool FiveMade;
    /// <summary>
    /// ��������������
    /// </summary>
    protected int FiveMadeCount = 0;
    /// <summary>
    /// �ж��Ƿ����TL����
    /// </summary>
    protected bool TLMade;
    /// <summary>
    /// ����TL��������
    /// </summary>
    protected int TLMadeCount = 0;
    /// <summary>
    /// ��ȡ���ⷽ�������
    /// </summary>
    protected int jellytype = -1;
    /// <summary>
    /// ��ȡ���ⷽ����������
    /// </summary>
    protected int jellySpetype = -1;
    /// <summary>
    /// �����������ⱦʯ��
    /// </summary>
    private List<GameObject> FourHJewels;
    /// <summary>
    /// �����������ⱦʯ��
    /// </summary>
    private List<GameObject> FourVJewels;
    /// <summary>
    /// �������ⱦʯ��
    /// </summary>
    private List<GameObject> FiveJewels;
    /// <summary>
    /// �������ⱦʯ��
    /// </summary>
    private List<GameObject> SquareJewels;
    /// <summary>
    /// TL���ⱦʯ��
    /// </summary>
    private List<GameObject> TLJewels;
    #endregion

    public GameObject Rock;						//Can be used as Rock or special item 
    public GameObject[] Obs;                     //������ϰ���
    public GameObject[] planes;                 //����ǵײ������
    public GameObject[] jewels;                //����������Ϸ�ϵı�ʯ������Ҫ3�ֲ�ͬ��
    public GameObject[] jewelsSpe;                 //����������Ϸ�ϵı�ʯ������Ҫ3�ֲ�ͬ��
    /// <summary>
    /// ȷ����λ�ô������ⱦʯ
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

        //������������ʼ��
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
    private float delayDrop;				// ���Ƶ��䱦ʯ����ʱ

    private int boardSizex;                 //������Xֵ
    private int boardSizey;                 //������Yֵ

    private List<GameObject> hintJewels;	//controls the Hint Jewels	//��ʾ��ʯ������
    private float hintTimer;				//hint timer
    private float hintDelay;				//hint timer delay

    private int baseScore;					//that's the base score of the game I've set it to 100 but you can change
    private int longestChain;				//controls the longest chain
    protected int mouseClickX, mouseClickY;	//controls the mouseclick based on the jewel position

    private int skullInitial = 100;  		//how many points the skull will reduce�������ü���

    public Texture2D flash;					//Texture for the Screen Flash
    public static bool flashIt;				//Flashes the Screen if true
    private float aFlash = 1;				//Alpha that controls the flash timer ��˸ʱ��

    private float ScoreBoostTimer;			//You can use this variable to boost your score for some time
    private int ScoreBoostValue;			//value if you want to use the scoreboost
    public Transform TheBoard;				//that's the board GameObject where the board will be placed

    GameObject Can; //�õ�ugui��canvas�����в���

    /// <summary>
    /// �������
    /// </summary>
    void Start()
    {
        Reset();
        Debug.Log(PlayerPrefs.GetInt("Level"));
    }
    /// <summary>
    /// ���״̬չʾ
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
    /// �������
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
    /// ���������ʼ�������䱦ʯ
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
                            // �Ų���ʱ��Ҫ�������3�������
                            objectType1 = planes[0];//���һ������ʵ��
                            //��ȡ����ʯ��ʵ����һ����ʯԤ����
                            planePrefab = (GameObject)Instantiate(objectType1, new Vector3(j, i, 0.01f), planes[0].transform.localRotation);
                            planeMapPosition[j, i] = planePrefab;//�����Ӧ����
                            planePrefab.transform.parent = TheBoard;
                            planePrefab.transform.localPosition = new Vector3(j, i, 0.01f);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// �����ϰ���Ϣ
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
                            obsMapPosition[j, i] = obsPrefab;//�����Ӧ���� = obsPrefab;//�����Ӧ����
                            obsPrefab.transform.parent = TheBoard;
                            obsPrefab.transform.localPosition = new Vector3(j, i, -0.01f);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// ���������ʼ�������䱦ʯ
    /// </summary>
    private void ResetJewels()
    {
        int x, y;
        //������屦ʯ֮ǰ��ɾ�����еı�ʯ
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
        //��ʼ���ñ�ʯ1��1�е��Ű�
        for (y = 0; y < boardSize; y++)
        {
            for (x = 0; x < boardSize; x++)
            {
                GameObject j = (GameObject)jewelMapPosition[x, y];
                GameObject pl = (GameObject)planeMapPosition[x, y];
                GameObject ob = (GameObject)obsMapPosition[x, y];

                if (j == null && pl != null && ob == null)
                {
                    // �Ų���ʱ��Ҫ�������3�������

                    GameObject objectType1 = jewels[UnityEngine.Random.Range(0, jewels.Length - 3)];//������һ����ʯ
                    //��ȡ����ʯ��ʵ����һ����ʯԤ����
                    GameObject jewelPrefab = (GameObject)Instantiate(objectType1, new Vector3(x, y, 0), jewels[0].transform.localRotation);
                    jewelMapPosition[x, y] = jewelPrefab;//�����Ӧ����
                    jewelPrefab.transform.parent = TheBoard;
                    jewelPrefab.transform.localPosition = new Vector3(x, y, 0);
                    //ѭ���ж���3���
                    while (true)
                    {
                        if (CountMatch3() == 0) break;//�ж��Ƿ������3�����������������ѭ���������һ��
                        //������3��������ٵ�ǰ��ʯ������ʵ����һ���±�ʯ
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
    /// ���������ʼ�������䱦ʯ
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
                                objectType1 = jewels[3];//���һ���ض���ʯ
                            }
                            else
                            {
                                objectType1 = jewels[UnityEngine.Random.Range(0, jewels.Length - 3)];//������һ����ʯ
                            }
                            GameObject jewelPrefab = (GameObject)Instantiate(objectType1, new Vector3(j, i, 0), jewels[0].transform.localRotation);
                            jewelMapPosition[j, i] = jewelPrefab;//�����Ӧ����
                            jewelPrefab.transform.parent = TheBoard;
                            jewelPrefab.transform.localPosition = new Vector3(j, i, 0);
                            //ѭ���ж���3���
                            while (true)
                            {
                                if (CountMatch3() == 0) break;//�ж��Ƿ������3�����������������ѭ���������һ��
                                //������3��������ٵ�ǰ��ʯ������ʵ����һ���±�ʯ
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
    /// �߼�����
    /// </summary>
    void Update()
    {
        Text();
        if (theBoardState == BState.RESETING) return; //�鿴��嵱ǰ״̬

        UpdateJewelRemains();  						//���±�ʯ״̬
        //�ж�����Ƿ��ڼ���״̬
        if (!isActive) return; 						//��崦���ƶ�״̬ʱ����δ����״̬
        //�жϵ�ǰ���ƶ��ı�ʯ�м���
        //if (CurrentlyMovingJewels > 0) return;

        // delayDropʱ���ʯ��ʼ���� 
        if (delayDrop > 0)
        {
            delayDrop -= (float)Time.deltaTime;
            return;
        }
        // ����ƶ���ʯ����0
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
            if (validateSwap)//�����˽������
            { 			// �����Ժ���֤��ʯλ���Ƿ������3
                int count = CountMatch3();
                if (count == 0)
                {
                    SwapJewels(swapping);	// ��������ھͻ���ȥ
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

        if (theBoardState != BState.PLAYING && theBoardState != BState.SWAPPING) return; // �ж��û��Ƿ���Խ��н���

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

    #region ��ʱ���õ����ֲ�ϵͳ
    private void ResetJewelse()
    {
        string gate = XmlHelper.FindByName(xmlpath, "Map");
        Array arr = gate.Split(';');
        string line;
        GameObject je;//���ڼ�¼��ʯ��λ��
        GameObject pl;//���ڼ�¼����λ��
        GameObject obs;//���ڼ�¼�ϰ���λ��
        GameObject objectType1;//��¼��ʯ������
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
                            // �Ų���ʱ��Ҫ�������3�������
                            objectType1 = jewels[UnityEngine.Random.Range(0, jewels.Length)];//������һ����ʯ
                            //��ȡ����ʯ��ʵ����һ����ʯԤ����
                            jewelPrefab = (GameObject)Instantiate(objectType1, new Vector3(i, j, 0), jewels[0].transform.localRotation);
                            jewelMapPosition[i, j] = jewelPrefab;//�����Ӧ����
                            jewelPrefab.transform.parent = TheBoard;
                            jewelPrefab.transform.localPosition = new Vector3(i, j, 0);
                            //ѭ���ж���3���
                            while (true)
                            {
                                if (CountMatch3() == 0) break;//�ж��Ƿ������3�����������������ѭ���������һ��
                                //������3��������ٵ�ǰ��ʯ������ʵ����һ���±�ʯ
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
                                    obs = obsPrefab;//�����Ӧ����

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
    /// �����������һ�������ı�ʯ
    /// </summary>
    /// <param name="tagType">��ʯ����</param>
    /// <param name="howMuch">����</param>
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
    /// ��û�п����ƶ��ı�ʯ��ʱ��
    /// ���ñ�ʯ
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
    /// �����ʾһ�����ѱ�ʯ
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
    /// ������ʯ��λ��
    /// </summary>
    /// <param name="swap"></param>
    private void SwapJewels(theSwap swap)
    {
        //Debug.Log("di er ci"+swapping.jewelBx + "" + swapping.jewelBy);
        int dirA, dirB;//����������ʯ�Ľ�������1��2�ң�3�ϣ�4��
        bool verifyRock = false;//�ж��Ƿ��Ǹ�ʯͷ
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
        //��������ʯ��tag��ʯͷ����verifyRock=true
        if (jewelMapPosition[swap.jewelAx, swap.jewelAy].tag == "Rock" || jewelMapPosition[swap.jewelBx, swap.jewelBy].tag == "Rock")
        {
            verifyRock = true;
        }
        else
        {
            verifyRock = false;
        }

        //�������ʯͷ�ͽ��н���
        if (!verifyRock)
        { //�����������
            objIn = jewelMapPosition[swap.jewelAx, swap.jewelAy].gameObject.GetComponent<Jewel>();
            objIn.Move(dirA);

            objIn = jewelMapPosition[swap.jewelBx, swap.jewelBy].gameObject.GetComponent<Jewel>();
            objIn.Move(dirB);

            GameObject j = jewelMapPosition[swap.jewelAx, swap.jewelAy];
            jewelMapPosition[swap.jewelAx, swap.jewelAy] = jewelMapPosition[swap.jewelBx, swap.jewelBy];
            jewelMapPosition[swap.jewelBx, swap.jewelBy] = j;
        }

        //��ȡ��ʯA�����(0:��4\1:5\2:����\3:��4\4:TL\-1:��ͨ)
        int SpeA = CheckSpe(swap.jewelAx, swap.jewelAy);
        //��ȡ��ʯA�����(0:��4\1:5\2:����\3:��4\4:TL\-1:��ͨ)
        int SpeB = CheckSpe(swap.jewelBx, swap.jewelBy);

        #region ��ʯA��5����  1
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
            //����һ��
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    //����õ��������
                    if (markedForRemoval[x, y])
                        RemoveJewelAt(x, y);
                }
            }
            needToDropJewels = true;
            delayDrop = 0.3f;
            canSwap = false;
        }
        #endregion
        #region ��ʯA��4����  0
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

            //����һ��
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    //����õ��������
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
        #region ��ʯA�������� 2
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


            //����һ��
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    //����õ��������
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
        #region ��ʯA��4����  3
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

            //����һ��
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    //����õ��������
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
        #region ��ʯATL����   4
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

        //    //����һ��
        //    for (int y = 0; y < boardSize; y++)
        //    {
        //        for (int x = 0; x < boardSize; x++)
        //        {
        //            //����õ��������
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
    /// �ж��Ƿ�Ϊ�Ϸ��Ľ���
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
    /// �������ȱ���ж���
    /// </summary>
    /// <param name="x">X����</param>
    /// <param name="y">Y����</param>
    /// <param name="checkUp">ture��Ϊ�м��ȱ�ڡ�false�Ǳ߽��ȱ��</param>
    /// <returns></returns>
    private int HowBigHoleAt(int x, int y, bool checkUp)
    {
        int length = 0;
        if (checkUp)
        {
            for (int hy = y; hy >= 0; hy--)
            {
                GameObject ob = (GameObject)obsMapPosition[x, hy];//�ϰ�
                GameObject pl = (GameObject)planeMapPosition[x, hy];//���
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
                GameObject ob = (GameObject)obsMapPosition[x, hy];//�ϰ�
                GameObject pl = (GameObject)planeMapPosition[x, hy];//���
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
    /// ���䱦ʯ,(x,y)��Ҫ���䱦ʯ������
    /// </summary>
    /// <param name="x">X����</param>
    /// <param name="y">Y����</param>
    /// <param name="slots">ȱ��</param>
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
        /*��ʡ�Ե�����*/
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
        //    if (x < 7)//���ұߵĵط���һ������
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
    /// �жϵ�ǰ�������Ƿ�������
    /// </summary>
    /// <param name="x">X����</param>
    /// <param name="y">Y����</param>
    /// <returns>���ڷ���true</returns>
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
    /// �жϵ�ǰ�������Ƿ�������
    /// </summary>
    /// <param name="x">X����</param>
    /// <param name="y">Y����</param>
    /// <returns>���ڷ���true</returns>
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
    /// ���齵��ı�ʯ
    /// </summary>
    public void CheckForFallingJewels()
    {
        for (int x = 0; x < boardSize; x++)
        {			// �����Ҵ��µ��ϵļ���ÿһ�еı�ʯ
            int totalSlots = 0;
            for (int y = boardSize - 1; y > 0; y--)
            {	// search for holes
                GameObject pl = (GameObject)planeMapPosition[x, y];
                GameObject ob = (GameObject)obsMapPosition[x, y];
                int slotsd;
                if (!IsJewelAt(x, y) && pl != null && ob == null)
                { 				//�������ȱ�ڼ���ȱ���ж���
                    int slots = HowBigHoleAt(x, y, true);
                    totalSlots += slots;
                    y -= slots;
                    slotsd = ShiftJewelsDown(x, y, slots); 		// ����֪��ȱ���е���ÿһ����ʯ
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
                int slots = HowBigHoleAt(x, 0, false);	// �������ȱ�ڼ���ȱ���ж���

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
    /// �ж��Ƿ��������
    /// </summary>
    public void CheckForCascades()
    {
        int total = RemoveTripletsAft();//�ж��Ƿ��������
        if (total == 0)
        { 						// û�п����������
            totalTriplets = 0;
            canSwap = true;

            movesLeft = HowManyMovesLeft();// ���㵱ǰ�����ƶ��Ĳ���

            if (movesLeft == 0)
            {				// ʹ�����������������ʱ���û������
                PopulateWithNewJewels();	// ����������������µ��鱦
            }
        }
    }

    /// <summary>
    /// ȷ�ϸ�λ���Ƿ���Է��ñ�ʯ
    /// </summary>
    /// <param name="x">x����</param>
    /// <param name="y">y����</param>
    /// <returns>���ܷ��÷���false</returns>
    public bool IsJewelAt(int x, int y)
    {
        //x��y���겻��С��0�����ܳ�Խ�߽磬�߽��0��ʼ����
        if (x < 0 || y < 0 || x > boardSize - 1 || y > boardSize - 1) return false;
        return jewelMapPosition[x, y] != null;
    }

    /// <summary>
    /// ȷ�ϸ�λ���Ƿ���ڱ�ʯ
    /// </summary>
    /// <param name="x">x����</param>
    /// <param name="y">y����</param>
    /// <returns>���ܷ��÷���false</returns>
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
    /// ������ǰ�����µı�ʯ
    /// </summary>
    /// <param name="x">X����</param>
    /// <param name="y">Y����</param>
    public void RemoveJewelAt(int x, int y)
    {
        if (!IsJewelAt(x, y)) return;
        if (x >= 0 && x < boardSize && y >= 0 && y < boardSize && PlaneExist(x, y))
        {
            GameObject j = (GameObject)jewelMapPosition[x, y];
            jewelMapPosition[x, y] = null;
            Jewel jj = j.gameObject.GetComponent<Jewel>();//��ȡ��Jewel�ű�
            jj.Die();
            jewelRemains.Add(j);
            totalRemovedJewels++;
            RemoveObs(x, y);
        }
    }

    /// <summary>
    /// ������ǰ�����µı�ʯ
    /// </summary>
    /// <param name="x">X����</param>
    /// <param name="y">Y����</param>
    public void RemoveObs(int x, int y)
    {
        if (ObsExist(x + 1, y))
        {
            GameObject o = (GameObject)obsMapPosition[x+1, y];
            obsMapPosition[x+1, y] = null;
            obstracle ob = o.gameObject.GetComponent<obstracle>();//��ȡ��obstracle�ű�
            ob.Die();
        }
        if (ObsExist(x - 1, y))
        {
            GameObject o = (GameObject)obsMapPosition[x - 1, y];
            obsMapPosition[x - 1, y] = null;
            obstracle ob = o.gameObject.GetComponent<obstracle>();//��ȡ��obstracle�ű�
            ob.Die();
        }
        if (ObsExist(x, y + 1))
        {
            GameObject o = (GameObject)obsMapPosition[x, y + 1];
            obsMapPosition[x, y + 1] = null;
            obstracle ob = o.gameObject.GetComponent<obstracle>();//��ȡ��obstracle�ű�
            ob.Die();
        }
        if (ObsExist(x, y - 1))
        {
            GameObject o = (GameObject)obsMapPosition[x, y - 1];
            obstracle ob = o.gameObject.GetComponent<obstracle>();//��ȡ��obstracle�ű�
            ob.Die();
            obsMapPosition[x, y - 1] = null;
        }
    }

    /// <summary>
    /// ���±�ʯ״̬
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
    /// �ж��Ƿ������3
    /// </summary>
    /// <returns value='triple'>����Ϊ0�������3���</returns>
    public int CountMatch3()
    {
        int triple = 0;

        #region ����
        for (int y = 0; y < boardSize; y++)
        { 		// Horizontal
            int counter = 1;
            for (int x = 1; x < boardSize; x++)
            {
                //�жϸ�λ�ÿɷ��ã�ˮƽǰһ��λ�ÿɷ��ò��Ҷ���tag��ͬ
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag)
                {
                    counter++;
                    if (counter >= 3 && jewelMapPosition[x, y].tag != "Rock")
                    {
                        triple++;					// ������3���������+1
                    }
                }
                else
                {
                    counter = 1;
                }
            }
        }
        #endregion

        #region �ж�����
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

        #region �жϷ�����
        for (int y = 0; y < boardSize; y++)
        { 		// ������ÿһ��X
            int counter = 1;
            for (int x = 0; x < boardSize; x++)
            {
                //�жϸ�λ�ÿɷ��ã�ˮƽǰһ��λ�ÿɷ��ò��Ҷ���tag��ͬ
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && IsJewelAt(x, y - 1) && IsJewelAt(x - 1, y - 1) && (jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag)
                    && (jewelMapPosition[x - 1, y].tag == jewelMapPosition[x, y - 1].tag) && (jewelMapPosition[x, y - 1].tag == jewelMapPosition[x - 1, y - 1].tag))
                {
                    counter = 4;
                    if (counter == 4 && jewelMapPosition[x, y].tag != "Rock")
                    {
                        triple = 6;					//��ǰ���ڷ����������triple����Ϊ6
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
    /// ��3������������
    /// </summary>
    /// <param name="jewels">������ʯ����</param>
    /// <param name="start">��ʼλ��</param>
    /// <param name="end">����λ��</param>
    /// <param name="lineRow">����</param>
    /// <param name="horizontal">�Ƿ�ˮƽ</param>
    private void ScoreMatch3(int jewels, int start, int end, int lineRow, bool horizontal, bool rectangle)
    {
        int tripletScore = (jewels - 2) * baseScore;
        string itemMatched = "";
        bool foundSkull = false;
        totalTriplets++;
        //�����3����1��
        if (totalTriplets > 1)
            tripletScore += 30 * (totalTriplets - 1);//��һ����30��

        if (totalTriplets > longestChain)			// ���㳤��
            longestChain = totalTriplets;
        //���������ļ�
        GetComponent<AudioSource>().pitch = 1;
        GetComponent<AudioSource>().pitch = 1 + (totalTriplets * 0.2f);

        if (horizontal)
        {
            // Horizontal match 3 or more
            PointCount(jewels, tripletScore, start, lineRow, end, jewelMapPosition[start, lineRow].tag, new Vector2(start + 1, lineRow));
            if (jewelMapPosition[start, lineRow].tag == "bomb") foundSkull = true;//���������
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
    /// ����Ļ�������Ч
    /// </summary>
    /// <param name="jewels">�����ı�ʯ����</param>
    /// <param name="score">��ǰ��õķ���</param>
    /// <param name="start">��ʼλ��</param>
    /// <param name="lineRow">����</param>
    /// <param name="end">����λ��</param>
    /// <param name="item">��ʯ����</param>
    /// <param name="middle">�е�</param>
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
        //��������ã����ű�ը����
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
    /// ������3���(��������)
    /// </summary>
    /// <returns></returns>
    private int RemoveTriplets(int count)
    {
        int triplets = count;
        if (triplets == 0) return 0;			// �������
        //����һ����ά��������¼��ǰλ���Ƿ��������
        bool[,] markedForRemoval = new bool[boardSize, boardSize];

        #region ������3����
        for (int y = 0; y < boardSize; y++)
        {	// Horizontal
            int counter = 1;
            int startsAt = 0;
            int endsAt = -1;

            for (int x = 1; x < boardSize; x++)
            {
                //�����λ�úͺ���ǰһ��λ�ö����ڡ����Ҷ��ߵ�tag��ͬ�����Ҹñ�ʯtag����rock
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag && jewelMapPosition[x, y].tag != "Rock")
                {
                    counter++;
                    endsAt = x;//��X�����滻ԭ����
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

        #region ������3����
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

        #region ������
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
                //����õ��������
                if (markedForRemoval[x, y])
                    RemoveJewelAt(x, y);
            }
        }
        #region ������λ���������ⱦʯ
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
    /// ������3���������������
    /// </summary>
    /// <returns></returns>
    private int RemoveTripletsAft()
    {
        int triplets = CountMatch3();
        if (triplets == 0) return 0;			// �������
        //����һ����ά���������ڵ�ǰλ���Ƿ��������
        bool[,] markedForRemoval = new bool[boardSize, boardSize];

        #region ������3����
        for (int y = 0; y < boardSize; y++)
        {	// Horizontal
            int counter = 1;
            int startsAt = 0;
            int endsAt = -1;

            for (int x = 1; x < boardSize; x++)
            {
                //�����λ�úͺ���ǰһ��λ�ö����ڡ����Ҷ��ߵ�tag��ͬ�����Ҹñ�ʯtag����rock
                if (IsJewelAt(x, y) && IsJewelAt(x - 1, y) && jewelMapPosition[x, y].tag == jewelMapPosition[x - 1, y].tag && jewelMapPosition[x, y].tag != "Rock")
                {
                    counter++;
                    endsAt = x;//��X�����滻ԭ����
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

        #region ������3����
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

        #region ������
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

        //��ǵĿ�������ʯ��������
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                //����õ��������
                if (markedForRemoval[x, y])
                    RemoveJewelAt(x, y);
            }
        }
        #region ����λ�����ⱦʯ����
        //���ɷ������ⱦʯ
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
        //���ɺ���4����ʯ
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
        //��������������ʯ
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
        //����5������ʯ
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
    /// ���㵱ǰ���ƶ�������
    /// </summary>
    /// <returns></returns>
    public int HowManyMovesLeft()
    {
        int moves = 0;//���㵱ǰ���ƶ�����
        hintJewels.Clear();//�����ʾ��ʯlist
        hintTimer = 0;//��ʾʱ���ʼ��Ϊ0

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (!IsJewelAt(x, y)) continue;
                // �����ƶ�һ�α�ʯ
                if (IsJewelAt(x + 1, y))
                {
                    GameObject swap = jewelMapPosition[x, y];
                    jewelMapPosition[x, y] = jewelMapPosition[x + 1, y];
                    jewelMapPosition[x + 1, y] = swap;
                    int m = CountMatch3();//�ж��Ƿ��������
                    if (m > 0) //����������Ѹ�λ�ü�¼����ʾ��ʯlist��
                    {
                        hintJewels.Add(jewelMapPosition[x, y]);
                        hintJewels.Add(jewelMapPosition[x + 1, y]);
                    }
                    moves += m;

                    // �ѱ�ʯ��������
                    jewelMapPosition[x + 1, y] = jewelMapPosition[x, y];
                    jewelMapPosition[x, y] = swap;
                }

                // �����ƶ�һ�α�ʯ
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

                    // ��ʯ��������
                    jewelMapPosition[x, y + 1] = jewelMapPosition[x, y];
                    jewelMapPosition[x, y] = swap;
                }
            }
        }

        return moves;
    }

    /// <summary>
    /// ��������
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
    /// �����������ʾ��ǰ�����ĵ÷֣���������λ��
    /// </summary>
    /// <param name="a">����</param>
    /// <param name="position">λ��</param>
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
    /// �������ⱦʯ
    /// </summary>
    /// <param name="x">x����</param>
    /// <param name="y">y����</param>
    /// <param name="type">��ʯ����</param>
    /// <param name="spetype">��������</param>
    void MadeSpe(int x, int y, int type, int spetype)
    {
        if (IsJewelExist(x, y) == false && jewelMapPosition[x, y] == null)
        {
            GameObject objectType1 = jewelsSpe[type];//������һ����ʯ
            //��ȡ����ʯ��ʵ����һ����ʯԤ����
            GameObject jewelPrefab = (GameObject)Instantiate(objectType1, new Vector3(x, y, 0), jewels[0].transform.localRotation);
            switch (spetype)
            {
                case 0://��4
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsH");
                    break;
                case 1://��5
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsFive");
                    break;
                case 2://����
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsSqu");
                    break;
                case 3://TL
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsTL");
                    break;
                case 4://��4
                    jewelPrefab.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/Jewels/JewelsV");
                    break;
            }
            Material mm = jewelPrefab.GetComponent<Renderer>().material;
            String name = mm.mainTexture.name;
            jewelMapPosition[x, y] = jewelPrefab;//�����Ӧ����
            jewelPrefab.transform.parent = TheBoard;
            jewelPrefab.transform.localPosition = new Vector3(x, y, 0);
        }
    }
    /// <summary>
    /// ��⵱ǰλ���Ƿ������ⱦʯ
    /// </summary>
    /// <param name="x">X����</param>
    /// <param name="y">Y����</param>
    /// <returns>������������</returns>
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
    /// ��������Ч��
    /// </summary>
    /// <param name="x">X����</param>
    /// <param name="y">Y����</param>
    /// <param name="spetype">���ⱦʯ����</param>
    /// <param name="markedForRemoval">ȷ�ϸ������Ƿ���������ʯ</param>
    void SpeClear(int x, int y, int spetype, ref bool[,] markedForRemoval)
    {
        //��4��ʯ
        if (spetype == 0)
        {
            for (int i = 0; i < boardSize; i++)
            {
                if (IsJewelAt(i, y) && markedForRemoval[i, y] == false)
                    markedForRemoval[i, y] = true;
            }
        }
        //��4��ʯ
        if (spetype == 3)
        {
            for (int i = 0; i < boardSize; i++)
            {
                if (IsJewelAt(x, i) && markedForRemoval[x, i] == false)
                    markedForRemoval[x, i] = true;
            }
        }
        //���鱦ʯ
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
        //TL��
        if (spetype == 4)
        {

        }
    }
}
