using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スコア(というか判定数)を管理する
/// </summary>
public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    [SerializeField]
    private int judge_GreatTimes;
    [SerializeField]
    private int judge_GoodTimes;
    [SerializeField]
    private int judge_BadTimes;

    public Text greatCount;
    public Text goodCount;
    public Text missCount;

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

    private void Update()
    {
        greatCount.text = judge_GreatTimes.ToString();
        goodCount.text = judge_GoodTimes.ToString();
        missCount.text = judge_BadTimes.ToString();
    }
}
