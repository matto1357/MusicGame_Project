using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicGame/Create MusicData", fileName = "_MusicData", order =0)]
[System.Serializable]
public class MusicData : ScriptableObject
{
    public List<SeetData> musicSeets = new List<SeetData>();
}

[System.Serializable]
public class SeetData
{
    public TextAsset seet;
    public NotesData notesData;
    public AudioClip clip;
}