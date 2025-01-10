using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IDragEvent
{
    void DragCorrect();

    void DragInCorrect();

    void DestroyObjectWithTween();
}
public class BarrageItem : MonoBehaviour, IDragEvent
{
   
    private bool m_isMoveRight = true;

    private bool m_isPositive = true;

    private float m_moveSpeed = 1f;


    bool isAlive = true;


    /// <summary>
    /// 初始化显示效果
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="info"></param>
    public void InitView(Sprite sprite,string info, bool isMoveRight,bool isPositive,float moveSpeed)
    {
        GetComponent<Image>().sprite = sprite;
        GetComponentInChildren<Text>().text = info;
        m_isMoveRight = isMoveRight;
        m_isPositive= isPositive;
        this.gameObject.tag = m_isPositive?"PositiveBarrage":"NegativeBarrage";
        m_moveSpeed= moveSpeed;
    }

    private void Update()
    {
        if (GetComponent<RectTransform>().anchoredPosition3D.x<970&& GetComponent<RectTransform>().anchoredPosition3D.x>-970) 
        {
            if (m_isMoveRight)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position +   Vector3.right, m_moveSpeed*0.12f);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position -   Vector3.right, m_moveSpeed*0.12f);
            }
        }
        else
        {
            if (isAlive)
            {
                isAlive = false;
                Destroy(gameObject);

            }
        }
    }

    public void DragCorrect()
    {
        m_moveSpeed = 0;
        this.GetComponentInChildren<Text>().DOFade(0,0.14f);
        this.GetComponent<Image>().DOFillAmount(0, 0.3f).OnComplete(() => 
        {
            Destroy(this.gameObject);
        }); ;
    }

    public void DragInCorrect()
    {
        m_moveSpeed = 0;
        this.GetComponentInChildren<Text>().DOFade(0, 0.1f);
        this.GetComponent<Image>().DOFillAmount(0, 0.3f).OnComplete(() =>
        {
            Destroy(this.gameObject);
        }); ;
    }

    public void DestroyObjectWithTween()
    {
        m_moveSpeed = 0;
        this.GetComponentInChildren<Text>().DOFade(0, 0.14f);
        this.GetComponent<Image>().DOFillAmount(0, 0.3f).OnComplete(() =>
        {
            Destroy(this.gameObject);
        }); ;
    }
}
