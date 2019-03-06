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
        if(file == null)
        {
            Debug.LogErrorFormat("MusicFile is Null");
            return;
        }

        MusicManager.instance.MusicLoad(file);
    }
}
