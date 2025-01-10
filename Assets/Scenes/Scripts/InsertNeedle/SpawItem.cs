using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class SpawItem : MonoBehaviour
{
    public enum ItemType
    {
        Partly,
        Single,
    }
    public ItemType type = ItemType.Single;
    private Transform m_item;

    private CircleControl m_circleControl;
    private Transform m_TargetPoint;

    float m_currentDis;
    private BoxCollider2D m_boxcollider;
    private Rigidbody2D m_rigidBody;

    bool m_isReached = false;

    private int Score = 1;
    public Action<int> GetScoreAction;
    private void Start()
    {
        m_item = GetComponent<Transform>();
        m_boxcollider = GetComponent<BoxCollider2D>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_circleControl = FindObjectOfType<CircleControl>();
        if (m_circleControl == null)
        {
            Debug.LogError("Circle controller is null!");
        }
        m_TargetPoint = m_circleControl?.transform;
        m_circleControl.InsertNeedle(this);

        GetScoreAction += (value) => 
        {
            m_circleControl.GetScoreCircleAnimation();
        };
    }

    float checkDis = 0;
    private void Update()
    {
        if (!m_isReached)
        {
            m_currentDis = Vector3.Distance(m_item.position, m_TargetPoint.position);
            checkDis = m_TargetPoint.GetComponent<RectTransform>().rect.height / 2 + 0.03f;
            if (m_currentDis < checkDis) 
            {
                m_isReached = true;
                m_boxcollider.isTrigger = false;
                m_item.transform.SetParent(m_TargetPoint.transform);
                type = ItemType.Partly;
                GetScoreAction?.Invoke(Score);

                Vector3 direction = (m_TargetPoint.position - transform.position).normalized;
                transform.position = m_TargetPoint.position - direction * checkDis;

            }
            else
            {
                m_item.position = Vector3.Lerp(m_item.position, m_TargetPoint.position,Time.deltaTime*10);
            }
        }
    }
    public void InitItem(CircleControl ItemFactory)
    {
        m_circleControl = ItemFactory;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.GetRunState())
        {
            SpawItem collisionItem = collision.GetComponent<SpawItem>();
            if (collisionItem != null)
            {
                if (collisionItem.type == ItemType.Partly)
                {
                    m_isReached = true;
                    //GameManager.Instance.GamePauseEvent?.Invoke(true);
                    GameManager.Instance.SetGameState(false);
                }
            }
        }


    }
}
