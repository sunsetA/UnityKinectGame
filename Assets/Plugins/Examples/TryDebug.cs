using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*转为UTF-8*/
public class TryDebug : MonoBehaviour
{
    public int m_data = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    public void SubmitData(int data) 
    {

        Debug.LogFormat("Origin data is {0} ,new data is {1}",m_data,data);
        m_data = data;
    }
}
