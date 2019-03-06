using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    //レーン毎のデリゲート
    public delegate void Lane(int num);

    public Lane lane1;
    public Lane lane2;
    public Lane lane3;
    public Lane lane4;

    //キー設定、後に設定可能にする
    private KeyCode key_Lane1 = KeyCode.Z;
    private KeyCode key_Lane2 = KeyCode.X;
    private KeyCode key_Lane3 = KeyCode.C;
    private KeyCode key_Lane4 = KeyCode.V;

    private void Start()
    {
        lane1 += MusicManager.instance.InputKey;
        lane2 += MusicManager.instance.InputKey;
        lane3 += MusicManager.instance.InputKey;
        lane4 += MusicManager.instance.InputKey;
    }

    private void Update()
    {
        if (Input.GetKeyDown(key_Lane1))
        {
            lane1(0);
        }
        if (Input.GetKeyDown(key_Lane2))
        {
            lane2(1);
        }
        if (Input.GetKeyDown(key_Lane3))
        {
            lane3(2);
        }
        if (Input.GetKeyDown(key_Lane4))
        {
            lane4(3);
        }
    }

    /// <summary>
    /// 入力できているか確認
    /// </summary>
    /// <param name="key">キー</param>
    private void TestInput(KeyCode key)
    {
        Debug.Log(key);
    }
}
