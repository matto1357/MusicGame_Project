using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    //レーン毎のデリゲート
    public delegate void Lane(int num, bool keyState);

    public Lane[] lanes;

    //キー設定、後に設定可能にする
    public KeyCode[] key =
    {
        KeyCode.Z,
        KeyCode.X,
        KeyCode.C,
        KeyCode.V,
    };

    public bool[] inputStates;

    private int keyNum;

    public void KeySetting()
    {
        keyNum = MusicManager.instance.data_KEY;

        lanes = new Lane[keyNum];
        inputStates = new bool[keyNum];

        for (int i = 0; i < keyNum; i++)
        {
            lanes[i] += MusicManager.instance.InputKey;
        }
    }

    private void Update()
    {
        for(int i = 0; i < keyNum; i++)
        {
            if(Input.GetKey(key[i]) != inputStates[i])
            {
                inputStates[i] = !inputStates[i];
                lanes[i](i, inputStates[i]);
            }
        }
    }
}
