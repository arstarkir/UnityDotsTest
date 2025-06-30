using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Builder : NetworkBehaviour
{
    [HideInInspector] public bool isBuilding = false;
    GameObject curBuilding;
    public int buildID = 0;
    void Update()
    {
        if (!IsLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.B) && !isBuilding)
        {
            RequestSpawnBlueprintServerRpc(buildID);
            isBuilding = true;
        }

        if (isBuilding && curBuilding != null)
        {
            RequestChangeMaterialServerRpc(curBuilding.GetComponent<NetworkObject>().NetworkObjectId, true);

            Vector2 mousePos = Input.mousePosition;
            Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            RequestMoveBlueprintServerRpc(curBuilding.GetComponent<NetworkObject>().NetworkObjectId, hit.point);

            if (Input.GetMouseButtonDown(0))
            {
                isBuilding = false;
                RequestSpawnBuildingServerRpc(curBuilding.GetComponent<NetworkObject>().NetworkObjectId, curBuilding.transform.position);
            }
        }
    }

    [ServerRpc]
    void RequestSpawnBlueprintServerRpc(int buildID, ServerRpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;

        BuildingRegister buildingRegister = Resources.Load<BuildingRegister>("SO/MainBuildingRegister");
        GameObject temp = Instantiate(buildingRegister.buildingDatas[buildID].prefab);
        NetworkObject netObj = temp.GetComponent<NetworkObject>();
        netObj.gameObject.GetComponent<BuildingDataHandler>().buildingData = buildingRegister.buildingDatas[buildID];

        foreach (Transform obj in netObj.gameObject.GetComponentsInChildren<Transform>())
        {
            obj.gameObject.layer = 2;
        }
        netObj.SpawnWithOwnership(requesterId);
        SetupBlueprintVisualsClientRpc(netObj.NetworkObjectId, buildID);
        NotifyClientOfSpawnedObjectClientRpc(netObj.NetworkObjectId, requesterId);
    }

    [ClientRpc]
    void SetupBlueprintVisualsClientRpc(ulong objectId, int buildID, ClientRpcParams rpc = default)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
            return;

        foreach (Transform t in netObj.gameObject.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = 2;

        BuildingRegister buildingRegister = Resources.Load<BuildingRegister>("SO/MainBuildingRegister");
        netObj.GetComponent<BuildingDataHandler>().buildingData = buildingRegister.buildingDatas[buildID];
    }

    [ServerRpc]
    void RequestSpawnBuildingServerRpc(ulong objectId, Vector3 pos, ServerRpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;

        BuildingRegister buildingRegister = Resources.Load<BuildingRegister>("SO/MainBuildingRegister");
        GameObject temp = buildingRegister.buildingDatas.First(buildingData =>
        buildingData.name == NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId].GetComponent<BuildingDataHandler>().buildingData.name).prefab;
        
        GameObject building = Instantiate(temp, pos, Quaternion.identity);
        NetworkObject netObj = building.GetComponent<NetworkObject>();

        foreach (Transform t in netObj.gameObject.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = 7;
        netObj.SpawnWithOwnership(requesterId);
        FlowFieldManager.Instance.MarkAreaUnwalkable(netObj.GetComponent<Collider>().bounds);
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId].Despawn();
    }

    [ServerRpc]
    void RequestMoveBlueprintServerRpc(ulong objectId, Vector3 hitPoint, ServerRpcParams rpcParams = default)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId].transform.position = hitPoint;
    }

    [ClientRpc]
    void NotifyClientOfSpawnedObjectClientRpc(ulong objectId, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        NetworkObject spawnedObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
        curBuilding = spawnedObj.gameObject;
    }

    [ServerRpc]
    void RequestChangeMaterialServerRpc(ulong objectId, bool buildingStatus)
    {
        NetworkObject spawnedObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

        GameObject temp = spawnedObj.gameObject;
        foreach (var r in temp.GetComponentsInChildren<Renderer>(true))
        {
            Material unique = r.material;
            //unique.MakeAdditiveTransparent();
            r.material = unique;
        }

        //if (!temp.GetComponent<BlinkAlpha>())
        //    temp.AddComponent<BlinkAlpha>();

        ChangeMaterialClientRpc(objectId);
    }

    [ClientRpc]
    void ChangeMaterialClientRpc(ulong objectId, ClientRpcParams rpc = default)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var netObj))
            return;

        GameObject temp = netObj.gameObject;
        foreach (var r in temp.GetComponentsInChildren<Renderer>(true))
        {
            Material unique = r.material;
            //unique.MakeAdditiveTransparent();
            r.material = unique;
        }

        //if (!temp.GetComponent<BlinkAlpha>())
        //    temp.gameObject.AddComponent<BlinkAlpha>();
    }
}