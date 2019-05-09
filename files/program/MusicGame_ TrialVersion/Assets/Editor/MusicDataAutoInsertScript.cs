using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicData))]//拡張対象クラス
public class MusicDataAutoInsertScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MusicData musicData = target as MusicData;

        if(GUILayout.Button("AutoInsert"))
        {
            musicData.Insert();

            //保存
            EditorUtility.SetDirty(musicData);
            AssetDatabase.SaveAssets();
        }
    }
}
