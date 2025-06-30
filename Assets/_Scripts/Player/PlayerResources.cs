using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerResources : NetworkBehaviour
{
    [SerializeField] ResourcesUI resourcesUI;

    List<StorageBuilding> mStorage = new List<StorageBuilding>();
    float maxMStorage = 100;
    float curMAmount = 100;
    List<StorageBuilding> fStorage = new List<StorageBuilding>();
    float maxFStorage = 100;
    float curFAmount = 100;
    List<StorageBuilding> pStorage = new List<StorageBuilding>();
    float maxPStorage = 100;
    float curPAmount = 100;

    void Start()
    {
        UpdateAllUI();
    }

    public void AddStorage(StorageBuilding storageBuilding)
    {
        if(storageBuilding.rType == ResTypes.Minirals)
        {
            mStorage.Add(storageBuilding);
            maxMStorage += storageBuilding.maxStorage;
        }
        if (storageBuilding.rType == ResTypes.Food)
        {
            fStorage.Add(storageBuilding);
            maxFStorage += storageBuilding.maxStorage;
        }
        if (storageBuilding.rType == ResTypes.Power)
        {
            pStorage.Add(storageBuilding);
            maxPStorage += storageBuilding.maxStorage;
        }
        UpdateAllUI();
    }

    public void AddRes(float amount,ResTypes rType)
    {
        if (rType == ResTypes.Minirals)
            foreach (StorageBuilding storageBuilding in mStorage)
            {
                curMAmount += amount;
                amount = storageBuilding.AddRes(amount);
                curMAmount -= amount;
                if (amount == 0)
                {
                    resourcesUI.UpdateCurRes(curMAmount, ResTypes.Minirals);
                    return;
                }
            }
        if (rType == ResTypes.Food)
            foreach (StorageBuilding storageBuilding in fStorage)
            {
                curFAmount += amount;
                amount = storageBuilding.AddRes(amount);
                curFAmount -= amount;
                if (amount == 0)
                {
                    resourcesUI.UpdateCurRes(curFAmount, ResTypes.Food);
                    return;
                }
            }
        if (rType == ResTypes.Power)
            foreach (StorageBuilding storageBuilding in pStorage)
            {
                curPAmount += amount;
                amount = storageBuilding.AddRes(amount);
                curPAmount -= amount;
                if (amount == 0)
                {
                    resourcesUI.UpdateCurRes(curPAmount, ResTypes.Power);
                    return;
                }
            }
    }

    public void RemoveStorage(StorageBuilding storageBuilding)
    {
        if (!HasStorageBuilding(storageBuilding))
            return;

        if (storageBuilding.rType == ResTypes.Minirals)
        {
            mStorage.Remove(storageBuilding);
            maxMStorage -= storageBuilding.maxStorage;
        }
        if (storageBuilding.rType == ResTypes.Food)
        {
            fStorage.Remove(storageBuilding);
            maxFStorage -= storageBuilding.maxStorage;
        }
        if (storageBuilding.rType == ResTypes.Power)
        {
            pStorage.Remove(storageBuilding);
            maxPStorage -= storageBuilding.maxStorage;
        }
        UpdateAllUI();
    }

    public bool HasStorageBuilding(StorageBuilding storageBuilding)
    {
        if (storageBuilding.rType == ResTypes.Minirals)
        {
            mStorage.Contains(storageBuilding);
            return true;
        }
        if (storageBuilding.rType == ResTypes.Food)
        {
            fStorage.Contains(storageBuilding);
            return true;
        }
        if (storageBuilding.rType == ResTypes.Power)
        {
            pStorage.Contains(storageBuilding);
            return true;
        }
        return false;
    }

    public void UpdateAllUI()
    {
        resourcesUI.UpdateMaxRes(maxMStorage, ResTypes.Minirals);
        resourcesUI.UpdateMaxRes(maxFStorage, ResTypes.Food);
        resourcesUI.UpdateMaxRes(maxPStorage, ResTypes.Power);
        resourcesUI.UpdateCurRes(curMAmount, ResTypes.Minirals);
        resourcesUI.UpdateCurRes(curFAmount, ResTypes.Food);
        resourcesUI.UpdateCurRes(curPAmount, ResTypes.Power);
    }
}
