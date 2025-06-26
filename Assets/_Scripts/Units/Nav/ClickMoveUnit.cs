using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ClickMoveUnit : NetworkBehaviour
{
    [SerializeField] LayerMask groundMask = ~0;
    [SerializeField] SelectionManager selection;

    Camera _cam;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) _cam = Camera.main;
    }

    void Update()
    {
        if (!IsOwner || !_cam) return;

        if (Input.GetMouseButtonDown(1))
        {
            List<ulong> ids = selection.SelectedIds;
            if (ids.Count > 0)
                SendMoveServerRpc(MouseWorldPosition.instance.GetPosition(), ids.ToArray());
            Debug.Log(ids.Count);
        }
    }

    [Rpc(SendTo.Server)]
    void SendMoveServerRpc(Vector3 goal, ulong[] unitIds)
    {
        ushort id = FlowFieldManager.Instance.BuildField(goal);
        AssignFieldClientRpc(goal, id, unitIds);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AssignFieldClientRpc(Vector3 goal, ushort fieldId, ulong[] ids)
    {
        if (!FlowFieldManager.Instance.TrySample(fieldId, Vector3.zero, out _))
            FlowFieldManager.Instance.BuildField(goal, fieldId);

        foreach (ulong uid in ids)
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(uid, out var o) &&
                o.TryGetComponent(out FlowFollower f))
                f.flowId.Value = fieldId;
    }
}
