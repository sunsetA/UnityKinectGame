using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaguManager : MonoBehaviour,IGameLogic
{
    /// <summary>
    /// 是否已经阅读了训练须知
    /// </summary>
    private bool isReadREADME;

   


    private void Awake()
    {

    }


    public void SetRead()
    {
        isReadREADME = true;
    }

    public void StatisticData()
    {

    }

    public void ResetData()
    {

    }

    public void OnGameLogicChange(bool isPause)
    {
        if (!isReadREADME)
        {
            //如果用户没有
            return;
        }
        if (isPause) 
        {
            StatisticData();
        }
        else
        {
            ResetData();
        }
    }
}
