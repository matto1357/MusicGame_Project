using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesScript : MonoBehaviour
{
    public ScoreIndex type;
    public float notesTiming;
    [System.NonSerialized]
    public GameObject LNendObj;
    [System.NonSerialized]
    public LineRenderer lr;

    private void Update()
    {
        if(lr != null)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, LNendObj.transform.position);
        }
    }
}
