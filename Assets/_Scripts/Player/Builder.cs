using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Builder : NetworkBehaviour
{
    [HideInInspector] public bool isBuilding = false;
    GameObject curBuilding = null;
    public int buildID = 0;
    BuildingRegister buildingRegister;
    PlayerResources pResources;
    
    [SerializeField] GameObject buildingHandlerPref;
    List<GameObject> buildingHandler = new List<GameObject>();
    List<GameObject> groupBuildings = new List<GameObject>();

    void Start()
    {
        pResources = GetComponent<PlayerResources>();
        buildingRegister = Resources.Load<BuildingRegister>("SO/MainBuildingRegister");
    }

    void Update()
    {
        if (!IsLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.B) && !isBuilding && pResources.HasRes(buildingRegister.buildingDatas[buildID].cost))
            if (pResources.PayCost(buildingRegister.buildingDatas[buildID].cost))
            {
                RequestSpawnBlueprintServerRpc(buildID);
            }

        if (isBuilding && curBuilding != null)
        {
            Debug.Log("IsHost: " + IsHost);
            RequestChangeMaterialServerRpc(curBuilding.GetComponent<NetworkObject>().NetworkObjectId, true);

            Vector2 mousePos = Input.mousePosition;
            Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            RequestMoveBlueprintServerRpc(curBuilding.GetComponent<NetworkObject>().NetworkObjectId, hit.point,
                !Input.GetKey(KeyCode.LeftControl));

            if (Input.mouseScrollDelta.y != 0 && Input.GetKey(KeyCode.CapsLock))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    RequestRotateBlueprintServerRpc(Vector3.up, Input.mouseScrollDelta.y);
                else
                {
                    if (Input.mouseScrollDelta.y > 0)
                    {
                        if (pResources.PayCost(buildingRegister.buildingDatas[buildID].cost))
                            RequestSpawnBlueprintServerRpc(buildID);
                    }
                    else
                    {
                        pResources.RefundCost(buildingRegister.buildingDatas[buildID].cost);
                        RequestRotateBlueprintServerRpc(buildID);
                    }
                }
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                RequestRotateBlueprintServerRpc(buildID,true);
                isBuilding = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isBuilding = false;
                RequestSpawnBuildingServerRpc(curBuilding.transform.position);
            }
        }
    }
    
    [ServerRpc]
    void RequestRotateBlueprintServerRpc(Vector3 rot, float mouseYRot,ServerRpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;
        buildingHandler.FirstOrDefault(gameObject => gameObject.name == "buildingHandler_" + requesterId)
            .transform.Rotate(rot , 15f * mouseYRot);
    }

    [ServerRpc]
    void RequestRotateBlueprintServerRpc(int buildID, bool all = false, ServerRpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;
        ulong objectId;
        BuildingRegister buildingRegister = Resources.Load<BuildingRegister>("SO/MainBuildingRegister");
        GameObject curBuildingHandler = buildingHandler.FirstOrDefault(gameObject => gameObject.name == "buildingHandler_" + requesterId);
        if (!all)
        {
            objectId = groupBuildings[groupBuildings.Count - 1].GetComponent<NetworkObject>().NetworkObjectId;
            NetworkObject netBlueprint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
            netBlueprint.TrySetParent((NetworkObject)null);
            netBlueprint.Despawn();
            groupBuildings.RemoveAt(groupBuildings.Count - 1);
        }
        else
        {
            for (int i = 0; i < groupBuildings.Count; i++)
            {
                objectId = groupBuildings[i].GetComponent<NetworkObject>().NetworkObjectId;
                NetworkObject netBlueprint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
                netBlueprint.TrySetParent((NetworkObject)null);
                netBlueprint.Despawn();
            }
            groupBuildings.Clear();
            RefundCostClientRpc(buildID, groupBuildings.Count, requesterId);
        }
    }

    [ClientRpc]
    void RefundCostClientRpc(int buildID, int amount, ulong targetClientId, ClientRpcParams rpc = default)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        for (int i = 0;i < amount;i++)
            pResources.RefundCost(buildingRegister.buildingDatas[buildID].cost);
    }

    [ServerRpc]
    void RequestSpawnBlueprintServerRpc(int buildID, ServerRpcParams rpcParams = default)
    {
        BuildingRegister buildingRegister = Resources.Load<BuildingRegister>("SO/MainBuildingRegister");
        ulong requesterId = rpcParams.Receive.SenderClientId;
        GameObject curBuildingHandler = buildingHandler.FirstOrDefault(gameObject => gameObject.name == "buildingHandler_" + requesterId);
        if (curBuildingHandler == null)
        {
            curBuildingHandler = Instantiate(buildingHandlerPref);
            curBuildingHandler.name = "buildingHandler_" + requesterId;
            curBuildingHandler.GetComponent<NetworkObject>().name = "buildingHandler_" + requesterId;
            curBuildingHandler.GetComponent<NetworkObject>().SpawnWithOwnership(requesterId);
            buildingHandler.Add(curBuildingHandler);
        }
            
        GameObject temp = Instantiate(buildingRegister.buildingDatas[buildID].prefab);
        foreach (CoreBuilding buildingComp in temp.GetComponents<CoreBuilding>())
            buildingComp.enabled = false;
        NetworkObject netObj = temp.GetComponent<NetworkObject>();
        netObj.gameObject.GetComponent<BuildingDataHandler>().buildingData = buildingRegister.buildingDatas[buildID];

        foreach (Transform obj in netObj.gameObject.GetComponentsInChildren<Transform>())
            obj.gameObject.layer = 2;
        netObj.SpawnWithOwnership(requesterId);
        Debug.Log(netObj.TrySetParent(curBuildingHandler.GetComponent<NetworkObject>(), worldPositionStays: false));

        groupBuildings.Clear();
        for (int i = 0; i < curBuildingHandler.transform.childCount; i++)
            groupBuildings.Add(curBuildingHandler.transform.GetChild(i).gameObject);

        List<Transform> netObjSnaps = new List<Transform>();
        foreach (Transform child in curBuildingHandler.transform.GetChild(0).GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("SnapPoint"))
                netObjSnaps.Add(child);
        }

        float longestX = 5;
        if (netObjSnaps.Count >= 2)
        {
            float minX = netObjSnaps.Min(t => t.position.x);
            float maxX = netObjSnaps.Max(t => t.position.x);
            longestX = maxX - minX;
        }
        netObj.transform.localPosition = new Vector3(longestX, 0, 0) * (curBuildingHandler.transform.childCount - 1);

        SetupBlueprintVisualsClientRpc(netObj.NetworkObjectId, buildID);
        NotifyClientOfSpawnedObjectClientRpc(curBuildingHandler.GetComponent<NetworkObject>().NetworkObjectId, requesterId);
    }

    [ServerRpc(RequireOwnership = false)]
    void SnapBuildingServerRpc(ulong objectId, Vector3 mousePos, ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
            return;

        ulong requesterId = rpcParams.Receive.SenderClientId;
        Debug.Log("netObjOwner " + netObj.OwnerClientId + " requesterId "+ requesterId);

        RaycastHit[] hits = Physics.SphereCastAll(new Ray(mousePos, mousePos + new Vector3(0,1,0)),5);
        RaycastHit[] snapHits = hits.Where(hit => hit.transform.CompareTag("SnapPoint")).ToArray();
        
        if(snapHits.Length == 0)
        {
            netObj.transform.localPosition = mousePos;
            return;
        }


        List<Transform> netObjSnaps = new List<Transform>();
        foreach (Transform child in netObj.transform.GetChild(0).GetComponentsInChildren<Transform>())
        {
            if(child.CompareTag("SnapPoint"))
                netObjSnaps.Add(child);
        }

        Transform closestNetObjSnap = null;
        Transform closestSnapHit = null;
        float closestDistance = float.MaxValue;
        float tempDistance;

        foreach (Transform child in netObjSnaps)
            foreach (RaycastHit hit in snapHits)
            {
                tempDistance = Vector3.Distance(child.position, hit.point);
                if (closestDistance > tempDistance)
                {
                    closestDistance = tempDistance;
                    closestNetObjSnap = child;
                    closestSnapHit = hit.collider.transform;
                }
            }

        if (closestNetObjSnap == null || closestSnapHit == null)
        {
            netObj.transform.position = mousePos;
            return;
        }

        Vector3 offset = closestSnapHit.position - closestNetObjSnap.position;
        netObj.transform.position += offset;
    }

    [ClientRpc]
    void SetupBlueprintVisualsClientRpc(ulong objectId, int buildID, ClientRpcParams rpc = default)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
            return;

        foreach (CoreBuilding buildingComp in netObj.GetComponents<CoreBuilding>())
            buildingComp.enabled = false;

        foreach (Transform t in netObj.gameObject.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = 2;

        BuildingRegister buildingRegister = Resources.Load<BuildingRegister>("SO/MainBuildingRegister");
        netObj.GetComponent<BuildingDataHandler>().buildingData = buildingRegister.buildingDatas[buildID];
    }

    [ServerRpc]
    void RequestSpawnBuildingServerRpc(Vector3 pos, ServerRpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;
        ulong objectId;
        BuildingRegister buildingRegister = Resources.Load<BuildingRegister>("SO/MainBuildingRegister");
        GameObject curBuildingHandler = buildingHandler.FirstOrDefault(gameObject => gameObject.name == "buildingHandler_" + requesterId);

        for (int i = 0; i < groupBuildings.Count; i++)
        {
            objectId = groupBuildings[i].GetComponent<NetworkObject>().NetworkObjectId;
            NetworkObject netBlueprint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

            GameObject temp = buildingRegister.buildingDatas.First(buildingData =>
                buildingData.name == netBlueprint.GetComponent<BuildingDataHandler>().buildingData.name).prefab;
            GameObject building = Instantiate(temp, netBlueprint.transform.position, netBlueprint.transform.rotation);
            NetworkObject netObj = building.GetComponent<NetworkObject>();

            foreach (Transform t in netObj.gameObject.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = 7;
            netObj.SpawnWithOwnership(requesterId);
            netObj.transform.rotation = curBuildingHandler.transform.rotation;
            FlowFieldManager.Instance.MarkAreaUnwalkable(netObj.GetComponent<Collider>().bounds);

            netBlueprint.TrySetParent((NetworkObject)null);
            netBlueprint.Despawn();
        }
        curBuildingHandler.transform.position = new Vector3(0, 0, 0);
    }

    [ServerRpc]
    void RequestMoveBlueprintServerRpc(ulong objectId, Vector3 hitPoint, bool isSnapable = true, ServerRpcParams rpcParams = default)
    {
        if(isSnapable)
            SnapBuildingServerRpc(objectId, hitPoint, rpcParams);
        else
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId].transform.position = hitPoint;
    }

    [ClientRpc]
    void NotifyClientOfSpawnedObjectClientRpc(ulong objectId, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        NetworkObject spawnedObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
        curBuilding = spawnedObj.gameObject;
        isBuilding = true;
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