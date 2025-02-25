using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SqliteMGR;

public class InsertNeedleManager : MonoBehaviour
{
    private CircleControl mainCircle;
    [SerializeField]
    private GameObject spawItem;
    [SerializeField]
    private Transform spawPoint;

    [SerializeField]
    private GameObject TrainingReport;

    [SerializeField]
    private GameObject Tips;
    bool canSpaw = false;

    [SerializeField]
    private GameObject CircleBG;

    /// <summary>
    /// 游戏进行的时间
    /// </summary>
    private int durationTime = 0;

    /// <summary>
    /// 游戏得分
    /// </summary>
    private int score = 0;

    [SerializeField]
    private TextMeshProUGUI durationTimeText;
    [SerializeField]
    private TextMeshProUGUI scoreTimeText;

    /// <summary>
    /// 协程等待时间
    /// </summary>
    WaitForSeconds wait = new WaitForSeconds(1f);

    private bool isReadIntroduce = false;

    public SqliteMGR.UserData userData = new UserData();
    private void Start()
    {
        if (mainCircle == null)
        {
            mainCircle = FindObjectOfType<CircleControl>();
            mainCircle.InsertNeedleManager = this;
        }
        GameManager.Instance.clapListener.ClapCallback += SpawnItem;
        GameManager.Instance.GamePauseEvent += SetSpawnState;
        StartCoroutine(GameCalculate());
        userData.userInfo=GetUserInfoFromJson();
        KinectGameDBHelper kinectGameDBHelper = new KinectGameDBHelper();
        kinectGameDBHelper.InsertUserData(userData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.Instance.GamePauseEvent?.Invoke(false);
            canSpaw = true;
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {

            SpawnItem();
        }
    }

    IEnumerator GameCalculate()
    {
        while (true)
        {
            yield return null;
            if (canSpaw)
            {
                yield return wait;
                durationTime += 1;
                durationTimeText.text = durationTime.ToString()+"秒";
            }

        }
    }

    private void SpawnItem()
    {
        if (canSpaw)
        {
            SpawItem _item = Instantiate(spawItem, spawPoint.transform).GetComponent<SpawItem>();
            _item.transform.localEulerAngles = Vector3.zero;
            _item.transform.localPosition = Vector3.zero;
            _item.GetScoreAction += AddScore;
        }
        else
        {
            Debug.Log("当前游戏暂停，无法发射");
        }
    }


    private void SetSpawnState(bool _isPause)
    {
        if (!isReadIntroduce) 
            return;
        canSpaw = !_isPause;
        if (!_isPause)
        {
            durationTime = 0;
            score = 0;
            durationTimeText.text = durationTime.ToString();
            scoreTimeText.text = score.ToString()+"个";
        }
        OnGameEnd(_isPause);
    }

    private void AddScore(int _score)
    {
        score += _score;
        scoreTimeText.text = score.ToString()+"个";
    }


    public void ConfirmReadIntro()
    {
        isReadIntroduce = true;
    }

    public bool GetConfirmReadIntro()
    {
        return isReadIntroduce;
    }

    public void OnGameEnd(bool isEnd)
    {

        if (isEnd)
        {
            //根据两个text设置UI内容
            TrainingReport.transform.Find("Score/Number/NumberValue").GetComponent<TextMeshProUGUI>().text = score.ToString()+"个";
            TrainingReport.transform.Find("Score/Time/TimeValue").GetComponent<TextMeshProUGUI>().text=durationTime.ToString()+"秒";
            FindObjectOfType<ShootScreenTool>().SetReportImage(() => 
            {
                TrainingReport.SetActive(isEnd);
            },
            () => 
            {
                mainCircle.transform.localRotation = Quaternion.identity;
                CircleBG.GetComponent<Image>().color = new Color(0.9843138f, 0.9450981f, 0.9019608f, 1);
            },
            () => 
            {
                CircleBG.GetComponent<Image>().color = new Color(0.9843138f, 0.9450981f, 0.9019608f, 0);
                Tips.SetActive(true);
            });
            userData.UserGameData.GameEndTime = GetCurrentTime();
            userData.UserGameData.GameName = "手眼协调";
            userData.UserGameData.GameCateloge = "身心协调训练";
            userData.UserGameData.GameScore = score;
            userData.UserGameData.GameResult = score > 10 ? "成功" : "失败";
            userData.UserGameData.GameDuration = durationTime;
            KinectGameDBHelper kinectGameDBHelper = new KinectGameDBHelper();
            kinectGameDBHelper.InsertUserData(userData);
        }
        else
        {
            //UI界面关闭
            TrainingReport.SetActive(isEnd);
            Tips.SetActive(false);
            Tips.GetComponentInChildren<Text>().text = "举起左手重新开始游戏";
            userData.UserGameData.GameStartTime = GetCurrentTime();

        }
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
