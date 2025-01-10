using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

//using UnityEngine.XR.WSA.Input;
using Image = UnityEngine.UI.Image;

public class HandGestureDetection : MonoBehaviour
{
    InteractionManager interactionManager;
    KinectManager kinectManager;
    public float distanceThreshold = 0.1f;

    bool isHandsClapped = false;


    Vector3 rightHandJointKinectPos;
    Vector3 leftHandJointKinectPos;
    float distance;
    long userId;

    public Action ClapCallback;
    WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.05f);

    public bool BothHand = true;
    public Image LeftCursor;
    public Image RightCursor;

    Vector3 leftHandScreenPos;
    Vector3 rightHandScreenPos;

    Vector3 leftHandJointPos;
    Vector3 rightHandJointPos;
    private Vector3 LeftCursorPos;
    private Vector3 RightCursorPos;

    //private Vector3 leftHandScreenPixelPos = Vector3.zero;
    //private Vector3 rightHandScreenPixelPos = Vector3.zero;
    //左手状态
    KinectInterop.HandState _leftHandState = KinectInterop.HandState.Unknown;
    //右手状态
    KinectInterop.HandState _rightHandState = KinectInterop.HandState.Unknown;

    /// <summary>
    /// 动作状态改变事件
    /// </summary>
    public Action<KinectInterop.JointType, KinectInterop.HandState> HandStateChangeEvent;

    public bool isLerp = false;
    private void Start()
    {
        interactionManager = FindObjectOfType<InteractionManager>();
        kinectManager = FindObjectOfType<KinectManager>();
        StartCoroutine(CheckGesture());
        if (!BothHand)
        {
            RightCursor.enabled = false;
        }
        StartCoroutine(Utills.ReadLocalTxt(Application.streamingAssetsPath + "/HandGesture.txt", (str)=>
        {
            if (str == "lerp") 
            {
                isLerp = true;
            }
            else
            {
                isLerp = false;
            }
        },null)); ;
    }
    IEnumerator CheckGesture()
    {
        while (true) 
        {
            yield return wait;
            if (KinectManager.Instance && KinectManager.Instance.IsInitialized())
            {
                if (KinectManager.Instance.IsUserDetected())
                {
                    CheckClap();
                }
            }
        }
    }
    private void Update()
    {
        ScreenSetting();
        CheckFist();
        return;
        //// 获取手指关节的状态
        //bool thumbOpen = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.ThumbRight).y > KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandRight).y;
        //bool indexOpen = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.IndexTipRight).y > KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandRight).y;
        //bool middleOpen = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.MiddleTipRight).y > KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandRight).y;
        //bool ringOpen = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.RingTipRight).y > KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandRight).y;
        //bool pinkyOpen = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.PinkyTipRight).y > KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandRight).y;

        //bool isRightHandClosed = thumbOpen && indexOpen && middleOpen && ringOpen && pinkyOpen;
        //if (isRightHandClosed)
        //{
        //    Debug.LogError("fist state");
        //}
        //获取左手姿势

        KinectInterop.HandState _leftHandState = KinectManager.Instance.GetLeftHandState(userId);
        KinectInterop.HandState _rightHandState = KinectManager.Instance.GetRightHandState(userId);
        if (_leftHandState == KinectInterop.HandState.Closed)
        {

            Debug.LogError("检测到左手握拳了");

        }
        if (_rightHandState == KinectInterop.HandState.Closed)
        {
            Debug.LogError("检测到右手握拳了");

        }
    }


    /// <summary>
    /// 检查是否击掌
    /// </summary>
    private void CheckClap()
    {
        userId = KinectManager.Instance.GetPrimaryUserID();
        rightHandJointKinectPos = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandRight);
        leftHandJointKinectPos = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandLeft);

        distance = Vector3.Distance(rightHandJointKinectPos, leftHandJointKinectPos);
        if (distance < distanceThreshold && !isHandsClapped)
        {
            isHandsClapped = true;
            ClapCallback?.Invoke();
        }
        if (distance > (distanceThreshold+0.1f) && isHandsClapped)
        {
            isHandsClapped = false;
        }
    } 

    private void CheckFist()
    {
        //判断当前记录的上一个左手状态和当前左手状态，如果不一致，调用变化事件
        if (_leftHandState!= KinectManager.Instance.GetLeftHandState(userId))
        {
            if (KinectManager.Instance.GetLeftHandState(userId)==KinectInterop.HandState.Open||
                KinectManager.Instance.GetLeftHandState(userId) == KinectInterop.HandState.Closed) 
            {
                //TODO改变
                _leftHandState = KinectManager.Instance.GetLeftHandState(userId);

                HandStateChangeEvent?.Invoke(KinectInterop.JointType.HandLeft, _leftHandState);
            }
        }
        if (_rightHandState != KinectManager.Instance.GetRightHandState(userId))
        {
            //TODO改变
            if (KinectManager.Instance.GetRightHandState(userId) == KinectInterop.HandState.Open ||
                KinectManager.Instance.GetRightHandState(userId) == KinectInterop.HandState.Closed)
            {
                _rightHandState = KinectManager.Instance.GetRightHandState(userId);
                HandStateChangeEvent?.Invoke(KinectInterop.JointType.HandRight, _rightHandState);
            }
            //else if (KinectManager.Instance.GetRightHandState(userId) == KinectInterop.HandState.Unknown)
            //{
            //    HandStateChangeEvent?.Invoke(KinectInterop.JointType.HandRight, KinectInterop.HandState.Closed);
            //}

        }


    }


    /// <summary>
    /// 获取UI位置
    /// </summary>
    /// <param name="isLeft"></param>
    /// <returns></returns>
    public Vector3 GetCursorPosition(bool isLeft)
    {
        return isLeft ? leftHandJointKinectPos : rightHandJointKinectPos;
    }


    // 存储关节历史数据，用于平滑
    private readonly Queue<Vector3> LefthandData = new Queue<Vector3>();

    private readonly Queue<Vector3> RighthandData = new Queue<Vector3>();

    /// <summary>
    /// 设置空间坐标
    /// </summary>
    private void ScreenSetting()
    {
        if (interactionManager!=null)
        {
            leftHandScreenPos = interactionManager.GetLeftHandScreenPos();
            rightHandScreenPos = interactionManager.GetRightHandScreenPos();
            //leftHandScreenPixelPos.x = (int)(leftHandScreenPos.x * (Camera.main ? Camera.main.pixelWidth : Screen.width));
            //leftHandScreenPixelPos.y = (int)(leftHandScreenPos.y * (Camera.main ? Camera.main.pixelHeight : Screen.height));
            //rightHandScreenPixelPos.x = (int)(rightHandScreenPos.x * (Camera.main ? Camera.main.pixelWidth : Screen.width));
            //rightHandScreenPixelPos.y = (int)(rightHandScreenPos.y * (Camera.main ? Camera.main.pixelWidth : Screen.width));



            Rect rectCanvas = LeftCursor.canvas.pixelRect;
            float xoffset = (rectCanvas.width) / 2;
            float yoffset = rectCanvas.height / 2;
            float multiple = 1.26f;
            Vector3 posSprite = new Vector2(Mathf.Clamp(multiple * (leftHandScreenPos.x * rectCanvas.width - xoffset) ,- xoffset,xoffset) , Mathf.Clamp(multiple*(leftHandScreenPos.y * rectCanvas.height - yoffset),-yoffset,yoffset) );

            Vector3 posSprite1 = new Vector2(Mathf.Clamp(multiple * (rightHandScreenPos.x * rectCanvas.width - xoffset), -xoffset, xoffset),Mathf.Clamp(multiple*(rightHandScreenPos.y * rectCanvas.height - yoffset),-yoffset,yoffset) );

            //LeftCursor.transform.GetComponent<RectTransform>().anchoredPosition = posSprite;
            //RightCursor.transform.GetComponent<RectTransform>().anchoredPosition3D = posSprite1;
            if (!isLerp)
            {
                if (LefthandData.Count < 20)
                {
                    LefthandData.Enqueue(posSprite);
                }
                else
                {
                    LefthandData.Dequeue();
                    LefthandData.Enqueue(posSprite);
                }
                if (RighthandData.Count < 20)
                {
                    RighthandData.Enqueue(posSprite1);
                }
                else
                {
                    RighthandData.Dequeue();
                    RighthandData.Enqueue(posSprite1);
                }

                LeftCursor.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(LefthandData.Average(item => item.x), LefthandData.Average(item => item.y), LefthandData.Average(item => item.z));
                if (BothHand)
                {
                    RightCursor.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(RighthandData.Average(item => item.x), RighthandData.Average(item => item.y), RighthandData.Average(item => item.z));

                }
            }
            else
            {
                LeftCursor.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(LeftCursor.transform.GetComponent<RectTransform>().anchoredPosition, posSprite, Time.deltaTime * 10);
                if (BothHand)
                {
                    RightCursor.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(RightCursor.transform.GetComponent<RectTransform>().anchoredPosition, posSprite1, Time.deltaTime * 10); ;

                }
            }



        }

    }
}