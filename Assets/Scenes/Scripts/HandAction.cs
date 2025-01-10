using UnityEngine;

public class HandAction : MonoBehaviour, KinectGestures.GestureListenerInterface
{

    public int playerIndex = 0;
    //动作取消时调用
    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
    {
        return true;
    }
    //动作完成时调用
    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
    {
        if (userIndex != playerIndex)
            return false;
        if (gesture == KinectGestures.Gestures.Psi)//判断如果完成的姿势是双手向上的姿势
        {
            print("Psi Completed...");
        }
        return true;
    }
    //动作进行时调用
    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
    }
    //检测到用户时调用
    public void UserDetected(long userId, int userIndex)
    {
        if (userIndex != playerIndex)
            return;
        print("检测到用户");
        KinectManager manager = KinectManager.Instance;//初始化KinectManager对象
        manager.DetectGesture(userId, KinectGestures.Gestures.Psi);//添加双手向上保持1秒的姿势检测

    }
    //丢失用户时调用
    public void UserLost(long userId, int userIndex)
    {
        print("丢失用户");
    }
    // Use this for initialization
    void Start()
    {

    }
    void Update()
    {

    }
}

