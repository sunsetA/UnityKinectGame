using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public enum FlowerColor
{
    None,
    Blue,
    Red,
    Yellow

}
public class RedBlueManager : MonoBehaviour
{
    /// <summary>
    /// 花瓣预制体
    /// </summary>
    [SerializeField]
    private GameObject flowerPrefab;




    /// <summary>
    /// 随即实例花瓣的点位
    /// </summary>
    [SerializeField]
    public List<Transform> SpawnPool;

    [SerializeField]
    private Sprite BlueFlowerSprite;
    [SerializeField]
    private Sprite RedFlowerSprite;
    [SerializeField]
    private Sprite YellowFlowerSprite;

    private int SucceededScore;
    private int FailedScore;
    private int TimeDuration = 100;
    private int SpawnItemCount = 0;

    public bool isSpawn = false;
    [SerializeField]
    private TextMeshProUGUI GetScoreText;
    [SerializeField]
    private TextMeshProUGUI MinusScoreText;
    [SerializeField]
    private TextMeshProUGUI TimeText;

    [SerializeField]
    private GameObject TrainingReport;
    [SerializeField]
    private int TotalTime = 100;
    [SerializeField]
    private Image CountDownTween1;
    [SerializeField]
    private Image CountDownTween2;
    [SerializeField]
    private Image CountDownTween3;

    [SerializeField]
    private GameObject Tips;

    WaitForSeconds waitForSeconds = new WaitForSeconds(1);


    public static bool isReadIntroduce = false;

    private bool isPlaying = false;

    List<InteractableObject> interactableObjects1;

    public Transform ComparedBorderHeight;
    private void Start()
    {

        //GameManager.Instance.clapListener.HandStateChangeEvent += OnDragStateEvent;
        GameManager.Instance.GamePauseEvent += this.GamePauseEvent;
        StartSpawnItems();
    }
    public void SetRead()
    {
        isReadIntroduce = true;
    }

    private void GamePauseEvent(bool ispause)
    {
        if (isReadIntroduce)
        {
            Tips.SetActive(ispause);
            if (ispause)
            {
                isSpawn = false;
                //出现 Report UI
                TrainingReport.SetActive(true);
                //成功个数
                TrainingReport.transform.Find("Score/SucceedNumberValue/Text").GetComponent<Text>().text = SucceededScore.ToString() + "个";
                //遗漏个数
                TrainingReport.transform.Find("Score/ForgetNumber1Value/Text").GetComponent<Text>().text = (SpawnItemCount - SucceededScore - FailedScore).ToString() + "个";
                //使用时间
                TrainingReport.transform.Find("Score/UseTimeValue/Text").GetComponent<Text>().text = TimeDuration.ToString() + "秒";
                //正确率
                TrainingReport.transform.Find("Score/CoreectPercentValue/Text").GetComponent<Text>().text = (100 * (float)SucceededScore / (float)SpawnItemCount).ToString("F2") + "%";
                Tips.GetComponentInChildren<Text>().text = "举起左手重新开始游戏";
            }
            else
            {
                //ResetData();
                if (!isSpawn)
                {
                    ResetData();
                    StartCoroutine(ShowCountDownTween(() => 
                    {
                        isSpawn = true;
                    }));
                }



                //ResetData();
                //清空花瓣
            }
        }
        else
        {

        }

    }

    private void ResetData()
    {

        TrainingReport.SetActive(false);
        SucceededScore = 0;
        GetScoreText.text = SucceededScore.ToString();
        FailedScore = 0;
        MinusScoreText.text = FailedScore.ToString();
        TimeDuration = 0;
        TimeText.text = TimeDuration.ToString();
        SpawnItemCount = 0;
        //StopAllCoroutines();

    }
    /// <summary>
    /// 开始生成花朵
    /// </summary>
    public void StartSpawnItems()
    {
        foreach (Transform t in SpawnPool)
        {
            StartCoroutine(RandomSpawn(t));
        }
        StartCoroutine(GameCalculate());
    }


    private IEnumerator ShowCountDownTween(Action callback)
    {
        CountDownTween1.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        CountDownTween1.gameObject.SetActive(false);
        callback?.Invoke();


    }
    /// <summary>
    /// 随机点位实例化花瓣
    /// </summary>
    private IEnumerator RandomSpawn(Transform parent)
    {
        while (true) 
        {
            yield return null;
            if (isSpawn)
            {
                if (SpawnItemCount==0)
                {
                    InteractableObject item1 = Instantiate(flowerPrefab, parent).GetComponent<InteractableObject>();
                    item1.transform.localPosition = Vector3.zero;
                    item1.transform.SetParent(parent.parent);
                    FlowerColor randomType1 = (FlowerColor)Random.Range(1, 4);
                    item1.InitFlower(randomType1, GetFlowerSprite(randomType1));
                    SpawnItemCount++;
                }

                yield return new WaitForSeconds(Random.Range(12f, 18f));
                if (isSpawn)
                {
                    InteractableObject item = Instantiate(flowerPrefab, parent).GetComponent<InteractableObject>();
                    item.transform.localPosition = Vector3.zero;
                    item.transform.SetParent(parent.parent);
                    FlowerColor randomType = (FlowerColor)Random.Range(1, 4);
                    item.InitFlower(randomType, GetFlowerSprite(randomType));
                    SpawnItemCount++;
                }

            }
            else
            {
                interactableObjects1 = FindObjectsOfType<InteractableObject>().ToList();
                if (interactableObjects1.Count>0)
                {
                    foreach (var item in interactableObjects1)
                    {
                        Destroy(item.gameObject);
                    }
                }

            }

        }
    }
    IEnumerator GameCalculate()
    {
        while (true)
        {
            yield return null;
            if (isSpawn)
            {
                if (TimeDuration <TotalTime )
                {
                    yield return waitForSeconds;
                    TimeDuration += 1;
                    TimeText.text = (TotalTime-TimeDuration).ToString() + "秒";
                }
                else
                {
                    GameManager.Instance.GamePauseEvent?.Invoke(true);
                    //游戏结束
                    isPlaying = false;
                }

            }

        }
    }
    public void AddScore(InteractableObject interactable)
    {
        SucceededScore += 1;
        GetScoreText.text = SucceededScore.ToString() + "个";
    }

    public void MinusScore(InteractableObject interactable)
    {
        FailedScore += 1;
        MinusScoreText.text = FailedScore.ToString() + "个";
    }


    private Sprite GetFlowerSprite(FlowerColor flowerType)
    {
        if (flowerType == FlowerColor.Blue)
        {            
            return BlueFlowerSprite;
        }
        else if (flowerType == FlowerColor.Red) 
        {
            return RedFlowerSprite; 
        }
        else if (flowerType == FlowerColor.Yellow)
        {
            return YellowFlowerSprite;
        }
        return null;
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
