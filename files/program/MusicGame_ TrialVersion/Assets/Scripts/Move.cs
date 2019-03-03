using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public GameObject parentObj;

    private float time;

    private float originYPos;
    private float movePerSec = 2.1f / (240f / 210f);

    private void Start()
    {
        originYPos = parentObj.transform.position.y;
        Debug.Log(movePerSec);
    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        Vector3 pos = parentObj.transform.position;
        pos.y = originYPos - time * movePerSec * MusicManager.instance.multi;
        parentObj.transform.position = pos;
    }
}
