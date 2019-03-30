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

    private int nextNum = 0;

    private void Start()
    {
        parentObj = MusicManager.instance.parentObj;
        originYPos = parentObj.transform.position.y;
        currentBPM = SetBPMS();
    }

    private void FixedUpdate()
    {
        time = _audioSource.time;
        Vector3 pos = parentObj.transform.position;
        pos.y = originYPos -
            ((time - currentBPM.currentTime) * movePerSec + 
            currentBPM.currentLength_Total) * 
            MusicManager.instance.multi;
        parentObj.transform.position = pos;

        if(MusicManager.instance.data_BPM.Count > nextNum && time >= MusicManager.instance.data_BPM[nextNum].currentTime)
        {
            currentBPM = SetBPMS();
        }
    }

    private BPMS SetBPMS()
    {
        BPMS data = MusicManager.instance.data_BPM[nextNum];
        movePerSec = data.BPM / 100f / (240f / data.BPM);
        //Debug.Log(data.BPM);
        nextNum++;
        return data;
    }
}
