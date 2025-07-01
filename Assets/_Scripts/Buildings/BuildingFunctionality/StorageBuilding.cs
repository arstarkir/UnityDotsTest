using UnityEngine;
using Unity.Netcode;
using UnityEditor.PackageManager;

public class StorageBuilding : CoreBuilding
{
    public ResTypes rType;
    public float maxStorage = 100;
    public float curStorage = 0;
    PlayerResources player;

    void Start()
    {
        if(OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            player = FindAnyObjectByType<PlayerResources>();
            player.AddStorage(this);
        }
    }

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

    private void OnDestroy()
    {
        try
        {
            if (OwnerClientId == NetworkManager.Singleton.LocalClientId && this.enabled)
            {
                player.RemoveStorage(this);
                player.AddRes(-curStorage, rType);
            }
        }
        catch
        {

        }
    }
}

public enum ResTypes
{
    Minirals = 0,
    Food = 1,
    Power = 2
}