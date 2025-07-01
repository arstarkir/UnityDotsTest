using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitBuilding : CoreBuilding
{
    Queue<int> unitQueue = new Queue<int>();
    UnitRegister unitRegister;
    float inProgressTime = 0;

    private void Start()
    {
        if (!IsOwner)
            enabled = false;

        unitRegister = Resources.Load<UnitRegister>("SO/MainUnitRegister");
    }

    void Update()
    {
        if (unitQueue.Count > 0)
        {
            inProgressTime += Time.deltaTime;

            if (inProgressTime >= unitRegister.unitDatas[unitQueue.Peek()].spawnTime)
            {
                RequestSpawnUnitServerRpc(unitQueue.Dequeue(), this.transform.position + new Vector3(0, 0, -6));
                Debug.Log("Spawned");
                inProgressTime = 0;
            }
        }
    }

    public void RequestRequestSpawnUnit(int unitID)
    {
        unitQueue.Enqueue(unitID);
    }

    [ServerRpc]
    void RequestSpawnUnitServerRpc(int unitId, Vector3 pos, ServerRpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;

        UnitRegister UnitRegister = Resources.Load<UnitRegister>("SO/MainUnitRegister");
        GameObject temp = UnitRegister.unitDatas[unitId].prefab;

        GameObject unit = Instantiate(temp, pos, Quaternion.identity);
        NetworkObject netObj = unit.GetComponent<NetworkObject>();
        foreach (Transform t in netObj.gameObject.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = 8;

        netObj.SpawnWithOwnership(requesterId);
    }
}
