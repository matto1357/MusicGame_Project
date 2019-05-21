using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode
{
    Normal = 0,
    Auto,
}

public enum SeetMode
{
    SeetData = 0,
    JointData,
}

/// <summary>
/// 全体の進行管理
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public PlayMode mode;
    public SeetMode testMode;

    public TextAsset file;
    public MusicData musicData;
    [System.NonSerialized]
    public SeetData seetData;

    private void Start()
    {
        if(file == null)
        {
            Debug.LogErrorFormat("MusicFile is Null");
            return;
        }

        if(testMode == SeetMode.SeetData)
        {
            MusicManager.instance.MusicLoad(file);
        }
        else
        {
            seetData = musicData.musicSeets[0];
            MusicManager.instance.MusicLoad(seetData);
        }
    }
}
