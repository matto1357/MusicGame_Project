using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ノーツ毎に付けて、移動を行う
/// </summary>
public class NotesController : MonoBehaviour
{
    /// <summary>
    /// このノーツの時間
    /// </summary>
    [System.NonSerialized]
    public float noteTime;

    /// <summary>
    /// このノーツの見逃しBadの位置
    /// </summary>
    private float noteDestroyPoint;

    /// <summary>
    /// このノーツのBPM
    /// 速度変化の時に必要になりそう
    /// </summary>
    [System.NonSerialized]
    public float noteBPM = 160;

    private void Start()
    {
        noteDestroyPoint = NotesManager.instance.judgeLinePos.y - //判定ラインから
            (NotesManager.instance.judgeWidth.JudgeTimings[NotesManager.instance.judgeWidth.JudgeTimings.Length - 1] //見逃し判定を
            / 1000 //秒単位にして
            * (NotesManager.instance.notesSpeed //ノーツスピードを掛けて
            * (NotesManager.instance.BPM / noteBPM))); //BPMの倍率を掛けてから引く

        Debug.Log(noteDestroyPoint);
    }

    private void FixedUpdate()
    {
        NoteMove();

        if(this.transform.position.y < noteDestroyPoint)
        {
            DestroyNote();
        }
    }

    /// <summary>
    /// ノーツが動く処理
    /// </summary>
    private void NoteMove()
    {
        if (NotesManager.instance.notesActive)
        {
            Vector3 pos = this.transform.position;
            pos.y -= (NotesManager.instance.notesSpeed * (NotesManager.instance.BPM / noteBPM)) * Time.deltaTime;
            this.transform.position = pos;
        }
    }

    /// <summary>
    /// ノーツを消去して、MISS判定を与える
    /// </summary>
    private void DestroyNote()
    {
        ScoreManager.instance.AddJudge_Bad();
        Destroy(this.gameObject);
    }
}
