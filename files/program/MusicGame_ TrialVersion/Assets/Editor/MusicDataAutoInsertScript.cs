using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(MusicData))]//拡張対象クラス
public class MusicDataAutoInsertScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MusicData musicData = target as MusicData;

        if(GUILayout.Button("AutoInsert"))
        {
            musicData.musicSeets = Insert(musicData);

            //保存
            EditorUtility.SetDirty(musicData);
            AssetDatabase.SaveAssets();
        }
    }

    private GameObject obj;

    public List<SeetData> Insert(MusicData musicData)
    {
        List<SeetData> musicSeets = new List<SeetData>();

        string filePath = AssetDatabase.GetAssetPath(musicData);
        string folderPath = Path.GetDirectoryName(filePath);
        string[] files = Directory.GetFiles(folderPath);

        obj = new GameObject();
        MusicManager musicManager = obj.AddComponent<MusicManager>();

        foreach (string str in files)
        {
            if (Path.GetExtension(str) == ".txt")
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath(str, typeof(TextAsset)) as TextAsset;
                SeetData seetData = new SeetData();
                seetData.seet = data;
                musicManager.MusicLoad(data, LoadMode.PreLoad);
                seetData.notesData = musicManager.notesData;
                seetData.clip = musicManager.data_AUDIO;

                musicSeets.Add(seetData);
            }
        }

        EditorApplication.delayCall += DestroyObj;

        return musicSeets;
    }

    private void DestroyObj()
    {
        EditorApplication.delayCall -= DestroyObj;
        DestroyImmediate(obj);
    }
}
