using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode
{
    Normal = 0,
    Auto,
}

/// <summary>
/// 全体の進行管理
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public PlayMode mode;

    public TextAsset file;

    private void Start()
    {
        if(file == null)
        {
            Debug.LogErrorFormat("MusicFile is Null");
            return;
        }

        MusicManager.instance.MusicLoad(file);
    }
}
