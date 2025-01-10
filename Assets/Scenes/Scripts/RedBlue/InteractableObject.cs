using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;


public class InteractableObject : MonoBehaviour
{
    public FlowerColor colorType = FlowerColor.None;
    /// <summary>
    /// 当前被哪个关节捕获
    /// </summary>
    private KinectInterop.JointType currentGrabJointType;

    private RectTransform m_rectTransform;
    private CircleCollider2D m_circleCollider;
    private Rigidbody2D m_rigidbody;
    private Image m_image;

    bool isDraged;
    
    private void InitSetting()
    {
        m_rectTransform=GetComponent<RectTransform>();
        m_circleCollider = GetComponent<CircleCollider2D>();
        m_rigidbody= GetComponent<Rigidbody2D>();
        m_image = GetComponent<Image>();
    }

    /// <summary>
    /// 初始化颜色
    /// </summary>
    /// <param name="_colorType">定义枚举类型</param>
    /// <param name="_spriteChange">定义贴图替换</param>
    public void InitFlower(FlowerColor _colorType,Sprite _spriteChange)
    {
        InitSetting();
        colorType = _colorType;
        m_image.sprite = _spriteChange;
    }

    public void OnDrag(Vector3 TargetPos)
    {
        m_rectTransform.anchoredPosition= TargetPos;
    }

    public Action<bool> SetRigidBody;
    public void SetRigidBodyState(bool isEnable)
    {
        isDraged = !isEnable;
        //m_rigidbody.simulated = isEnable;
        m_rigidbody.gravityScale = isEnable?1:0;
    }

    /// <summary>
    /// 获取当前对象的抓取状态
    /// </summary>
    /// <returns>返回为true，表示正在处于抓取状态</returns>
    public bool GetDragState()
    {
        return isDraged;
    }

    /// <summary>
    /// 获取当前的花瓣类型
    /// </summary>
    /// <returns></returns>
    public FlowerColor GetFlowerType()
    {
        return colorType;
    }

    public KinectInterop.JointType GetCurrentJointType()
    {
        return currentGrabJointType;
    }

    public void SetCurrentJointType(KinectInterop.JointType jointType)
    {
        currentGrabJointType = jointType;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Border"))
        {
            Destroy(this.gameObject);
        }

    }

}
