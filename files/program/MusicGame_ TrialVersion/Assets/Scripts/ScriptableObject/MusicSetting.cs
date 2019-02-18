using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MusicGame/Create MusicSettings")]
public class MusicSetting : ScriptableObject
{
    public string songTitle;
    public string songArtist;
    public float songOffset;

    public TextAsset[] musicSheet;
    public AudioClip musicClip;

    //個々のシートに入れるもの
    //譜面データ、BPM(速度変化情報)、難易度情報
}
