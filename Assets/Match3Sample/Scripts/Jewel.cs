/* Copyright (c) Vander Amaral
 * I've created this code based on a lot of Match3 Codes I found on the Internet
 * I tried to make it the best and easy to change.
 * You can polish it even more, and add functions to it if you bought.
 */

using UnityEngine;
using System.Collections;
using System;

public class Jewel : MonoBehaviour
{

    private Vector2 position;
    private bool moving;
    private int movedSlots;
    private bool reportedMove;
    private int moveTo;
    private Vector2 targetPosition;
    private Vector2 startPosition;
    public bool Dead = false;
    private int speed;
    public bool alpha;
    private float pingP;
    private int thetype;
    /// <summary>
    /// ��ʯ����ö��
    /// </summary>
    public enum jellttype
    {
        hong = 0,
        lan = 1,
        lv = 2,
        zi = 3,
        qing = 4,
        bomb = 5,
        hp = 6,
        xp = 7,
        Rock = 8
    };
    /// <summary>
    /// ���ⱦʯ����ö��
    /// </summary>
    public enum Spejellytype
    {
        FourH = 0,
        FourV = 4,
        Five = 1,
        Square = 2,
        TL = 3
    };

    // ��ʼ����ʯ
    void Start()
    {
        SetTexture();//��������
        movedSlots = 0;//�ƶ���
        reportedMove = false;
        Dead = false;
        speed = 8;
        position = new Vector2(transform.localPosition.x, transform.localPosition.y);
    }

    /// <summary>
    /// ��ȡ��ʯ����
    /// </summary>
    /// <param name="tag">tag����</param>
    /// <returns>�������͵�ֵ</returns>
    public int gettype(string tag)
    {
        int type = 0;
        Array ary = Enum.GetValues(typeof(jellttype));
        for (int i = 0; i < ary.Length; i++)
        {
            if (ary.GetValue(i).ToString() == tag)
            {
                type = i;
            }
        }
        return type;
    }

    /// <summary>
    /// ��ȡ���ⱦʯ����
    /// </summary>
    /// <param name="tag">���ⱦʯ����</param>
    /// <returns>�������ⱦʯ���͵�ֵ</returns>
    public int getSpeType(string Spetype)
    {
        int type = 0;
        Array ary = Enum.GetValues(typeof(Spejellytype));
        for (int i = 0; i < ary.Length; i++)
        {
            if (ary.GetValue(i).ToString() == Spetype)
            {
                type = i;
            }
        }
        return type;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (alpha)
        {
            pingP = Mathf.PingPong(Time.time, 1.2f);
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, pingP);
        }
        else if (GetComponent<Renderer>().material.color.a != 1)
        {
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        }

        if (moving && !Dead)
        {
            Vector3 end = new Vector3(targetPosition.x, targetPosition.y, transform.localPosition.z);
            transform.localPosition = new Vector3(Mathf.MoveTowards(transform.localPosition.x, targetPosition.x, speed * Time.deltaTime), Mathf.MoveTowards(transform.localPosition.y, targetPosition.y, speed * Time.deltaTime), transform.localPosition.z);
            if (transform.localPosition == end)
            {
                if (GameController.jewelMapPosition[(int)transform.localPosition.x, (int)transform.localPosition.y] != this.gameObject)
                {
                    Move(3, 1);
                }
                else
                {
                    reportedMove = false;
                    if (GameController.CurrentlyMovingJewels > 0)
                        GameController.CurrentlyMovingJewels--;
                    moving = false;
                }
            }
        }
    }
    /// <summary>
    /// �ƶ�
    /// </summary>
    /// <param name="direction">����</param>
    /// <param name="Slots">���ƶ��ľ���</param>
    public void Move(int direction, int Slots)
    {

        if (direction != 0)
        {
            startPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);

            if (direction == 1)
            {
                targetPosition = new Vector2(transform.localPosition.x + Slots, transform.localPosition.y);
            }
            if (direction == 2)
            {
                targetPosition = new Vector2(transform.localPosition.x - Slots, transform.localPosition.y);
            }
            if (direction == 3)
            {
                targetPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + Slots);
            }
            if (direction == 4)
            {
                targetPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - Slots);
            }
            if (direction == 5)//������
            {
                targetPosition = new Vector2(transform.localPosition.x - 1, transform.localPosition.y + 1);
            }
            if (direction == 6)//������
            {
                targetPosition = new Vector2(transform.localPosition.x + 1, transform.localPosition.y + 1);
            }

            transform.localPosition = new Vector3(startPosition.x, startPosition.y, transform.localPosition.z);
            moving = true;

            if (!reportedMove)
            {
                GameController.CurrentlyMovingJewels++;
                reportedMove = true;
            }
        }
    }

    public void Move2(int direction, int Slots)
    {

        if (direction != 0)
        {
            startPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);

            if (direction == 1)
            {
                targetPosition = new Vector2(transform.localPosition.x + Slots, transform.localPosition.y);
            }
            if (direction == 2)
            {
                targetPosition = new Vector2(transform.localPosition.x - Slots, transform.localPosition.y);
            }
            if (direction == 3)
            {
                targetPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + Slots);
            }
            if (direction == 4)
            {
                targetPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - Slots);
            }

            transform.localPosition = new Vector3(startPosition.x, startPosition.y, transform.localPosition.z);
            moving = true;

            
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    public void MoveOut(Vector3 x)
    {
        targetPosition = x;
        moving = true;

        if (!reportedMove)
        {
            GameController.CurrentlyMovingJewels++;
            reportedMove = true;
        }
    }

    /// <summary>
    /// ĳ���򵥲��ƶ�
    /// </summary>
    /// <param name="dir"></param>
    public void Move(int dir)
    {
        Move(dir, 1);
    }


    public void Die()
    {
        Dead = true;
        GetComponent<Renderer>().enabled = false;
        if (this.tag == "Rock" || this.tag == "bomb")
        {
            Instantiate(Resources.Load("Prefabs/puzzleUI/Explosion"), new Vector3(transform.position.x + 0.08f, transform.position.y + 0.08f, transform.position.z), transform.rotation);
        }
        else
        {
            Instantiate(Resources.Load("Prefabs/puzzleUI/Explosion2"), new Vector3(transform.position.x + 0.08f, transform.position.y + 0.08f, transform.position.z), transform.rotation);
        }
        Destroy(this.gameObject, 1.0f);
    }
    /// <summary>
    /// ����ʯ��������
    /// </summary>
    void SetTexture()
    {
        if (this.tag == "hong")
        {
            GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.0f, 0.5f);
            GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f);
        } //ok
        if (this.tag == "lan") { GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.123f, 0.5f); GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f); } //ok
        if (this.tag == "lv") { GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.249f, 0.5f); GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f); } //ok
        if (this.tag == "zi") { GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.373f, 0.5f); GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f); } //ok
        if (this.tag == "qing") { GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.498f, 0.5f); GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f); } //ok
        if (this.tag == "bomb") { GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.623f, 0.5f); GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f); } //ok
        if (this.tag == "hp") { GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.749f, 0.5f); GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f); } //ok
        if (this.tag == "xp") { GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0f); GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f); } //ok
        if (this.tag == "Rock") { GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.877f, 0.5f); GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.125f, 0.497f); } //ok
    }


}
