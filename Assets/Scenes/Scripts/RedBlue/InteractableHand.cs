using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum DragEndCheck
{
    NoObject,
    RightObject,
    WrongObject
}

public class InteractableHand : MonoBehaviour
{
    public KinectInterop.JointType handType;
    [SerializeField]
    private Sprite OpenSpritre;

    [SerializeField]
    private Sprite CloseSprite;

    private RectTransform m_rectTransform;
    private CircleCollider2D m_CircleCollider = null;

    private InteractableObject dragObject = null;
    private FlowerBasket m_flowerBasket;
    
    private Image m_Cursor;
    bool isDrag = false;

    public Action OnDragReleaseCallback;

    public Action<DragEndCheck> OnDragGetScoreCallback;
    private void Start()
    {
        m_rectTransform=GetComponent<RectTransform>();
        m_CircleCollider =GetComponent<CircleCollider2D>();
        m_Cursor = GetComponent<Image>();
        GameManager.Instance.clapListener.HandStateChangeEvent += OnDragStateChange;
    }

    private void Update()
    {

        if (dragObject!=null)
        {
            if (dragObject.gameObject!=null)
            {
                dragObject?.OnDrag(m_rectTransform.anchoredPosition);
            }
        }

    }
    private void OnDragStateChange(KinectInterop.JointType jointType, KinectInterop.HandState handState)
    {
        if (SceneManager.GetActiveScene().name=="BarrageDragKinect")
        {
            //判断左右手
            if (jointType == handType)
            {
                if (handState == KinectInterop.HandState.Open)
                {
                    //释放
                    m_Cursor.sprite = OpenSpritre;

                }
                else if (handState == KinectInterop.HandState.Closed)
                {

                    m_Cursor.sprite = CloseSprite;
                    RaycastCheck("PositiveBarrage");
                }
            }
        }
        if (SceneManager.GetActiveScene().name == "RedBlueKinect")
        {
            //判断左右手
            if (jointType == handType)
            {
                if (handState == KinectInterop.HandState.Open)
                {
                    //释放
                    //m_flowerBasket = RayCastHitBasket();
                    //if (m_flowerBasket != null)
                    //{
                    //    if (dragObject != null)
                    //    {
                    //        if (m_flowerBasket.GetFlowerEaqul(dragObject))
                    //        {
                    //            m_flowerBasket.AddToBasket(1);
                    //            FindObjectOfType<RedBlueManager>().AddScore(dragObject);
                    //            Destroy(dragObject.gameObject);
                    //        }
                    //        else
                    //        {
                    //            FindObjectOfType<RedBlueManager>().MinusScore(dragObject);
                    //            Destroy(dragObject.gameObject);

                    //        }

                    //    }
                    //}
                    //isDrag = false;
                    //if (dragObject!=null)
                    //{
                    //    if (dragObject.gameObject != null)
                    //    {
                    //        dragObject?.SetRigidBodyState(true);
                    //        dragObject?.SetCurrentJointType(KinectInterop.JointType.None);
                    //    }
                    //}


                    dragObject = null;
                    m_Cursor.sprite = OpenSpritre;
                    //Debug.LogError("释放");

                }
                else if (handState == KinectInterop.HandState.Closed)
                {

                    isDrag = true;
                    RaycastCheck();
                    dragObject?.SetRigidBodyState(false);
                    dragObject?.SetCurrentJointType(handType);
                    //Debug.LogError("抓取");
                    m_Cursor.sprite = CloseSprite;
                }
            }
        }

    }
    

    private bool RaycastCheck(string tag)
    {
        RaycastHit2D hit = Physics2D.Raycast(m_rectTransform.position, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag(tag))
            {
                OnDragGetScoreCallback?.Invoke(DragEndCheck.RightObject);
                hit.collider.GetComponent<IDragEvent>().DragCorrect();
                return true;
            }
            else
            {
                OnDragGetScoreCallback?.Invoke(DragEndCheck.WrongObject);
                hit.collider.GetComponent<IDragEvent>().DragInCorrect();
                return false;
            }
        }
        OnDragGetScoreCallback?.Invoke(DragEndCheck.NoObject);
        return false;
    }

    private bool RaycastCheck()
    {

        RaycastHit2D hit = Physics2D.Raycast(m_rectTransform.position, Vector2.zero);
        if (hit.collider!=null)
        {
            dragObject = hit.collider.GetComponent<InteractableObject>();
            if (dragObject != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }

    private FlowerBasket RayCastHitBasket()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(m_rectTransform.position, Vector2.zero);
        if (hit.Length > 0) 
        {
            foreach (var item in hit)
            {
                if (item.transform.CompareTag("Basket"))
                {
                    FlowerBasket flowerBasket = item.transform.GetComponent<FlowerBasket>();
                    return flowerBasket;
                }
            }
        }
        return null;
    }

}
