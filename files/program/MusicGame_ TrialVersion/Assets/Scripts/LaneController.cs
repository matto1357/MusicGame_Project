using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// レーン毎に付けて、入力を受けたら判定を行う
/// </summary>
public class LaneController : MonoBehaviour
{
    public KeyCode laneKey;

    public GameObject[] laneNotes;

    private void Update()
    {
        if (Input.GetKeyDown(laneKey) || NotesManager.instance.notesActive && Input.GetKeyDown(laneKey))
        {
            //ここレイキャスト出来ないかね
            GameObject targetNote = null;
            foreach(GameObject note in laneNotes)
            {
                if(targetNote == null)
                {
                    targetNote = note;
                }
                else
                {
                    if(targetNote.transform.position.y > note.transform.position.y)
                    {
                        targetNote = note;
                    }
                }
            }
            NoteJudge(targetNote);
        }
    }

    private void NoteJudge(GameObject note)
    {
        float diff = Mathf.Abs(NotesManager.instance.judgeLinePos.y - note.transform.position.y);
        Debug.Log(diff);
    }
}
