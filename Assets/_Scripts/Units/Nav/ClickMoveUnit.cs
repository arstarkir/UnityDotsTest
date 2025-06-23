using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClickMoveUnit : NetworkBehaviour
{
    Camera cam;
    [SerializeField] LayerMask groundMask = 1;
    [SerializeField] SelectionManager selection;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) cam = Camera.main;
    }

    void Update()
    {
        if (!IsOwner || cam == null) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),
                                out var hit, 512f, groundMask))
            {
                List<ulong> ids = selection.CurrentUnitIds;
                if (ids.Count > 0)
                    SendGroupMoveServerRpc(hit.point, ids.ToArray());
            }
        }
    }

    [Rpc(SendTo.Server)]
    void SendGroupMoveServerRpc(Vector3 goal, ulong[] unitIds)
    {
        ushort id = FlowFieldManager.Instance.BuildField(goal);

        AssignFieldClientRpc(id, unitIds);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AssignFieldClientRpc(ushort fieldId, ulong[] unitIds)
    {
        foreach (ulong uid in unitIds)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(uid, out var obj))
            {
                if (obj.TryGetComponent(out FlowFollower ff))
                    ff.flowId.Value = fieldId;
            }
        }
    }
}
