using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CircleControl : MonoBehaviour
{
    private Image m_Circle;

    bool _Rotate = false;

    [SerializeField]
    private float _RotateSpeed= 1.0f;

    [SerializeField]
    private Color GetScoreColor;
    private List<SpawItem> CurrentOwner = new List<SpawItem>();

    public InsertNeedleManager InsertNeedleManager;
    private void Start()
    {
        m_Circle = GetComponent<Image>();
        GameManager.Instance.GamePauseEvent += GameStateChangedEvent;


    }
    private void FixedUpdate()
    {
        RotateSelf(m_Circle.transform);
        if (_Rotate && InsertNeedleManager != null && InsertNeedleManager.GetConfirmReadIntro())
        {
            RotateSelf(m_Circle.transform);
        }
    }

    /// <summary>
    /// 当前主圆圈的出现动画
    /// </summary>
    public void ShowCircleAnimation()
    {
        m_Circle = GetComponent<Image>();
        m_Circle.DOFillAmount(1, 1f);
    }

    /// <summary>
    /// 得分后主圆圈颜色变化
    /// </summary>
    public void GetScoreCircleAnimation()
    {
        m_Circle.transform.DOScale(Vector3.one * 1.1f, 0.2f).OnComplete(() => 
        {
            m_Circle.transform.DOScale(Vector3.one,0.2f);
        });
        m_Circle.DOColor(GetScoreColor, 0.2f).OnComplete(() => 
        {
            m_Circle.DOColor(Color.white,0.2f);
        }); ;
    }
    private void RotateSelf(Transform _transform)
    {
        // 将旋转量转换为弧度,每帧旋转的角度，默认为1
        float rotationRadians = _RotateSpeed * Mathf.Deg2Rad;

        // 创建每帧旋转的四元数
        Quaternion rotation = Quaternion.Euler(0, 0, rotationRadians);

        // 更新物体的旋转
        _transform.rotation *= rotation;
    }


    private void StartRoate()
    {

    }
    private void PauseCircleRotate(bool _isPause)
    {
        _Rotate = !_isPause;
    }

    /// <summary>
    /// 插针
    /// </summary>
    /// <param name="spawItem">插入的对象</param>
    public void InsertNeedle(SpawItem spawItem)
    {
        CurrentOwner.Add(spawItem);
    }

    /// <summary>
    /// 删除针
    /// </summary>
    private void ClearNeedle(bool isGameEnd)
    {
        if (isGameEnd)
        {
            return;
        }
        foreach (var item in CurrentOwner)
        {
            Destroy(item.gameObject);
        }
        CurrentOwner.Clear();
    }


    /// <summary>
    /// 根据游戏改变切换UI状态
    /// </summary>
    /// <param name="isGameEnd">游戏是否结束，为true时为结束</param>
    private void GameStateChangedEvent(bool isGameEnd)
    {
        PauseCircleRotate(isGameEnd);

        //重新开始时，刷新
        if (!isGameEnd)
        {
            ClearNeedle(isGameEnd);
            this.transform.rotation = Quaternion.identity;
        }
    }
}
