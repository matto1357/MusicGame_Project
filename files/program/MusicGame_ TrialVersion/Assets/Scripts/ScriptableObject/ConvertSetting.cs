using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicGame/Call ConvertMenu", fileName = "ConvertMenu", order =-10)]
public class ConvertSetting : ScriptableObject
{
    public TextAsset seetSource;
    public DataType targetDataType;
}
