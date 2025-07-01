using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ClickMoveUnit : NetworkBehaviour
{
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
            List<ulong> ids = selection.GetUnitSelectedIds();
            if (ids.Count > 0)
                SendMoveServerRpc(MouseWorldPosition.instance.GetPosition(), ids.ToArray());
            Debug.Log(ids.Count);
        }
    }

    [Rpc(SendTo.Server)]
    void SendMoveServerRpc(Vector3 goal, ulong[] unitIds)
    {
        ushort fieldId = FlowFieldManager.Instance.BuildField(goal);

        int n = unitIds.Length;
        int side = Mathf.CeilToInt(Mathf.Sqrt(n));
        float spacing = 1.2f;

        Vector3[] slots = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            int row = i / side;
            int col = i % side;
            float fx = (col - (side - 1) * 0.5f) * spacing;
            float fz = (row - (side - 1) * 0.5f) * spacing;
            slots[i] = goal + new Vector3(fx, 0f, fz);
        }

        AssignFormationClientRpc(fieldId, unitIds, slots);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AssignFormationClientRpc(ushort fieldId, ulong[] ids, Vector3[] slots)
    {
        if (!IsServer) 
            return;

        for (int i = 0; i < ids.Length; i++)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(ids[i], out var obj) &&
                obj.TryGetComponent(out FlowFollower ff))
            {
                ff.flowId.Value = fieldId;
                ff.finalTarget.Value = slots[i];
                ff.IsMoveing.Value = true;
            }
        }
    }

}
