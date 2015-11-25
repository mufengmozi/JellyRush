/* Copyright (c) Vander Amaral
 * I've created this code based on a lot of Match3 Codes I found on the Internet
 * I tried to make it the best and easy to change.
 * You can polish it even more, and add functions to it if you bought.
 */

using UnityEngine;
using System.Collections;
using System;

public class obstracle : MonoBehaviour
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
    /// 障碍类型枚举
    /// </summary>
    public enum obstype
    {
        bing = 0,
        huo = 1,
        feng = 2,
        lei = 3,
    };

    // 初始化宝石
    void Start()
    {
        //SetTexture();//设置纹理
        Dead = false;
        speed = 8;
        position = new Vector2(transform.localPosition.x, transform.localPosition.y);
    }

    /// <summary>
    /// 获取障碍类型
    /// </summary>
    /// <param name="tag">tag名称</param>
    /// <returns>返回类型的值</returns>
    public int gettype(string tag)
    {
        int type = 0;
        Array ary = Enum.GetValues(typeof(obstype));
        for (int i = 0; i < ary.Length; i++)
        {
            if (ary.GetValue(i).ToString() == tag)
            {
                type = i;
            }
        }
        return type;
    }

    // Update is called once per frame
    void LateUpdate()
    {

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
    /// 给障碍设置纹理
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
