using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerBasket : MonoBehaviour
{
    /// <summary>
    /// 收集的类型
    /// </summary>
    [SerializeField]
    private FlowerColor CollectType;

    private int MergeCondition = 3;
    InteractableObject interactableCache;

    public Action<InteractableObject> CollectSucceedAction;

    public Action<InteractableObject> CollectFailedAction;

    private RedBlueManager redBlueManager;

    int totalflowerCount = 0;

    /// <summary>
    /// 合成后显示在花篮里的花
    /// </summary>
    [SerializeField]
    private GameObject MergeFlowerPrefab;
    [SerializeField]
    private Sprite SucceedSprite;
    [SerializeField]
    private Sprite FailedSprite;

    private Image m_TweenUI;

    private Color defaultAlphaColor;

    private List<GameObject> FlowerList = new List<GameObject>();
    private void Start()
    {
        redBlueManager = FindObjectOfType<RedBlueManager>();
        m_TweenUI = transform.Find("TweenIcon").GetComponent<Image>();
        defaultAlphaColor = new Color(m_TweenUI.color.r, m_TweenUI.color.g, m_TweenUI.color.b,1);

        GameManager.Instance.GamePauseEvent += this.GamePauseEvent;
        //HandStateChangeEvent?.Invoke(KinectInterop.JointType.HandLeft, _leftHandState);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AddToBasket(1);
        }

    }
    private void GamePauseEvent(bool pause)
    {
        ClearFlower();
        return;
        if (!pause)
        {
            ClearFlower();
        }
    }

    public bool GetFlowerEaqul(InteractableObject interactable)
    {
        bool isEaqul = interactable.GetFlowerType() == CollectType;
        m_TweenUI.sprite = isEaqul ? SucceedSprite : FailedSprite;
        m_TweenUI.color = defaultAlphaColor;
        m_TweenUI.DOFade(0,1f);
        return isEaqul;
    }

    public void AddToBasket(int addValue)
    {
        //flowerCount += addValue;
        //int MergeCount = flowerCount / MergeCondition;
        //if (flowerCount % MergeCondition == 0)
        //{
        //    GameObject flower = Instantiate(MergeFlowerPrefab, this.transform.GetChild(0));
        //    FlowerList.Add(flower);
        //}

        ////修改代码：当flowerCount增加且没达到mergecondition时，使得flowerlist[flowerCount % MergeCondition]的image组件的透明度按照百分比递增flower/mergecontion;达到mergecondition时，实例化flower并且将这个flower的透明度设置为flowerCount/mergecondition

        totalflowerCount += addValue;

        int currentflower = totalflowerCount / MergeCondition;
        float flowerAlpha = (float)totalflowerCount % MergeCondition;

        // 处理增加花朵的情况，当flowerAlpha为1时添加新花朵
        if (flowerAlpha == 1)
        {
            GameObject flower = Instantiate(MergeFlowerPrefab, this.transform.GetChild(0));
            FlowerList.Add(flower);
            flower.GetComponent<Image>().color = new Color(1, 1, 1, flowerAlpha / MergeCondition);
        }
        // 处理flowerAlpha为0的情况，确保索引合法后进行操作
        else if (flowerAlpha == 0 && currentflower > 0 && FlowerList.Count >= currentflower - 1)
        {
            if (currentflower - 1 < FlowerList.Count)
            {
                FlowerList[currentflower - 1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            // 可以考虑在这里添加额外逻辑，比如如果列表元素数量刚好比currentflower少1，是否需要做其他处理等
        }
        // 处理其他情况，同样确保索引合法
        else
        {
            if (currentflower < FlowerList.Count)
            {
                FlowerList[currentflower].GetComponent<Image>().color = new Color(1, 1, 1, flowerAlpha / MergeCondition);
            }
            else if (currentflower == FlowerList.Count)
            {
                // 当相等时，可根据具体需求添加对应的处理逻辑，比如添加新元素或者其他操作
                GameObject flower = Instantiate(MergeFlowerPrefab, this.transform.GetChild(0));
                FlowerList.Add(flower);
                flower.GetComponent<Image>().color = new Color(1, 1, 1, flowerAlpha / MergeCondition);
            }
            // 如果currentflower大于FlowerList.Count，可以考虑在这里添加提示信息或者进行其他合适的处理，比如扩容等操作（取决于具体需求）
        }

        return;
        //totalflowerCount += addValue;
        //int currentflower = totalflowerCount / MergeCondition;
        //float flowerAlpha = (float)totalflowerCount % MergeCondition;
        //if (flowerAlpha==1)
        //{
        //    GameObject flower = Instantiate(MergeFlowerPrefab, this.transform.GetChild(0));
        //    FlowerList.Add(flower);
        //    flower.GetComponent<Image>().color = new Color(1, 1, 1, flowerAlpha/ MergeCondition);
        //}
        //else if(flowerAlpha==0)
        //{
        //    FlowerList[currentflower - 1].GetComponent<Image>().color=new Color(1,1,1,1);
        //}
        //else
        //{
        //    if (FlowerList.Count > currentflower)
        //    {
        //        FlowerList[currentflower].GetComponent<Image>().color = new Color(1, 1, 1, flowerAlpha / MergeCondition);
        //    }

        //    //try
        //    //{

        //    //}
        //    //catch (Exception)
        //    //{

        //    //    Debug.LogErrorFormat("Flower list count:{0}  code target flowerlist index:{1},basket name:{2}",FlowerList.Count, floawerCount - 1,this.transform.name);
        //    //}

        //}
        //if (flowerCount % MergeCondition == 0)
        //{
        //    GameObject flower = Instantiate(MergeFlowerPrefab, this.transform.GetChild(0));
        //    FlowerList.Add(flower);
        //    // Set the transparency of the new flower
        //    Color color = flower.GetComponent<Image>().color;
        //    color.a = (float)flowerCount / MergeCondition;
        //    flower.GetComponent<Image>().color = color;
        //}
        //else
        //{
        //    // Increment the transparency of the existing flower
        //    int index = flowerCount % MergeCondition;
        //    GameObject existingFlower = FlowerList[index];
        //    Color color = existingFlower.GetComponent<Image>().color;
        //    color.a += (float)addValue / MergeCondition;
        //    existingFlower.GetComponent<Image>().color = color;
        //}

    }
    public void ClearFlower()
    {
        foreach (var flower in FlowerList) 
        {
            Destroy(flower);
        }
        FlowerList.Clear();
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactableCache = collision.GetComponent<InteractableObject>();
        if (interactableCache != null)
        {
            if (interactableCache.GetDragState())
            {
                if (GetFlowerEaqul(interactableCache))
                {
                    AddToBasket(1);
                    FindObjectOfType<RedBlueManager>().AddScore(interactableCache);

                    //Destroy(interactableCache.gameObject);
                    KinectInterop.JointType jointType = interactableCache.GetCurrentJointType();
                    GameManager.Instance.clapListener.HandStateChangeEvent?.Invoke(jointType, KinectInterop.HandState.Open);
                    ////
                    Debug.Log("defen");
                    Destroy(interactableCache.gameObject);
                }
                else
                {
                    //扣分
                    //AddToBasket(-1);
                    FindObjectOfType<RedBlueManager>().MinusScore(interactableCache);



                    KinectInterop.JointType jointType = interactableCache.GetCurrentJointType();
                    GameManager.Instance.clapListener.HandStateChangeEvent?.Invoke(jointType, KinectInterop.HandState.Open);
                    Destroy(interactableCache.gameObject);
                }
            }
            return;
            if (GetFlowerEaqul(interactableCache))
            {
                if (interactableCache.GetDragState())
                {
                    //取消抓取
                    //得分
                    //CollectSucceedAction?.Invoke(interactableCache);
                    //redBlueManager.GetScore(interactableCache);

                    AddToBasket(1);
                    FindObjectOfType<RedBlueManager>().AddScore(interactableCache);

                    //Destroy(interactableCache.gameObject);
                    KinectInterop.JointType jointType = interactableCache.GetCurrentJointType();
                    GameManager.Instance.clapListener.HandStateChangeEvent?.Invoke(jointType, KinectInterop.HandState.Open);
                    ////
                    Debug.Log("defen");
                    Destroy(interactableCache.gameObject);
                }
                else
                {
                    //无效得分
                }
            }
            else
            {
                if (interactableCache.GetDragState())
                {
                    //扣分
                    //AddToBasket(-1);
                    FindObjectOfType<RedBlueManager>().MinusScore(interactableCache);



                    KinectInterop.JointType jointType = interactableCache.GetCurrentJointType();
                    GameManager.Instance.clapListener.HandStateChangeEvent?.Invoke(jointType, KinectInterop.HandState.Open);
                    Destroy(interactableCache.gameObject);
                }

                //KinectInterop.JointType jointType = interactableCache.GetCurrentJointType();
                //GameManager.Instance.clapListener.HandStateChangeEvent?.Invoke(jointType, KinectInterop.HandState.Open);
                //Debug.Log("koufen");
            }

        }
    }
}
