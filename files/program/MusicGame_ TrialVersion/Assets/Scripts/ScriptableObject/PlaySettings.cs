using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイに関連する設定
/// </summary>
[CreateAssetMenu(menuName = "MusicGame/Create PlaySettings", fileName = "PlaySettings")]
public class PlaySettings : ScriptableObject
{
    [Header("1秒辺りの速度")]
    public float notesSpeed;
}
