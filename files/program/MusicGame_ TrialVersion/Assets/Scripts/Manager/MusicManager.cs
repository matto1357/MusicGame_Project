using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

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
    LONG, //複合
    LONG_START,
    LONG_END,
    MINE,
}

public enum MusicState
{
    Start = 0,
    Play,
    End,
}

public enum JudgeState
{
    none = -1,

    Great = 0,
    Good,
    Bad,
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

public class NotesInfo
{
    public int bar;
    public int th;
    public int barOriginTh;
    public int LNend_bar;
    public int LNend_th;
    public int LNend_barOriginTh;
    public ScoreIndex type;
}

/// <summary>
/// 譜面ファイルを読み込む
/// </summary>
[DefaultExecutionOrder(-1)]
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
    public List<STOPS> data_STOP = new List<STOPS>();
    [System.NonSerialized]
    public List<GIMMICKS> data_GIMMICK = new List<GIMMICKS>();
    [System.NonSerialized]
    public int data_MOVETYPE;
    [System.NonSerialized]
    public List<NotesInfo>[] data_MusicScore;
    [System.NonSerialized]
    public Queue<GameObject>[] noteObjects;
    [System.NonSerialized]
    public GameObject[] activeNotes;
    
    private Dictionary<ScoreIndex, string> NotesDictionary = new Dictionary<ScoreIndex, string>()
    {
        {ScoreIndex.none, "0"},
        {ScoreIndex.SIMPLE, "1" },
        {ScoreIndex.LONG_START, "2" },
        {ScoreIndex.LONG_END, "3" },
        {ScoreIndex.MINE, "M" },
    };

    public JudgeWidth judgeWidth;
    public Move move;
    public GameObject parentObj;
    public GameObject notesPrefab;
    public Text difftext;

    public float scoreLengthThumbnail;

    public AudioSource _audioSource;

    public float multi;

    private MusicState _musicState = MusicState.Start;
    
    /// <summary>
    /// 初期化関数
    /// </summary>
    private void Init()
    {
        data_TITLE = null;
        data_SUBTITLE = null;
        data_AUDIO = null;
        data_ARTIST = null;
        data_OFFSET = 0f;
        data_BPM = new List<BPMS>();
        data_KEY = 0;
        data_DIFFICULT = null;
        data_LEVEL = 0;
        data_STOP = new List<STOPS>();
        data_GIMMICK = new List<GIMMICKS>();
        data_MOVETYPE = 0;
    }

    /// <summary>
    /// data_MusicScoreを初期化する
    /// </summary>
    private void Init_data_MusicScore()
    {
        data_MusicScore = new List<NotesInfo>[data_KEY];
        for (int i = 0; i < data_MusicScore.Length; i++)
        {
            data_MusicScore[i] = new List<NotesInfo>();
        }
    }

    private void Init_noteObjects()
    {
        noteObjects = new Queue<GameObject>[data_KEY];
        for(int i = 0; i < noteObjects.Length; i++)
        {
            noteObjects[i] = new Queue<GameObject>();
        }
    }

    private void Init_ActiveNotes()
    {
        activeNotes = new GameObject[data_KEY];
    }

    private void StartMusic()
    {
        _audioSource.clip = data_AUDIO;
        _audioSource.Play();
    }

    private void Update()
    {
        bool input = Input.GetKeyDown(KeyCode.Space);
        if (_musicState == MusicState.Start && input)
        {
            _musicState = MusicState.Play;
            StartMusic();
        }
        DoNotNotes();
    }

    /// <summary>
    /// 譜面読み込み
    /// </summary>
    /// <param name="file">譜面ファイル</param>
    public void MusicLoad(TextAsset file)
    {
        Init();
        StringReader sr = new StringReader(file.text);

        string lines = null;
        MusicIndex index = MusicIndex.none;

        //ノーツ情報が出てくるまで1行ずつ読み込みを行う
        while (sr.Peek() != -1)
        {
            //行読み込み
            string line = sr.ReadLine();

            //譜面情報かどうか確認
            if (lines != null || (line.Length != 0 && line.Substring(0, 1) == "#"))
            {
                if (index == MusicIndex.none)
                {
                    index = CheckIndex(line);
                }

                //ノーツ情報が来たらBreak
                if (lines == null && index == MusicIndex.NOTES)
                {
                    break;
                }

                //タグが無かったら次の行へ
                if (lines == null && index == MusicIndex.none)
                {
                    continue;
                }

                lines += line;

                if (line.Substring(line.Length - 1, 1) == ";")
                {
                    IndexLoad(index, lines, file);
                    lines = null;
                    index = MusicIndex.none;
                }
            }
        }
        MakeMusic(sr);
        InputManager.instance.KeySetting();
        GenerateScore();
        Init_ActiveNotes();
        SetActiveNotesAll();
    }

    /// <summary>
    /// お前はどのタグだ
    /// </summary>
    /// <param name="line">行</param>
    /// <returns></returns>
    private MusicIndex CheckIndex(string line)
    {
        MusicIndex index = MusicIndex.none;
        string str = null;

        for (int i = 0; i < line.Length; i++)
        {
            if (line.Substring(i, 1) == ":")
            {
                str = line.Substring(1, i - 1);
                break;
            }
        }

        if (str == null)
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
    private void IndexLoad(MusicIndex index, string line, TextAsset file)
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
                if (contents.Substring(contents.Length - 1, 1) == "f")
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
    private List<BPMS> BreakContents_BPM(string contents)
    {
        List<BPMS> bpms = new List<BPMS>();
        List<string> breakContents = new List<string>();
        int index = 0;

        for (int i = 0; i < contents.Length; i++)
        {
            if (contents.Substring(i, 1) == ",")
            {
                breakContents.Add(contents.Substring(index, i - index));
                index = i + 1;
            }
        }

        breakContents.Add(contents.Substring(index, contents.Length - index));

        foreach (string str in breakContents)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str.Substring(i, 1) == "=")
                {
                    BPMS bpm = new BPMS();
                    bpm.Timing = float.Parse(str.Substring(0, i - 1));
                    bpm.BPM = float.Parse(str.Substring(i + 1, str.Length - i - 1));
                    bpms.Add(bpm);
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
    private void MakeMusic(StringReader sr)
    {
        //初期化
        Init_data_MusicScore();
        //元のデータね
        string data = sr.ReadToEnd();
        //1小節ごとに分けたデータ
        List<string> score_String = new List<string>();
        //これを入れていくよ
        string score_Bar = null;

        for (int i = 0; i < data.Length; i++)
        {
            string str = data.Substring(i, 1);

            if (str == "\n" || str == "\r")
            {
                continue;
            }

            if (str == "," || str == ";")
            {
                score_String.Add(score_Bar);
                score_Bar = null;
                continue;
            }

            score_Bar += str;
        }

        //譜面入れてくよ
        for(int bar = 0; bar < score_String.Count; bar++)
        {
            //--1小節ごと
            string barStr = score_String[bar];
            int barOriginTh = barStr.Length / data_KEY;

            for (int num = 0; num < barStr.Length; num++)
            {
                //--1文字ごと
                string score = barStr.Substring(num, 1);
                int key = num % data_KEY;
                int th = (num - key) / data_KEY;
                foreach(KeyValuePair<ScoreIndex, string> pair in NotesDictionary)
                {
                    if(score == pair.Value)
                    {
                        ScoreIndex index = pair.Key;

                        if(index == ScoreIndex.none)
                        {
                            break;
                        }

                        if(index == ScoreIndex.MINE)
                        {
                            break;
                        }

                        if(index == ScoreIndex.LONG_END)
                        {
                            data_MusicScore[key][data_MusicScore[key].Count - 1].type = ScoreIndex.LONG;
                            data_MusicScore[key][data_MusicScore[key].Count - 1].LNend_bar = bar;
                            data_MusicScore[key][data_MusicScore[key].Count - 1].LNend_th = th;
                            data_MusicScore[key][data_MusicScore[key].Count - 1].LNend_barOriginTh = barOriginTh;
                            break;
                        }

                        NotesInfo note = new NotesInfo();

                        note.bar = bar;
                        note.th = th;
                        note.barOriginTh = barOriginTh;
                        note.type = index;

                        data_MusicScore[key].Add(note);

                        //Debug.Log(note.type.ToString() + ":" + (note.bar + 1) + "小節目:" + (note.th + 1) + "分目");
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 譜面をDebugLogに出力
    /// </summary>
    /// <param name="score_Bar">1小節の譜面</param>
    /// <param name="count">何小節目か</param>
    private void Debug_MusicScore(string score_Bar, int count)
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

    /// <summary>
    /// 譜面を生成するよ
    /// </summary>
    private void GenerateScore()
    {
        Init_noteObjects();

        float offset = data_OFFSET;
        //変速対応はまだ
        float bpm = data_BPM[0].BPM;
        float scoreLengthPerBar = bpm / scoreLengthThumbnail;
        float[] xPos = { -7.0f, -5.0f, -3.0f, -1.0f };
        for(int x = 0; x < data_MusicScore.Length; x++)
        {
            for(int y = 0; y < data_MusicScore[x].Count; y++)
            {
                GameObject obj = Instantiate(notesPrefab);

                float noteBarPos = (float)data_MusicScore[x][y].bar + (float)data_MusicScore[x][y].th / (float)data_MusicScore[x][y].barOriginTh;

                NotesScript note = obj.AddComponent<NotesScript>();

                note.notesTiming = 240f / data_BPM[0].BPM * noteBarPos;
                note.type = data_MusicScore[x][y].type;
                obj.transform.SetParent(parentObj.transform);
                float yPos = scoreLengthPerBar * noteBarPos;
                obj.transform.localPosition = new Vector3(xPos[x], yPos * multi, 0f);

                if (data_MusicScore[x][y].type == ScoreIndex.LONG)
                {
                    GameObject obj2 = Instantiate(notesPrefab);
                    noteBarPos = (float)data_MusicScore[x][y].LNend_bar + (float)data_MusicScore[x][y].LNend_th / (float)data_MusicScore[x][y].LNend_barOriginTh;

                    note.LNendObj = obj2;
                    note.lr = obj.AddComponent<LineRenderer>();

                    note = obj2.AddComponent<NotesScript>();

                    note.type = ScoreIndex.LONG_END;
                    note.notesTiming = 240f / data_BPM[0].BPM * noteBarPos;
                    obj2.transform.SetParent(parentObj.transform);
                    yPos = scoreLengthPerBar * noteBarPos;
                    obj2.transform.localPosition = new Vector3(xPos[x], yPos * multi, 0f);

                    obj2.transform.SetParent(obj.transform);
                }

                noteObjects[x].Enqueue(obj);
            }
        }
    }

    //セットするよ
    public void SetActiveNotes(int laneNum, JudgeState judge = JudgeState.none)
    {
        bool LongEnd = false;

        if (activeNotes[laneNum] != null)
        {
            NotesScript noteInfo = activeNotes[laneNum].GetComponent<NotesScript>();

            ScoreIndex noteType = noteInfo.type;

            switch (noteType)
            {
                case ScoreIndex.SIMPLE:
                    Destroy(activeNotes[laneNum]);
                    break;
                case ScoreIndex.LONG:
                    if(judge == JudgeState.Bad)
                    {
                        Destroy(activeNotes[laneNum]);
                    }
                    else
                    {
                        LongEnd = true;
                    }
                    break;
                case ScoreIndex.LONG_END:
                    Destroy(activeNotes[laneNum].transform.parent.gameObject);
                    break;
            }
        }

        if(LongEnd)
        {
            activeNotes[laneNum] = activeNotes[laneNum].GetComponent<NotesScript>().LNendObj;
            return;
        }

        if (noteObjects[laneNum].Count != 0)
        {
            activeNotes[laneNum] = noteObjects[laneNum].Dequeue();
        }
        else
        {
            activeNotes[laneNum] = null;
        }
    }

    private void SetActiveNotesAll()
    {
        for(int i = 0; i < data_KEY; i++)
        {
            SetActiveNotes(i);
        }
    }

    public void InputKey(int laneNum, bool keyState)
    {
        if (activeNotes[laneNum] == null)
        {
            return;
        }

        NotesScript noteInfo = activeNotes[laneNum].GetComponent<NotesScript>();

        ScoreIndex type = noteInfo.type;
        if(keyState == false && type != ScoreIndex.LONG_END)
        {
            return;
        }
        
        float diff = noteInfo.notesTiming - move.time;

        float fixDiff = Mathf.Abs(diff);

        JudgeState state = JudgeState.none;

        //判定部分
        if (judgeWidth.JudgeTimings[judgeWidth.JudgeTimings.Length - 1] >= fixDiff)
        {
            for (int i = 0; i < judgeWidth.JudgeTimings.Length; i++)
            {
                if (judgeWidth.JudgeTimings[i] >= fixDiff)
                {
                    switch (i)
                    {
                        case 0:
                            state = JudgeState.Great;
                            break;
                        case 1:
                            state = JudgeState.Good;
                            break;
                        case 2:
                            state = JudgeState.Bad;
                            break;
                    }
                    break;
                }
            }   
        }

        switch (type)
        {
            case ScoreIndex.SIMPLE:
                if (state == JudgeState.none)
                {
                    break;
                }
                ScoreManager.instance.AddJudge(state);
                SetActiveNotes(laneNum, state);
                break;
            case ScoreIndex.LONG:
                if (state == JudgeState.none)
                {
                    break;
                }
                ScoreManager.instance.AddJudge(state);
                SetActiveNotes(laneNum, state);
                break;
            case ScoreIndex.LONG_END:
                if (state == JudgeState.none)
                {
                    state = JudgeState.Bad;
                }
                ScoreManager.instance.AddJudge(state);
                SetActiveNotes(laneNum, state);
                break;
        }

        difftext.text = diff.ToString("f3");
    }

    private void DoNotNotes()
    {
        for(int i = 0; i < activeNotes.Length; i ++)
        {
            if(activeNotes[i] == null)
            {
                continue;
            }
            float diff = activeNotes[i].GetComponent<NotesScript>().notesTiming
            - move.time;
            if(diff < (judgeWidth.JudgeTimings[judgeWidth.JudgeTimings.Length - 1] * -1.0f))
            {
                ScoreManager.instance.AddJudge(JudgeState.Bad);
                SetActiveNotes(i, JudgeState.Bad);
            }
        }
    }
}
