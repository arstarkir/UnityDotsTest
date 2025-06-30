using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuildingRegister", menuName = "Scriptable Objects/BuildingRegister")]
public class BuildingRegister : ScriptableObject
{
    public List<BuildingData> buildingDatas;
}
