using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoSingleton<BGMManager>
{
    /// <summary>
    /// 背景音乐播放器
    /// </summary>
    public AudioSource BGMAudio;

    /// <summary>
    /// 得分或丢分播放器
    /// </summary>
    public AudioSource ScoreEffectAudio;
    /// <summary>
    /// 背景音乐列表
    /// </summary>
    public List<AudioClip> audioClips;


    /// <summary>
    /// 得分成功的音效
    /// </summary>
    public List<AudioClip> GetScoreSucceedAudioEffectList;

    /// <summary>
    /// 得分失败的音效
    /// </summary>
    public List<AudioClip> GetScoreFailedAudioEffectList;


    /// <summary>
    /// 展示得分或者失分音效
    /// </summary>
    /// <param name="isSucceed"></param>
    public void ShowGetScoreAudioEffect(bool isSucceed)
    {
        if (isSucceed)
        {
            ScoreEffectAudio.clip = GetScoreSucceedAudioEffectList[Random.Range(0, GetScoreSucceedAudioEffectList.Count)];
        }
        else
        {
            ScoreEffectAudio.clip = GetScoreFailedAudioEffectList[Random.Range(0, GetScoreFailedAudioEffectList.Count)];
        }
    }

    
}
