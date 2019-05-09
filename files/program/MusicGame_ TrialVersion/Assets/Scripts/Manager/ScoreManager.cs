using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 判定数と、それに掛かる物(ゲージ・スコアなど)を管理する
/// </summary>
public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    [SerializeField]
    private int judge_GreatTimes;
    [SerializeField]
    private int judge_GoodTimes;
    [SerializeField]
    private int judge_BadTimes;
    [SerializeField]
    private float score;
    [SerializeField]
    private float combo;
    private string scoreString = "0.00";

    public Text greatCount;
    public Text goodCount;
    public Text missCount;
    public Text scoreCount;
    public Text judgeStr;
    public Text FSStr;

    public void AddJudge(JudgeState state, NotesScript note, float diff = 0.0f)
    {
        AddScore(state, note);
        SetJudgeString(state, note);
        string FS;
        if(diff >= 0.0f)
        {
            FS = "FAST";
        }
        else
        {
            FS = "SLOW";
        }
        switch(state)
        {
            case JudgeState.Great:
                AddJudge_Great();
                FSStr.text = "";
                break;
            case JudgeState.Good:
                AddJudge_Good();
                FSStr.text = FS;
                break;
            case JudgeState.Bad:
                AddJudge_Bad();
                FSStr.text = FS;
                if (note.type == ScoreIndex.LONG)
                {
                    AddJudge_Bad();
                }
                break;
            case JudgeState.AUTO:
                AddJudge_Great();
                if(note.type == ScoreIndex.LONG)
                {
                    AddJudge_Great();
                }
                break;
        }
    }

    private void AddJudge_Great()
    {
        judge_GreatTimes++;
        //Debug.Log("Great:" + judge_GreatTimes);
    }

    private void AddJudge_Good()
    {
        judge_GoodTimes++;
        //Debug.Log("Good:" + judge_GoodTimes);
    }

    private void AddJudge_Bad()
    {
        judge_BadTimes++;
        //Debug.Log("Bad:" + judge_BadTimes);
    }

    private void SetJudgeString(JudgeState state, NotesScript note)
    {
        switch(state)
        {
            case JudgeState.Great:
                combo++;
                judgeStr.text = "GREAT " + combo.ToString();
                break;
            case JudgeState.Good:
                combo++;
                judgeStr.text = "GOOD " + combo.ToString();
                break;
            case JudgeState.Bad:
                combo = 0;
                judgeStr.text = "BAD";
                break;
            case JudgeState.AUTO:
                if(note.type == ScoreIndex.LONG)
                {
                    combo += 2;
                }
                else
                {
                    combo++;
                }
                judgeStr.text = "AUTO " + combo.ToString();
                break;
        }
    }

    private void AddScore(JudgeState state, NotesScript note)
    {
        switch(state)
        {
            case JudgeState.Great:
                score += 2;
                break;
            case JudgeState.Good:
                score += 1;
                break;
            case JudgeState.AUTO:
                if(note.type == ScoreIndex.LONG)
                {
                    score += 4;
                }
                else
                {
                    score += 2;
                }
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        greatCount.text = judge_GreatTimes.ToString();
        goodCount.text = judge_GoodTimes.ToString();
        missCount.text = judge_BadTimes.ToString();
        float percent = Mathf.Round((float)score / ((float)MusicManager.instance.notesCount * 2f) * 10000f) / 100f;
        scoreString = percent.ToString("f2");
        scoreCount.text = scoreString;
    }
}
