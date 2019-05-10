using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicGame/Create MusicData", fileName = "_MusicData", order =0)]
[System.Serializable]
public class MusicData : ScriptableObject
{
    public TextAsset[] musicSeets;

    public void Insert()
    {
        musicSeets[0] = null;
        //TextAssetで保持してみる
    }
}
