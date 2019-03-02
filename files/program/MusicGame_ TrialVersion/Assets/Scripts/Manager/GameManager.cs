using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全体の進行管理
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public TextAsset file;

    private void Start()
    {
        if(file != null)
        {
            MusicManager.instance.MusicLoad(file);
        }
        else
        {
            Debug.LogErrorFormat("MusicFile is Null");
        }
    }
}
