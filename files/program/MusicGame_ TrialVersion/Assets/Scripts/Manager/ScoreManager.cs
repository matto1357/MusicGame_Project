using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコア(というか判定数)を管理する
/// </summary>
public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    private int judge_GreatTimes;
    private int judge_GoodTimes;
    private int judge_BadTimes;

    public void ADDJudge_Great()
    {
        judge_GreatTimes++;
        Debug.Log("Great:" + judge_GreatTimes);
    }

    public void AddJudge_Good()
    {
        judge_GoodTimes++;
        Debug.Log("Good:" + judge_GoodTimes);
    }

    public void AddJudge_Bad()
    {
        judge_BadTimes++;
        Debug.Log("Bad:" + judge_BadTimes);
    }
}
