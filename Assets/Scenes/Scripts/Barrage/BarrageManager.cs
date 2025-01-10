using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarrageManager : MonoBehaviour
{
    public static bool isRead = false;
    public  bool isPause = true;
    [Header("左手")]
    public InteractableHand left_hand;
    [Header("右手")]
    public InteractableHand right_hand;

    public BarrageItem BarrageTemplate;

    //[HideInInspector]
    /// <summary>
    /// 积极弹幕
    /// </summary>
    public string PositiveBarrage;


    //[HideInInspector]
    /// <summary>
    /// 消极弹幕
    /// </summary>
    public string NegativeBarrage;

    [HideInInspector]
    public List<string> PositiveBarrageList;
    [HideInInspector]
    public List<string> NegativeBarrageList;


    public List<Sprite> barrageBGSprite;

    /// <summary>
    /// 向右移动的组
    /// </summary>
    private List<BarrageContent> TurnRightContent=new List<BarrageContent>();

    /// <summary>
    /// 向左移动的组
    /// </summary>
    private List<BarrageContent> TurnLeftContent = new List<BarrageContent>();


    public List<Image> rainbowImages;
    [Header("得到满分后出现的花")]
    public GameObject FinalFLower;

    /// <summary>
    /// 设置一共完成多少个正确答案后，金字塔达到满值
    /// </summary>
    public float TotalScore = 20;

    [Header("当前的分数")]
    public float CurrentScore = 0;

    [Header("当前的游戏时间")]
    private int CurrentDuration=0;

    [Header("设定的总时间")]
    public int TotalTimeSetting = 100;


    /// <summary>
    /// 成功的次数
    /// </summary>
    private int correctCount = 0;

    /// <summary>
    /// 错误的次数
    /// </summary>
    private int incorrectCount = 0;
    ///// <summary>
    ///// 当前得分的文本框
    ///// </summary>
    //public Text CurrentScoreText;
    /// <summary>
    /// 当前所用时间的文本框
    /// </summary>
    public Text CurrentTimeText;

    [Header("举手开始游戏的提示框")]
    /// <summary>
    /// 举手开始游戏提示框
    /// </summary>
    public GameObject Tips;
    [Header("结算页面")]
    /// <summary>
    /// 结算页面
    /// </summary>
    public GameObject FinalPage;

    public bool canStart = true;
    public int cache = -1;

    public float Speed = 1f;
    private void Awake()
    {
        PositiveBarrageList = PositiveBarrage.Split('、').ToList();
        NegativeBarrageList = NegativeBarrage.Split('、').ToList();
        left_hand.OnDragGetScoreCallback += ClickObject;
        right_hand.OnDragGetScoreCallback += ClickObject;
        GameManager.Instance.GamePauseEvent += PauseGame;
    }
    
    private void Start()
    {
        StartCoroutine(DelaySpawn());
        StartCoroutine(CalculateGameLogic());
        StartCoroutine(Utills.ReadLocalTxt(Path.Combine(Application.streamingAssetsPath, "BarrageSpeed.txt"), (str) => { Speed = float.Parse(str); }, null));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            PauseGame(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetProcesserImage(1,0.2f);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetProcesserImage(-1, 0.2f);
        }


    }
    private IEnumerator DelaySpawn()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(2f/Speed, 3f/Speed));
            if (isRead && !isPause)
            {
                CreatTemplate();
            }

        }
    }

    private IEnumerator CalculateGameLogic()
    {
        while (true) 
        {
            yield return new WaitForSecondsRealtime(1f);
            if (isRead)
            {
                if (isPause)
                {

                }
                else
                {
                    CurrentDuration += 1;
                    CurrentTimeText.text = (TotalTimeSetting - CurrentDuration).ToString()+"秒"; ;
                    if (CurrentDuration>=TotalTimeSetting)
                    {
                        canStart = true;
                        GameManager.Instance.SetGameState(false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 每次重新开始游戏数据清空
    /// </summary>
    public void ResetData()
    {
        CurrentDuration = 0;
        CurrentScore = 0;

        correctCount = 0;
        incorrectCount = 0;

        CurrentTimeText.text = CurrentDuration.ToString();
        //CurrentScoreText.text=CurrentScore.ToString();

        //imageProcess.fillAmount = 0;
        ResetProcesserImages();
        FinalFLower.SetActive(false);
    }

    /// <summary>
    /// 将数据显示在训练报告上
    /// </summary>
    public void CopyData()
    {
        //FinalecorrectcountText.text = string.Empty;

        //TODO:
        float percent = CurrentScore / TotalScore;
        FinalPage.transform.GetChild(0).Find("UseTimeValueText").GetChild(0).GetComponent<Text>().text = (Mathf.Clamp(CurrentDuration,0,120) ).ToString();
        FinalPage.transform.GetChild(0).Find("ScoreText").GetChild(0).GetComponent<Text>().text = (Mathf.Floor(percent * 100)).ToString();
        string scoreState = "";
        if (percent < 0.6f)
        {
            scoreState = "与情绪和解，让我们做情绪的主人！！！";
        }
        else if (percent >= 0.6 && percent <= 0.8)
        {
            scoreState = "厉害的人，不被情绪左右(๑•̀ㅂ•́)و✧";
        }
        else
        {
            scoreState = "你的开心吸引着美好的一切(*^▽^*)";
        }
        FinalPage.transform.GetChild(0).Find("CongratulationText").GetComponent<Text>().text = scoreState;
    }
    //public void SetProcesserImage(float value,float tweenDuration)
    //{
    //    float targetValue = Mathf.Clamp(imageProcess.fillAmount + value, 0, 1);
    //    imageProcess.DOFillAmount(targetValue, tweenDuration);
    //}

    private void SetProcesserImage(float value, float tweenDuration)
    {
        FinalFLower.SetActive(false);
        // Adjust CurrentScore and ensure it stays within bounds
        CurrentScore = Mathf.Clamp(CurrentScore + value, 0, TotalScore);

        // Calculate the filled amount based on the percentage
        float percent = CurrentScore / TotalScore;
        int countFilledAmount = rainbowImages.Count * 100;
        int targetFilledAmount = Mathf.Max((int)(percent * countFilledAmount), 0); // Ensure it's not negative

        int targetChangeIndex = targetFilledAmount / 100;

        if (targetChangeIndex > 0)
        {
            // Fill the previous image completely before moving to the next one
            rainbowImages[targetChangeIndex - 1].DOFillAmount(1f, tweenDuration);
        }
        else if (targetChangeIndex == 0 && value < 0)
        {
            // If going back to the first image and value is negative, reset the first image
            rainbowImages[targetChangeIndex].DOFillAmount(0f, tweenDuration);
        }

        if (rainbowImages.Count <= targetChangeIndex)
        {
            FinalFLower.SetActive(true);
            rainbowImages[rainbowImages.Count - 1].DOFillAmount(1, tweenDuration);
            canStart = true;
            GameManager.Instance.SetGameState(false);
        }
        else
        {
            rainbowImages[targetChangeIndex].DOFillAmount(targetFilledAmount % 100 / 100f, tweenDuration);
        }

    }

   private void ResetProcesserImages()
    {
        foreach (var item in rainbowImages)
        {
            item.fillAmount = 0;
        }
    }

    public void PauseGame(bool isPause)
    {
        if (!canStart)
        {
            return;
        }
        this.isPause= isPause;

        Tips.SetActive(isPause);
        //每次重新开始前，重置数据
        if (!isPause)
        {
            ResetData();
        }
        else
        {
            //Show finaleData
            foreach (var item in FindObjectsOfType<BarrageItem>().ToList())
            {
                item.DestroyObjectWithTween();
            }
            CopyData();
        }
        //放在最后，用以成功刷新数据后再显示界面


        FinalPage.SetActive(isPause);
    }
    public void ClickObject(DragEndCheck dragEndCheck)
    {
        switch (dragEndCheck)
        {
            case DragEndCheck.NoObject:
                Debug.Log("没有抓取到物体");
                break;
            case DragEndCheck.RightObject:
                Debug.Log("抓取到了正确的物体,加分");
                SetProcesserImage(1f,0.2f);
                correctCount += 1;
                break;
            case DragEndCheck.WrongObject:
                Debug.Log("抓取到了错误的物体,扣分");
                SetProcesserImage(-1f, 0.2f);
                incorrectCount += 1;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 设置已读训练须知
    /// </summary>
    public void SetRead()
    {
        isRead = true;
        Tips.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreatTemplate()
    {
        bool isSpwanLeft = Random.Range(1,3)>1?true:false;
        BarrageItem barrageItem;
        if (isSpwanLeft) 
        {
            barrageItem= Instantiate(BarrageTemplate, TurnRightContent[Random.Range(0, TurnRightContent.Count)].transform);
        }
        else
        {
            barrageItem = Instantiate(BarrageTemplate, TurnLeftContent[Random.Range(0, TurnLeftContent.Count)].transform);

        }
        barrageItem.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(isSpwanLeft?-960: 960, barrageItem.GetComponent<RectTransform>().anchoredPosition3D.y, barrageItem.GetComponent<RectTransform>().anchoredPosition3D.z);
        Sprite sprite = barrageBGSprite[Random.Range(0, barrageBGSprite.Count)];
        bool isPositive=Random.Range(1,3)>1?true: false;

        float speed = Random.Range(2.5f,2.8f);

        barrageItem.InitView(sprite, 
            isPositive ? PositiveBarrageList[Random.Range(0, PositiveBarrageList.Count)]: NegativeBarrageList[Random.Range(0, NegativeBarrageList.Count)],
            isSpwanLeft,
            isPositive,
            speed);

    }
    public void InitTurnLightAndLeftLists(BarrageContent barrageContent)
    {
        if (barrageContent.GetTurnRight())
        {
            TurnRightContent.Add(barrageContent);
        }
        else
        {
            TurnLeftContent.Add(barrageContent);
        }
    }


}
