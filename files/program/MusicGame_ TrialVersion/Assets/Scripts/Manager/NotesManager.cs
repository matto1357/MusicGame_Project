using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ノーツ生成を管理する
/// </summary>

public class NotesManager : SingletonMonoBehaviour<NotesManager>
{
    //今のBPM
    public float BPM = 160;

    public JudgeWidth judgeWidth;
    public PlaySettings playSettings;
    [System.NonSerialized]
    public float notesSpeed;

    [System.NonSerialized]
    public GameObject judgeLine;
    [System.NonSerialized]
    public Vector3 judgeLinePos;

    /// <summary>
    /// ノーツが流れるかどうか
    /// </summary>
    public bool notesActive;

    private void Start()
    {
        notesSpeed = playSettings.notesSpeed;
        judgeLine = GameObject.FindGameObjectWithTag("JudgeLine");
        judgeLinePos = judgeLine.transform.position;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            notesActive = !notesActive;
        }
    }
}
