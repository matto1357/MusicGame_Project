using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Musics))]
public class SearchMusics : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Musics musics = target as Musics;

        if (GUILayout.Button("AutoSearch"))
        {
            musics.musicDatas = Search(musics);

            //保存
            EditorUtility.SetDirty(musics);
            AssetDatabase.SaveAssets();
        }
    }

    public List<MusicData> Search(Musics musics)
    {
        List<MusicData> datas = new List<MusicData>();
        Queue<string> directories = new Queue<string>();

        string filePath = AssetDatabase.GetAssetPath(musics);
        string folderPath = Path.GetDirectoryName(filePath);
        directories.Enqueue(folderPath);
        
        while (directories.Count > 0)
        {
            string dir = directories.Dequeue();

            string[] files = Directory.GetFiles(dir);
            foreach(string file in files)
            {
                if(Path.GetExtension(file) == ".asset")
                {
                    try
                    {
                        MusicData data = AssetDatabase.LoadAssetAtPath(file, typeof(MusicData)) as MusicData;
                        if(data != null)
                        {
                            datas.Add(data);
                        }
                    }
                    catch
                    {
                        
                    }
                }
            }

            string[] dirs = Directory.GetDirectories(dir);
            foreach(string str in dirs)
            {
                directories.Enqueue(str);
            }
        }
        return datas;
    }
}
