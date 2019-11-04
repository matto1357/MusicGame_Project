using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ConvertSetting))]//拡張対象クラス
public class NotesConverter : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ConvertSetting convertSetting = target as ConvertSetting;

        if(GUILayout.Button("Convert"))
        {
            TextAsset textAsset = convertSetting.seetSource;
            DataType type = convertSetting.targetDataType;
            Convert(textAsset, type);
        }
    }

    private GameObject obj;

    public void Convert(TextAsset textAsset, DataType type)
    {
        obj = new GameObject();
        MusicManager manager = obj.AddComponent<MusicManager>();
        manager.MusicLoad(textAsset, LoadMode.PreLoad);
        if (manager.data_DataType == type)
        {
            Debug.LogError("変換元と変換先のDataTypeが一致しています");
            EditorApplication.delayCall += DestroyObj;
            return;
        }

        string filePath = AssetDatabase.GetAssetPath(textAsset);
        string folderPath = Path.GetDirectoryName(filePath);
        string path = Directory.GetParent(UnityEngine.Application.dataPath) + "/" + folderPath + "/converted.txt";
        using (File.Create(path)) { };
        StreamWriter sw = new StreamWriter(path);

        bool cont = true;
        int[] laneIndex = new int[manager.data_MusicScore.Length];
        while(cont)
        {
            cont = false;
            NotesInfo info = null;
            int tgt = 0;

            for(int lane = 0; lane < manager.data_MusicScore.Length; lane++)
            {
                if(manager.data_MusicScore[lane].Count == laneIndex[lane])
                {
                    continue;
                }
                
                if(info == null)
                {
                    info = manager.data_MusicScore[lane][laneIndex[lane]];
                    tgt = lane;
                    cont = true;
                }
                else
                {
                    float info_time = (float)info.bar + (float)info.th / (float)info.barOriginTh;
                    NotesInfo tgtInfo = manager.data_MusicScore[lane][laneIndex[lane]];
                    float tgt_time = (float)tgtInfo.bar + (float)tgtInfo.th / (float)tgtInfo.barOriginTh;
                    if(tgt_time < info_time)
                    {
                        info = tgtInfo;
                        tgt = lane;
                    }
                }
            }
            if(cont)
            {
                string str = "[" + tgt + "," + ((int)info.type) + "," + info.bar + "." + info.th + "/" + info.barOriginTh + "]";
                sw.WriteLine(str);
                laneIndex[tgt]++;
            }
        }

        //for(int lane = 0; lane < manager.data_MusicScore.Length; lane++)
        //{
        //    foreach(NotesInfo notes in manager.data_MusicScore[lane])
        //    {
        //        string str = "[" + lane + "," + ((int)notes.type) + "," + notes.bar + "." + notes.th + "/" + notes.barOriginTh + "]";
        //        Debug.Log(str);
        //        sw.WriteLine(str);
        //    }
        //}
        sw.Close();


        EditorApplication.delayCall += DestroyObj;
    }

    private void DestroyObj()
    {
        EditorApplication.delayCall -= DestroyObj;
        DestroyImmediate(obj);
    }
}
