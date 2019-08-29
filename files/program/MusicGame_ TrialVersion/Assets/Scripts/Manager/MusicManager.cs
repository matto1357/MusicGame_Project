using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 譜面読み込みのモード
/// </summary>
public enum LoadMode
{
    Normal = 0,
    PreLoad,
}

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
    GAMETYPE,
    MOVETYPE,
    GAUGETYPE,
}

/// <summary>
/// レーンのレイアウトとか譜面の流れ方とか
/// </summary>
public enum GameType
{
    Basic = 0,     //上から下に流れるタイプ
    Step,          //下から上に流れるタイプ(DDR・ギター)
    Stage,         //スクフェス的なアレ
    Circle,        //CytusやOsu的なアレ
}

/// <summary>
/// ノーツの動き方
/// </summary>
public enum MoveType
{
    Normal = 0,     //通常タイプ
    WildlyScroll,   //追い越し可能なタイプ(太鼓の追い越しのイメージ)
}

/// <summary>
/// クリアに必要な要素
/// </summary>
public enum GaugeType
{
    AchievementRate = 0,    //スコアの達成率部分で評価するタイプ(Rb・maimai)
    GrooveGauge,            //曲終了時にゲージが一定以上でクリア(Bm・Pm)
    LifeGauge,              //曲終了までゲージを残せばクリア(DDR,Bmハードゲージ)
}


/// <summary>
/// ノーツ種類
/// </summary>
public enum ScoreIndex
{
    none = -1,

    SIMPLE = 0,
    LONG, //複合
    LONG_START,
    LONG_END,
    MINE,
}

/// <summary>
/// ロングノーツのタイプ
/// </summary>
public enum LNType
{
    Release = 0, //終点で離す
    Keep,
}

/// <summary>
/// 音楽の状態
/// </summary>
public enum MusicState
{
    Start = 0,
    Play,
    End,
}

/// <summary>
/// 判定
/// </summary>
public enum JudgeState
{
    AUTO = -10,
    none = -1,

    Great = 0,
    Good,
    Bad,
}

/// <summary>
/// ハイスピードオプション
/// </summary>
public enum SpeedOption
{
    TopSpeed,
    MaxFit,
    MinFit,
}

/// <summary>
/// BPM管理クラス
/// </summary>
[System.Serializable]
public class BPMS
{
    public float BPM = 0.0f;
    public int   changeTiming_Bar = 0;
    public int   changeTiming_Th = 0;
    public int   changeTiming_OriginTh = 0;
    public float changeTiming = 0.0f;
    public float scoreLengthPerBar = 0.0f;
    public float currentLength_Total = 0.0f;
    public float currentTime = 0.0f;
    public float correctionNum = 1.0f;
}

/// <summary>
/// 譜面停止管理クラス
/// </summary>
[System.Serializable]
public class STOPS
{
    public int   stopTiming_Bar = 0;
    public int   stopTiming_Th = 0;
    public int   stopTiming_OriginTh = 0;
    public float stopTiming = 0.0f;
    public float stopTiming_Time = 0.0f;
    public float stopTiming_AfterTime = 0.0f;
    public float stopTime = 0.0f;
    public float totalStopTime = 0.0f;
}

/// <summary>
/// その他ギミック管理クラス
/// </summary>
public class GIMMICKS
{

}

/// <summary>
/// ノーツ単体の情報管理クラス
/// </summary>
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
/// ノーツ数の情報
/// </summary>
[System.Serializable]
public class NotesData
{
    public int SimpleNote;
    public int LongNote;
    public int MineNote;
    public int TotalNotes;
}

/// <summary>
/// 譜面ファイルを読み込んで、ベース情報を作成
/// </summary>
[DefaultExecutionOrder(-1)]
public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    //読み込んだ譜面の情報を格納する変数群

    /// <summary>
    /// 曲タイトル
    /// </summary>
    [System.NonSerialized]
    public string data_TITLE;
    /// <summary>
    /// サブタイトル
    /// </summary>
    [System.NonSerialized]
    public string data_SUBTITLE;
    /// <summary>
    /// 音声ファイル
    /// </summary>
    //[System.NonSerialized]
    public AudioClip data_AUDIO;
    /// <summary>
    /// アーティスト名
    /// </summary>
    [System.NonSerialized]
    public string data_ARTIST;
    /// <summary>
    /// 曲オフセット
    /// </summary>
    [System.NonSerialized]
    public float data_OFFSET;
    /// <summary>
    /// BPMデータ
    /// </summary>
    [System.NonSerialized]
    public List<BPMS> data_BPM = new List<BPMS>();
    /// <summary>
    /// レーン数
    /// </summary>
    [System.NonSerialized]
    public int data_KEY;
    /// <summary>
    /// 難易度(文字)
    /// </summary>
    [System.NonSerialized]
    public string data_DIFFICULT;
    /// <summary>
    /// 難易度(数字)
    /// </summary>
    [System.NonSerialized]
    public int data_LEVEL;
    /// <summary>
    /// 譜面停止ギミック
    /// </summary>
    [System.NonSerialized]
    public List<STOPS> data_STOP = new List<STOPS>();
    /// <summary>
    /// その他ギミック
    /// </summary>
    [System.NonSerialized]
    public List<GIMMICKS> data_GIMMICK = new List<GIMMICKS>();
    //[System.NonSerialized]
    //public int data_
    /// <summary>
    /// 譜面の流れ方
    /// </summary>
    [System.NonSerialized]
    public MoveType data_MOVETYPE;
    /// <summary>
    /// レーン毎に分けたノーツデータ
    /// </summary>
    [System.NonSerialized]
    public List<NotesInfo>[] data_MusicScore;
    /// <summary>
    /// レーン毎に分けたノーツオブジェクト
    /// </summary>
    [System.NonSerialized]
    public Queue<GameObject>[] noteObjects;

    //ノーツ周り

    /// <summary>
    /// 次に来るノーツ
    /// </summary>
    [System.NonSerialized]
    public GameObject[] activeNotes;

    /// <summary>
    /// ノーツ辞典(ノーツの種類,ノーツ文字)
    /// </summary>    
    private Dictionary<ScoreIndex, string> NotesDictionary = new Dictionary<ScoreIndex, string>()
    {
        {ScoreIndex.none, "0"},
        {ScoreIndex.SIMPLE, "1" },
        {ScoreIndex.LONG_START, "2" },
        {ScoreIndex.LONG_END, "3" },
        {ScoreIndex.MINE, "M" },
    };

    /// <summary>
    /// ノーツ数
    /// </summary>
    public NotesData notesData = new NotesData();

    public MoveManager move;
    public GameObject parentObj;
    public GameObject notesPrefab;
    private float scoreLengthThumbnail = 100.0f;
    public LNType LNType;

    //判定周り

    public JudgeWidth judgeWidth;

    //サウンド周り

    public AudioSource _audioSource;
    public AudioClip clap;

    //ハイスピード機能周り

    /// <summary>
    /// ハイスピの値をここで指定
    /// </summary>
    public float HS;
    /// <summary>
    /// ハイスピの合わせ方
    /// </summary>
    public SpeedOption option;
    [System.NonSerialized]
    public float multi;  
    private float speed;
    private float maxBPM = 0.0f;
    private float minBPM = 0.0f;
    private float standardBPM = 0.0f;

    //その他

    private MusicState _musicState = MusicState.Start;
    
    //初期化関数

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

        notesData = new NotesData();
    }

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

    /// <summary>
    /// 音楽を再生する
    /// </summary>
    private void StartMusic()
    {
        _audioSource.clip = data_AUDIO;
        _audioSource.time = 0f;
        _audioSource.Play();
    }

    private void Update()
    {
        //スペースキーを押したら音楽を再生する
        bool input = Input.GetKeyDown(KeyCode.Space);
        if (_musicState == MusicState.Start && input)
        {
            _musicState = MusicState.Play;
            StartMusic();
        }

        //見逃し判定の実行とオートプレイの実行
        switch(GameManager.instance.mode)
        {
            case PlayMode.Normal:
                DoNotNotes();
                break;
            case PlayMode.Auto:
                AutoPlay();
                break;
        }
    }

    /// <summary>
    /// 本編用読み込み
    /// </summary>
    public void MusicLoad(SeetData data)
    {
        MusicLoad(data.seet);
        data_AUDIO = data.clip;
    }

    /// <summary>
    /// 譜面読み込み
    /// </summary>
    /// <param name="file">譜面ファイル</param>
    public void MusicLoad(TextAsset file, LoadMode mode = LoadMode.Normal)
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
                    IndexLoad(index, lines, file, mode);
                    lines = null;
                    index = MusicIndex.none;
                }
            }
        }
        SpeedFit();
        MakeMusic(sr, mode);
        notesData.TotalNotes = notesData.SimpleNote + notesData.LongNote * 2;
        if (mode != LoadMode.Normal)
        {
            return;
        }
        InputManager.instance.KeySetting();
        GenerateScore();
        InitActiveNotes();
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
                //タグを抽出
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
    private void IndexLoad(MusicIndex index, string line, TextAsset file, LoadMode mode)
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
                if(mode == LoadMode.Normal)
                {
                    break;
                }
                #if UNITY_EDITOR
                string fileDirectory = AssetDatabase.GetAssetPath(file);
                string folderDirectory = Path.GetDirectoryName(fileDirectory);
                string tgtDirectory = folderDirectory + "/" + contents;
                data_AUDIO = (AudioClip)AssetDatabase.LoadAssetAtPath(tgtDirectory, typeof(AudioClip));
                #endif
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
                data_STOP = BreakContents_STOP(contents);
                break;
            case MusicIndex.GIMMICK:
                //実装
                break;
            case MusicIndex.MOVETYPE:
                data_MOVETYPE = (MoveType)Enum.ToObject(typeof(MoveType), int.Parse(contents));
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
            if(str.Contains("=") == false)
            {
                break;
            }

            int equal = str.IndexOf("=");
            BPMS bpm = new BPMS();
            float currentBpm = float.Parse(str.Substring(equal + 1, str.Length - equal - 1));
            bpm.BPM = currentBpm;
            bpm.scoreLengthPerBar = currentBpm / scoreLengthThumbnail;
            string timing = str.Substring(0, equal);
            
            if(timing.Contains("-"))
            {
                int pos = timing.IndexOf("-");
                bpm.changeTiming_Bar = int.Parse(timing.Substring(0, pos));
                bpm.changeTiming_Th = int.Parse(timing.Substring(pos + 1, timing.Length - pos - 1));
            }
            else
            {
                bpm.changeTiming_Bar = int.Parse(timing);
            }
            bpms.Add(bpm);

            if(minBPM == 0.0f)
            {
                minBPM = bpm.BPM;
            }
            else if(minBPM > bpm.BPM)
            {
                minBPM = bpm.BPM;
            }

            if(maxBPM == 0.0f)
            {
                maxBPM = bpm.BPM;
            }
            else if(maxBPM < bpm.BPM)
            {
                maxBPM = bpm.BPM;
            }
        }
        return bpms;
    }

    private List<STOPS> BreakContents_STOP(string contents)
    {
        List<STOPS> stops = new List<STOPS>();
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
            if (str.Contains("=") == false)
            {
                break;
            }

            int equal = str.IndexOf("=");
            STOPS stop = new STOPS();
            float stopTime = float.Parse(str.Substring(equal + 1, str.Length - equal - 1));

            stop.stopTime = stopTime;

            string timing = str.Substring(0, equal);

            if (timing.Contains("-"))
            {
                int pos = timing.IndexOf("-");
                stop.stopTiming_Bar = int.Parse(timing.Substring(0, pos));
                stop.stopTiming_Th = int.Parse(timing.Substring(pos + 1, timing.Length - pos - 1));
            }
            else
            {
                stop.stopTiming_Bar = int.Parse(timing);
            }
            stops.Add(stop);
            Debug.Log(stop.stopTiming_Bar + ":" + stop.stopTiming_Th + "=" + stop.stopTime);
        }
        return stops;
    }

    private void SpeedFit()
    {
        speed = HS * 100000;
        switch (option)
        {
            case SpeedOption.TopSpeed:
                standardBPM = data_BPM[0].BPM;
                break;
            case SpeedOption.MaxFit:
                standardBPM = maxBPM;
                break;
            case SpeedOption.MinFit:
                standardBPM = minBPM;
                break;
        }
        multi = speed / standardBPM;
    }

    /// <summary>
    /// 譜面を作るんじゃい
    /// </summary>
    /// <param name="sr">StringReader</param>
    private void MakeMusic(StringReader sr, LoadMode mode = LoadMode.Normal)
    {
        //初期化
        Init_data_MusicScore();
        //元のデータね
        string data = sr.ReadToEnd();
        //1小節ごとに分けたデータ
        List<string> score_String = new List<string>();
        //これを入れていくよ
        string score_Bar = null;
        List<int> originThs = new List<int>();
        bool isCommentOut = false;

        for (int i = 0; i < data.Length; i++)
        {
            string str = data.Substring(i, 1);

            if (str == "\n" || str == "\r")
            {
                isCommentOut = false;
                continue;
            }

            if (isCommentOut)
            {
                continue;
            }

            if (str == " ")
            {
                continue;
            }

            if (str == "/")
            {
                if (data.Substring(i + 1, 1) == "/")
                {
                    isCommentOut = true;
                }
                continue;
            }

            if (str == "," || str == ";")
            {
                if(score_Bar != null)
                {
                    score_String.Add(score_Bar);
                    score_Bar = null;
                }
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
            originThs.Add(barOriginTh);

            Debug.Log(bar + ":" + bar * 4 + "-" + (bar * 4 + 3) + "=" + barOriginTh + "th");

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

                        //----- 無視処理 -----//
                        if(index == ScoreIndex.none)
                        {
                            break;
                        }

                        if(index == ScoreIndex.MINE)
                        {
                            notesData.MineNote++;
                            break;
                        }

                        //----- サブ処理 -----//

                        if (index == ScoreIndex.SIMPLE)
                        {
                            notesData.SimpleNote++;
                        }
                        else if(index == ScoreIndex.LONG_START)
                        {
                            notesData.LongNote++;
                        }

                        //----- 分岐 -----//

                        if (mode != LoadMode.Normal)
                        {
                            break;
                        }

                        //----- メイン処理 -----//

                        if (index == ScoreIndex.LONG_END)
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
        BPMData_Add(originThs);
        STOPData_Add(originThs);
    }

    private void BPMData_Add(List<int> data)
    {
        for(int i = 0; i < data_BPM.Count; i++)
        {
            data_BPM[i].changeTiming_OriginTh = data[data_BPM[i].changeTiming_Bar];
            data_BPM[i].changeTiming = data_BPM[i].changeTiming_Bar +
                ((float)data_BPM[i].changeTiming_Th / data_BPM[i].changeTiming_OriginTh);

            data_BPM[i].correctionNum = standardBPM / data_BPM[i].BPM / data_BPM[i].BPM / (standardBPM / data_BPM[i].BPM);

            if (i == 0)
            {
                continue;
            }

            float afterPos = data_BPM[i].changeTiming;
            float beforePos = data_BPM[i - 1].changeTiming;

            float totalLength = data_BPM[i - 1].scoreLengthPerBar * (afterPos - beforePos) * data_BPM[i - 1].correctionNum;

            data_BPM[i].currentLength_Total = totalLength + data_BPM[i - 1].currentLength_Total;

            float currentTime = data_BPM[i - 1].currentTime +
                240f / data_BPM[i - 1].BPM * (afterPos - beforePos);
            data_BPM[i].currentTime = currentTime;
        }

    }

    private void STOPData_Add(List<int> data)
    {
        for (int i = 0; i < data_STOP.Count; i++)
        {
            data_STOP[i].stopTiming_OriginTh = data[data_STOP[i].stopTiming_Bar];
            data_STOP[i].stopTiming = data_STOP[i].stopTiming_Bar + 
                ((float)data_STOP[i].stopTiming_Th / data_STOP[i].stopTiming_OriginTh);

            NotesInfo info = new NotesInfo();

            info.bar = data_STOP[i].stopTiming_Bar;
            info.th = data_STOP[i].stopTiming_Th;
            info.barOriginTh = data_STOP[i].stopTiming_OriginTh;

            BPMS bpm = GetBPM(info);

            float afterPos = data_STOP[i].stopTiming;
            float beforePos = bpm.changeTiming;

            data_STOP[i].stopTiming_Time = bpm.currentTime +
                240f / bpm.BPM * (afterPos - beforePos);

            data_STOP[i].stopTiming_AfterTime = data_STOP[i].stopTiming_Time + data_STOP[i].stopTime;

            if (i == 0)
            {
                continue;
            }

            data_STOP[i].totalStopTime = data_STOP[i - 1].stopTime + data_STOP[i - 1].totalStopTime;
            data_STOP[i].stopTiming_Time += data_STOP[i].totalStopTime;
            data_STOP[i].stopTiming_AfterTime += data_STOP[i].totalStopTime;
        }

        STOPS addData = new STOPS();
        addData.stopTiming_Time = float.MaxValue;
        if (data_STOP.Count == 0)
        {
            data_STOP.Add(addData);
            return;
        }
        STOPS basestop = data_STOP[data_STOP.Count - 1];
        addData.totalStopTime = basestop.totalStopTime + basestop.stopTime;
        data_STOP.Add(addData);
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
        if(data_BPM.Count == 0)
        {
            Debug.LogWarning("BPM情報が見つかりませんでした");
            return;
        }

        Init_noteObjects();

        float offset = data_OFFSET;
        float[] xPos = { -7.0f, -5.0f, -3.0f, -1.0f };
        for(int x = 0; x < data_MusicScore.Length; x++)
        {
            for(int y = 0; y < data_MusicScore[x].Count; y++)
            {
                NotesInfo info = data_MusicScore[x][y];
                BPMS bpm = GetBPM(info);
                STOPS stop = GetStop(info);

                GameObject obj = Instantiate(notesPrefab);
                float noteBarPos = (float)info.bar + (float)info.th / (float)info.barOriginTh;

                NotesScript note = obj.AddComponent<NotesScript>();
                note.stopdata = stop;

                note.notesTiming = bpm.currentTime + 240f / bpm.BPM * (noteBarPos - bpm.changeTiming);
                note.type = info.type;
                obj.transform.SetParent(parentObj.transform);

                float yPos = bpm.currentLength_Total + (noteBarPos - bpm.changeTiming) * bpm.scoreLengthPerBar * bpm.correctionNum;
                
                obj.transform.localPosition = new Vector3(xPos[x], yPos * multi, 0f);

                if (info.type == ScoreIndex.LONG)
                {
                    GameObject obj2 = Instantiate(notesPrefab);
                    noteBarPos = (float)info.LNend_bar + (float)info.LNend_th / (float)info.LNend_barOriginTh;

                    note.LNendObj = obj2;
                    note.lr = obj.AddComponent<LineRenderer>();
                    note = obj2.AddComponent<NotesScript>();

                    note.type = ScoreIndex.LONG_END;
                    note.notesTiming = bpm.currentTime + 240f / bpm.BPM * (noteBarPos - bpm.changeTiming);
                    obj2.transform.SetParent(parentObj.transform);
                    yPos = bpm.currentLength_Total + (noteBarPos - bpm.changeTiming) * bpm.scoreLengthPerBar * bpm.correctionNum;

                    obj2.transform.localPosition = new Vector3(xPos[x], yPos * multi, 0f);
                    obj2.transform.SetParent(obj.transform);
                }

                noteObjects[x].Enqueue(obj);
            }
        }
    }

    private BPMS GetBPM(NotesInfo info, bool isLN = false)
    {
        BPMS bpm = null;

        float pos = info.bar + ((float)info.th / info.barOriginTh);

        if(isLN)
        {
            pos = info.LNend_bar + (info.LNend_th / info.LNend_barOriginTh);
        }

        for(int i = 0; i < data_BPM.Count; i++)
        {
            if(pos < data_BPM[i].changeTiming)
            {
                if(i == 0)
                {
                    bpm = data_BPM[i];
                }
                else
                {
                    bpm = data_BPM[i - 1];
                }
                break;
            }
        }
        if(bpm == null)
        {
            bpm = data_BPM[data_BPM.Count - 1];
        }

        return bpm;
    }

    private STOPS GetStop(NotesInfo info, bool isLN = false)
    {
        STOPS stop = null;

        float pos = info.bar + ((float)info.th / info.barOriginTh);

        if (isLN)
        {
            pos = info.LNend_bar + (info.LNend_th / info.LNend_barOriginTh);
        }

        for (int i = 0; i < data_STOP.Count; i++)
        {
            if(pos <= data_STOP[i].stopTiming)
            {
                if(i == 0)
                {
                    stop = data_STOP[i];
                }
                else
                {
                    stop = data_STOP[i/* - 1*/];
                }
                break;
            }
            stop = data_STOP[i];
        }
        if (stop == null)
        {
            stop = data_STOP[data_STOP.Count - 1];
        }

        return stop;
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

    private void InitActiveNotes()
    {
        activeNotes = new GameObject[data_KEY];
        for (int i = 0; i < data_KEY; i++)
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
                ScoreManager.instance.AddJudge(state, noteInfo, diff);
                SetActiveNotes(laneNum, state);
                break;
            case ScoreIndex.LONG:
                if (state == JudgeState.none)
                {
                    break;
                }
                ScoreManager.instance.AddJudge(state, noteInfo, diff);
                SetActiveNotes(laneNum, state);
                break;
            case ScoreIndex.LONG_END:
                if (state == JudgeState.none)
                {
                    state = JudgeState.Bad;
                }
                ScoreManager.instance.AddJudge(state, noteInfo, diff);
                SetActiveNotes(laneNum, state);
                break;
        }
    }

    private void DoNotNotes()
    {
        if(activeNotes == null)
        {
            return;
        }

        for(int i = 0; i < activeNotes.Length; i ++)
        {
            if(activeNotes[i] == null)
            {
                continue;
            }
            NotesScript note = activeNotes[i].GetComponent<NotesScript>();
            float diff = note.notesTiming - move.time;
            if(LNType == LNType.Keep && note.type == ScoreIndex.LONG_END && diff <= 0f)
            {
                ScoreManager.instance.AddJudge(JudgeState.Great, note, diff);
                SetActiveNotes(i, JudgeState.Great);
            }
            else if(diff < (judgeWidth.JudgeTimings[judgeWidth.JudgeTimings.Length - 1] * -1.0f))
            {
                ScoreManager.instance.AddJudge(JudgeState.Bad, note, diff);
                SetActiveNotes(i, JudgeState.Bad);
            }
        }
    }

    private void AutoPlay()
    {
        if (activeNotes == null)
        {
            return;
        }

        for (int i = 0; i < activeNotes.Length; i++)
        {
            if (activeNotes[i] == null)
            {
                continue;
            }
            float diff = activeNotes[i].GetComponent<NotesScript>().notesTiming
            - move.time;
            if (diff <= 0.0f)
            {
                NotesScript note = activeNotes[i].GetComponent<NotesScript>();
                ScoreManager.instance.AddJudge(JudgeState.AUTO, note);
                SetActiveNotes(i, JudgeState.Great);
                _audioSource.PlayOneShot(clap);
            }
        }
    }
}