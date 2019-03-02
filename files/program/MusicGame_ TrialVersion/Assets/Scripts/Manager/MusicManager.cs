using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 譜面の各情報
/// </summary>
public enum MusicIndex
{
    none = -1,

    NOTES = 0,
    TITLE,
    SUBTITLE,
    AUDIO,
    ARTIST,
    OFFSET,
    BPM,
    KEY,
    DIFFICULT,
    LEVEL,
    STOP,
    GIMMICK,
    MOVETYPE,
}

public enum ScoreIndex
{
    none = -1,

    SIMPLE = 0,
    LONG_START,
    LONG_END,
    MINE,
}


public class BPMS
{
    public float Timing = 0.0f;
    public float BPM = 0.0f;
}

public class STOPS
{
    
}

public class GIMMICKS
{

}

/// <summary>
/// 譜面ファイルを読み込む
/// </summary>
public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    [System.NonSerialized]
    public string data_TITLE;
    [System.NonSerialized]
    public string data_SUBTITLE;
    [System.NonSerialized]
    public AudioClip data_AUDIO;
    [System.NonSerialized]
    public string data_ARTIST;
    [System.NonSerialized]
    public float data_OFFSET;
    [System.NonSerialized]
    public List<BPMS> data_BPM = new List<BPMS>();
    [System.NonSerialized]
    public int data_KEY;
    [System.NonSerialized]
    public string data_DIFFICULT;
    [System.NonSerialized]
    public int data_LEVEL;
    [System.NonSerialized]
    public STOPS[] data_STOP;
    [System.NonSerialized]
    public GIMMICKS[] data_GIMMICK;
    [System.NonSerialized]
    public int data_MOVETYPE;

    [System.NonSerialized]
    public Dictionary<ScoreIndex, string> NotesDictionary = new Dictionary<ScoreIndex, string>()
    {
        {ScoreIndex.none, "0"},
        {ScoreIndex.SIMPLE, "1" },
        {ScoreIndex.LONG_START, "2" },
        {ScoreIndex.LONG_END, "3" },
        {ScoreIndex.MINE, "M" },
    };

    /// <summary>
    /// ファイル分解
    /// </summary>
    /// <param name="file"></param>
    public void MusicLoad(TextAsset file)
    {
        StringReader sr = new StringReader(file.text);
        string lines = null;
        MusicIndex index = MusicIndex.none;

        //ノーツ情報が出てくるまで1行ずつ読み込みを行う
        while (sr.Peek() != -1)
        {
            //行読み込み
            string line = sr.ReadLine();

            //譜面情報かどうか確認
            if(lines != null || (line.Length != 0 && line.Substring(0, 1) == "#"))
            {
                if(index == MusicIndex.none)
                {
                    index = CheckIndex(line);
                }

                //ノーツ情報が来たらBreak
                if (lines == null && index == MusicIndex.NOTES)
                {
                    break;
                }

                //タグが無かったら次の行へ
                if(lines == null && index == MusicIndex.none)
                {
                    continue;
                }

                lines += line;

                if(line.Substring(line.Length - 1, 1) == ";")
                {
                    IndexLoad(index, lines, file);
                    lines = null;
                    index = MusicIndex.none;
                }
            }
        }

        MakeMusic(sr);
    }

    /// <summary>
    /// お前はどのタグだ
    /// </summary>
    /// <param name="line">行</param>
    /// <returns></returns>
    public MusicIndex CheckIndex(string line)
    {
        MusicIndex index = MusicIndex.none;
        string str = null;

        for(int i = 0; i < line.Length; i++)
        {
            if(line.Substring(i, 1) == ":")
            {
                str = str = line.Substring(1, i - 1);
                break;
            }
        }

        if(str == null)
        {
            Debug.LogWarningFormat("タグを読み込めませんでした : " + line);
            return index;
        }

        try
        {
            index = (MusicIndex)Enum.Parse(typeof(MusicIndex), str);
        }
        catch
        {
            Debug.LogWarningFormat("譜面情報のタグが正しくありません : " + str);
        }

        return index;
    }

    /// <summary>
    /// タグから情報を読み込む
    /// </summary>
    /// <param name="index">タグ</param>
    public void IndexLoad(MusicIndex index, string line, TextAsset file)
    {
        int tagLength = 1 + index.ToString().Length + 1;
        string contents = line.Substring(tagLength, line.Length - tagLength - 1);
        switch (index)
        {
            case MusicIndex.TITLE:
                data_TITLE = contents;
                break;
            case MusicIndex.SUBTITLE:
                data_SUBTITLE = contents;
                break;
            case MusicIndex.AUDIO:
                string fileDirectory = AssetDatabase.GetAssetPath(file);
                string folderDirectory = Path.GetDirectoryName(fileDirectory);
                string tgtDirectory = folderDirectory + "/" + contents;
                data_AUDIO = (AudioClip)AssetDatabase.LoadAssetAtPath(tgtDirectory, typeof(AudioClip));
                break;
            case MusicIndex.ARTIST:
                data_ARTIST = contents;
                break;
            case MusicIndex.OFFSET:
                if(contents.Substring(contents.Length - 1, 1) == "f")
                {
                    contents = contents.Substring(0, contents.Length - 1);
                }
                data_OFFSET = float.Parse(contents);
                break;
            case MusicIndex.BPM:
                data_BPM = BreakContents_BPM(contents);
                break;
            case MusicIndex.KEY:
                data_KEY = int.Parse(contents);
                break;
            case MusicIndex.DIFFICULT:
                data_DIFFICULT = contents;
                break;
            case MusicIndex.LEVEL:
                data_LEVEL = int.Parse(contents);
                break;
            case MusicIndex.STOP:
                //実装
                break;
            case MusicIndex.GIMMICK:
                //実装
                break;
            case MusicIndex.MOVETYPE:
                data_MOVETYPE = int.Parse(contents);
                break;
        }
    }

    /// <summary>
    /// BPMを入れるよ
    /// </summary>
    /// <param name="contents">情報</param>
    /// <returns></returns>
    public List<BPMS> BreakContents_BPM(string contents)
    {
        List<BPMS> bpms = new List<BPMS>();
        List<string> breakContents = new List<string>();
        int index = 0;

        for(int i = 0; i < contents.Length; i++)
        {
            if(contents.Substring(i, 1) == ",")
            {
                breakContents.Add(contents.Substring(index, i - index));
                index = i + 1;
            }
        }

        breakContents.Add(contents.Substring(index, contents.Length - index));

        foreach (string str in breakContents)
        {
            for(int i = 0; i < str.Length; i++)
            {
                if(str.Substring(i, 1) == "=")
                {
                    BPMS bpm = new BPMS();
                    bpm.Timing = float.Parse(str.Substring(0, i - 1));
                    bpm.BPM = float.Parse(str.Substring(i + 1, str.Length - i - 1));
                    //Debug.Log(bpm.Timing + ":" + bpm.BPM);
                    break;
                }
            }
        }

        return bpms;
    }

    /// <summary>
    /// 譜面を作るんじゃい
    /// </summary>
    /// <param name="sr">StringReader</param>
    public void MakeMusic(StringReader sr)
    {
        //元のデータね
        string data = sr.ReadToEnd();
        //1小節ごとに分けたデータ
        List<string> score_String = new List<string>();
        //これを入れていくよ
        string score_Bar = null;

        for(int i = 0; i < data.Length; i++)
        {
            string str = data.Substring(i, 1);

            if (str == "\n" || str == "\r")
            {
                continue;
            }

            if(str == ",")
            {
                score_String.Add(score_Bar);

                debug(score_Bar, score_String.Count);

                score_Bar = null;
                continue;
            }

            score_Bar += str;
        }
        
    }

    private void debug(string score_Bar, int count)
    {
        int th = score_Bar.Length / data_KEY;
        string tmp = null;
        string log = null;

        for (int a = 0; a < score_Bar.Length; a++)
        {
            tmp += score_Bar.Substring(a, 1);
            if (tmp.Length >= 4)
            {
                log += tmp + "\n";
                tmp = null;
            }
        }

        Debug.Log(count + ":" + th + "th\n\n" + log);
    }
}
