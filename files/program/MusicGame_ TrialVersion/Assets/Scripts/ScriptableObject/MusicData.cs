using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

[CreateAssetMenu(menuName = "MusicGame/Create MusicData", fileName = "_MusicData", order =0)]
[System.Serializable]
public class MusicData : ScriptableObject
{
    public TextAsset[] musicSeets;

    public void Insert()
    {
        string filePath = AssetDatabase.GetAssetPath(this);
        string folderPath = Path.GetDirectoryName(filePath);
        DirectoryInfo dir = new DirectoryInfo(folderPath);
        FileInfo[] files = dir.GetFiles("*.txt");
        foreach(FileInfo info in files)
        {
            TextAsset data = 
        }
    }
}
