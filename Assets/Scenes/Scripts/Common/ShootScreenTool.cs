using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShootScreenTool : MonoBehaviour
{

    private string path = @"C:\Users\Admin\Desktop\Test";

    private Rect rt;

    public RectTransform Area;
    public Image ShowImage;
    
    private void Start()
    {
        CheckPath();
        //GameObject.Find("111").GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    private void CheckPath()
    {
        bool isExists = Directory.Exists(path);
        if (!isExists)
        {
            Directory.CreateDirectory(path);
            Debug.Log("不存在当前路径,创建文件夹");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetReportImage(() => { },
            () =>
            {
                GameObject.Find("111").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                Debug.Log(GameObject.Find("111").GetComponent<Image>().color.a);
                
            }, () =>
            {
                GameObject.Find("111").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                Debug.Log(GameObject.Find("111").GetComponent<Image>().color.a);
            });
        }


    }

    public void SetReportImage(Action ScreenShotCallback,Action RenderAction1,Action RenderAction2)
    {
        rt = RectTransformToScreenSpace(Area);
        StartCoroutine(ShootScreen(rt, ScreenShotCallback,RenderAction1,RenderAction2));

    }
    private IEnumerator ShootScreen(Rect rt, Action callback,Action callback1,Action callback2)
    {
        callback1?.Invoke();
        yield return new WaitForEndOfFrame();

        Camera.main.Render();
        int width = (int)rt.width;
        int height = (int)rt.height;
        Texture2D tt = new Texture2D(width, height);
        tt.ReadPixels(rt, 0, 0);
        tt.Apply();

        ShowImage.sprite = GetSprite(tt);
        Debug.Log("截图成功");
        yield return new WaitForEndOfFrame();
        callback?.Invoke();
        callback2?.Invoke();
        //byte[] data_PNG = tt.EncodeToPNG();//还有其它的格式：tt.EncodeToJPG(); tt.EncodeToTGA(); tt.EncodeToEXR();

        //string outPutPath = string.Concat(path, @"\" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm") + "_ShootScreen.PNG");
        //File.WriteAllBytes(outPutPath, data_PNG);
        //Debug.Log("截图成功");
    }


    private Sprite GetSprite(Texture2D screenshotTexture)
    {
        Sprite screenshotSprite = Sprite.Create(screenshotTexture, new Rect(0, 0, screenshotTexture.width, screenshotTexture.height), Vector2.one * 0.5f);

        return screenshotSprite;
    }

    private Rect RectTransformToScreenSpace(RectTransform RectTrans)
    {
        Vector2 size = Vector2.Scale(RectTrans.rect.size, RectTrans.lossyScale);
        Rect rect = new Rect(RectTrans.position.x, RectTrans.position.y, size.x, size.y);
        rect.x -= (RectTrans.pivot.x * size.x);
        rect.y -= ((1.0f - RectTrans.pivot.y) * size.y);
        return rect;
    }
}