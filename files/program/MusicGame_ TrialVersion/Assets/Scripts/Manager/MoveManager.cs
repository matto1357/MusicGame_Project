using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : SingletonMonoBehaviour<MoveManager>
{
    private GameObject parentObj;
    public AudioSource _audioSource;

    [System.NonSerialized]
    public float time;

    private float originYPos;
    private float movePerSec = 1.0f / (240f / 100f);

    private BPMS currentBPM;
    private STOPS currentSTOP;

    private int nextNum_BPM = 0;
    private int nextNum_Stop = 0;

    private void Start()
    {
        parentObj = MusicManager.instance.parentObj;
        originYPos = parentObj.transform.position.y;
        currentBPM = SetBPMS();
        currentSTOP = SetStops();
    }

    private void FixedUpdate()
    {
        time = _audioSource.time;
        float fixTime = time;
        
        if(time >= currentSTOP.stopTiming_Time)
        {
            time = currentSTOP.stopTiming_Time - currentSTOP.totalStopTime;
        }
        else
        {
            time -= currentSTOP.totalStopTime;
        }

        Vector3 pos = parentObj.transform.position;
        pos.y = originYPos -
            ((time - currentBPM.currentTime) * movePerSec * currentBPM.correctionNum + currentBPM.currentLength_Total) * 
            MusicManager.instance.multi;
        parentObj.transform.position = pos;

        if(MusicManager.instance.data_BPM.Count > nextNum_BPM &&
            time >= MusicManager.instance.data_BPM[nextNum_BPM].currentTime)
        {
            currentBPM = SetBPMS();
        }

        if(MusicManager.instance.data_STOP.Count > nextNum_Stop &&
            _audioSource.time >= currentSTOP.stopTiming_AfterTime)
        {
            currentSTOP = SetStops();
        }
    }

    private BPMS SetBPMS()
    {
        BPMS data = MusicManager.instance.data_BPM[nextNum_BPM];
        movePerSec = data.BPM / 100f / (240f / data.BPM);
        Debug.Log(GetTime("sec") + ":SetBPMS:" + data.BPM);
        nextNum_BPM++;
        return data;
    }

    private STOPS SetStops()
    {
        STOPS data = MusicManager.instance.data_STOP[nextNum_Stop];
        Debug.Log(GetTime("sec") + ":SetStops:" + data.stopTiming_Time);
        nextNum_Stop++;
        return data;
    }

    private string GetTime(string addText = "")
    {
        float time = Mathf.Round(_audioSource.time * 100) / 100;
        string str = time.ToString() + addText;
        return str;
    }
}
