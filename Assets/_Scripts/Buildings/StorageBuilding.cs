using UnityEngine;

public class StorageBuilding : CoreBuilding
{
    public ResTypes rType;
    public float maxStorage = 100;
    public float curStorage = 0;

    public float AddRes(float amount)
    {
        if(curStorage + amount <= maxStorage && curStorage + amount >= 0)
        {
            curStorage += amount;
            return 0;
        }
        else
        if(amount > 0)
        {
            amount -= curStorage - maxStorage;
            curStorage = maxStorage;
            return amount;
        }
        else
        {
            amount += curStorage;
            curStorage = 0;
            return amount;
        }
    }
}

public enum ResTypes
{
    Minirals = 0,
    Food = 1,
    Power = 2
}