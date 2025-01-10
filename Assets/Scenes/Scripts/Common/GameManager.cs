using System;
using System.Collections;
using System.Threading;
using UnityEngine;


public interface IGameLogic
{
    /// <summary>
    /// 统计数据
    /// </summary>
    void StatisticData();

    /// <summary>
    /// 刷新数据
    /// </summary>
    void ResetData();

    /// <summary>
    /// 游戏逻辑改变
    /// </summary>
    /// <param name="isPause"></param>
    void OnGameLogicChange(bool isPause);
}
public class GameManager : MonoSingleton<GameManager>, KinectGestures.GestureListenerInterface
{
    private bool isRun = false;
    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("UI-Text to display gesture-listener messages and gesture information.")]
    public UnityEngine.UI.Text gestureInfo;

    // private bool to track if progress message has been displayed
    private bool progressDisplayed;
    private float progressGestureTime;

    public event Action<KinectGestures.Gestures> GesturesEvent;
    public Action<bool> GamePauseEvent;
    public Action<bool> GameLogicRun;
    [HideInInspector]
    public HandGestureDetection clapListener;

    public bool GetRunState() => isRun;


    protected override  void Awake()
    {
        base.Awake();
        clapListener = FindObjectOfType<HandGestureDetection>();
        InitEvent();
    }
    private void InitEvent()
    {
        GesturesEvent += RaiseLeftAndRightGesturesEvent;
        GesturesEvent += RaiseLeftHandGesturesEvent;
        GesturesEvent += ClapGestureEvent;
        GamePauseEvent += (ispause) => 
        {
            if (ispause) 
            {
                LogGameMessage();
            }
        };
    }

    private void ClapGestureEvent(KinectGestures.Gestures gestures)
    {
        if (gestures == KinectGestures.Gestures.ClapHands)
        {
            Debug.Log("Clap Succeed!");
        }
    }


    private void RaiseLeftHandGesturesEvent(KinectGestures.Gestures gestures)
    {
        //Start game
        if (gestures == KinectGestures.Gestures.RaiseLeftHand) 
        {
            SetGameState(true);
            Debug.Log("Start Game");
        }

    }

    private void RaiseLeftAndRightGesturesEvent(KinectGestures.Gestures gestures)
    {
        //Stop Game
        if (gestures == KinectGestures.Gestures.RaiseLeftRightHand)
        {
            Application.Quit();
        }

    }

    public void UserDetected(long userId, int userIndex)
    {
        if (userIndex != playerIndex)
            return;

        // as an example - detect these user specific gestures
        KinectManager manager = KinectManager.Instance;
        //manager.DetectGesture(userId, KinectGestures.Gestures.SwipeLeft);
        //manager.DetectGesture(userId, KinectGestures.Gestures.SwipeRight);
        manager.DetectGesture(userId, KinectGestures.Gestures.RaiseLeftHand);
        manager.DetectGesture(userId, KinectGestures.Gestures.RaiseLeftRightHand);
        manager.DetectGesture(userId, KinectGestures.Gestures.ClapHands);


    }

    public void UserLost(long userId, int userIndex)
    {
        if (userIndex != playerIndex)
            return;

        if (gestureInfo != null)
        {
            gestureInfo.text = string.Empty;
        }
    }

    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
        if (userIndex != playerIndex)
            return;

        if ((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
        {
            if (gestureInfo != null)
            {
                string sGestureText = string.Format("{0} - {1:F0}%", gesture, screenPos.z * 100f);
                gestureInfo.text = sGestureText;

                progressDisplayed = true;
                progressGestureTime = Time.realtimeSinceStartup;
            }
        }
        else if ((gesture == KinectGestures.Gestures.Wheel || gesture == KinectGestures.Gestures.LeanLeft || gesture == KinectGestures.Gestures.LeanRight ||
            gesture == KinectGestures.Gestures.LeanForward || gesture == KinectGestures.Gestures.LeanBack) && progress > 0.5f)
        {
            if (gestureInfo != null)
            {
                string sGestureText = string.Format("{0} - {1:F0} degrees", gesture, screenPos.z);
                gestureInfo.text = sGestureText;

                progressDisplayed = true;
                progressGestureTime = Time.realtimeSinceStartup;
            }
        }
        else if (gesture == KinectGestures.Gestures.Run && progress > 0.5f)
        {
            if (gestureInfo != null)
            {
                string sGestureText = string.Format("{0} - progress: {1:F0}%", gesture, progress * 100);
                gestureInfo.text = sGestureText;

                progressDisplayed = true;
                progressGestureTime = Time.realtimeSinceStartup;
            }
        }
    }

    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint, Vector3 screenPos)
    {
        if (userIndex != playerIndex)
            return false;
        
        if (progressDisplayed)
        {
            GesturesEvent?.Invoke(gesture);
            return true;
        }


        GesturesEvent?.Invoke(gesture);
        return true;
    }

    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint)
    {
        if (userIndex != playerIndex)
            return false;

        if (progressDisplayed)
        {
            progressDisplayed = false;
        }

        return true;
    }

    public void Update()
    {
        if (progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
        {
            progressDisplayed = false;
        }
    }

    /// <summary>
    /// 设置游戏状态
    /// </summary>
    /// <param name="_isRun">是否为持续运行状态</param>
    public void SetGameState(bool _isRun)
    {
        isRun= _isRun;
        GamePauseEvent?.Invoke(!_isRun);
    }

    /// <summary>
    /// 游戏结束打印信息
    /// </summary>
    private void LogGameMessage()
    {
        Debug.LogError("任务失败，游戏结束");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}