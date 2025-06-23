using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitRegister", menuName = "Scriptable Objects/UnitRegister")]
public class UnitRegister : ScriptableObject
{
    public List<UnitData> unitDatas;
}