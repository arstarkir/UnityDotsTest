using Unity.Netcode;
using UnityEngine;

public class UnitBuilding : CoreBuilding
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) && ownerId == OwnerClientId)
            RequestSpawnUnitServerRpc(0, ownerId, this.transform.position + new Vector3(0,0,-6));
    }

    [ServerRpc]
    void RequestSpawnUnitServerRpc(int unitId, ulong ownerId, Vector3 pos, ServerRpcParams rpcParams = default)
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
