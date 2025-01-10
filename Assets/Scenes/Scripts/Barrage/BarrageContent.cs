using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageContent : MonoBehaviour
{
    //private List<BarrageItem> barrageItems = new List<BarrageItem>();

    [SerializeField]
    private bool m_isTurnRight = true;

    private void Awake()
    {
        //m_isTurnRight = Random.Range(1, 3) > 1 ? true : false;
        FindObjectOfType<BarrageManager>().InitTurnLightAndLeftLists(this);
    }

    public bool GetTurnRight()
    {
        return m_isTurnRight;
    }
}
