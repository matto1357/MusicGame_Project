using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 判定幅を保持
/// </summary>
[CreateAssetMenu(menuName ="MusicGame/Create JudgeWidth", fileName = "JudgeWidth", order =100)]
public class JudgeWidth : ScriptableObject
{
    [Header("全て秒換算で入力してください")]
    public float[] JudgeTimings;
}
