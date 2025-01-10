using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class Utills 
{
    /// <summary>
    /// 获取本地某个路径下的txt文本文档   
    /// *NOTE 需要包含文件拓展名
    /// </summary>
    /// <param name="filePath">txt文本的路径</param>
    /// <returns></returns>
    public static IEnumerator ReadLocalTxt(string filePath,Action<string> SucceedCallback,Action<string> failedCallback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();

            if (string.IsNullOrEmpty(request.error))
            {
                string textContent = request.downloadHandler.text;
                Debug.Log(textContent);
                SucceedCallback?.Invoke(textContent);
            }
            else
            {
                Debug.LogError(request.error);
                failedCallback?.Invoke(request.error);
            }
        }
    }

    /// <summary>
    /// 写入浮点型数据到本地某个路径下的txt文本文档
    /// *NOTE 需要包含文件拓展名
    /// </summary>
    /// <param name="filePath">txt文本的路径</param>
    /// <param name="value">要写入的浮点型数据</param>
    /// <returns></returns>
    public static IEnumerator WriteLocalTxt(string filePath, float value, Action<string> SucceedCallback, Action<string> failedCallback)
    {
        string content = value.ToString();
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(content);

        using (UnityWebRequest request = UnityWebRequest.Put(filePath, bodyRaw))
        {
            yield return request.SendWebRequest();

            if (string.IsNullOrEmpty(request.error))
            {
                SucceedCallback?.Invoke(content);
                Debug.Log("edit file succeed!");
            }
            else
            {

                failedCallback?.Invoke(request.error);
                Debug.LogError(request.error);
            }
        }
    }

}
