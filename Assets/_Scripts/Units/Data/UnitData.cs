using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public string description;
    public CostData cost;
    public float spawnTime = 1;
    public Sprite sprite;
    public GameObject prefab;
}
