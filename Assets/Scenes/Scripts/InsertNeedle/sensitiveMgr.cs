using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class sensitiveMgr : MonoBehaviour
{
    public Slider sensitiveSlider;

    public Text sensitiveText;

    public HandGestureDetection handGesture;

    private CanvasGroup group;
    private void Awake()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath,"sensitive.txt");
        StartCoroutine(Utills.ReadLocalTxt(filePath, (str) => 
        {
            handGesture.distanceThreshold = float.Parse(str); 
            sensitiveSlider.value = handGesture.distanceThreshold *10f;
            sensitiveText.text = (handGesture.distanceThreshold * 100).ToString() + "厘米";
            Debug.Log("read succeed"); 
        }, 
        (errorcontent) => 
        {
            sensitiveText.text = errorcontent; 
        }));
        group=GetComponent<CanvasGroup>();
        sensitiveSlider.onValueChanged.AddListener((flo)=> { handGesture.distanceThreshold = (float)Math.Round(flo * 0.1f,2) ; sensitiveText.text = (handGesture.distanceThreshold*100).ToString()+"厘米"; } );

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            group.alpha = group.alpha == 1 ? 0 : 1;
        }
    }

}
