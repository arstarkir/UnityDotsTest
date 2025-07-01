using Unity.Netcode;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(Collider))]
public class FlowFollower : NetworkBehaviour
{
    public NetworkVariable<ushort> flowId = new();
    public NetworkVariable<Vector3> finalTarget = new();

    public float speed = 4f;
    [SerializeField] float unitRadius = 0.4f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float arrivedEps = 0.5f;

    [HideInInspector] public NetworkVariable<bool> IsMoveing = new (false);

    public override void OnNetworkSpawn()
    {
        if (IsServer) 
            finalTarget.Value = transform.position;

        base.OnNetworkSpawn();
    }

    void Update()
    {
        if (!IsSpawned) 
            return;

        if (!Reached(finalTarget.Value, arrivedEps * 9f))
        {
            if (FlowFieldManager.Instance.TrySample(flowId.Value, transform.position, out float2 dir))
            {
                Step(new Vector3(dir.x, 0f, dir.y));
                return;
            }
        }

        if (!Reached(finalTarget.Value, 0.05f))
        {
            Vector3 to = (finalTarget.Value - transform.position).normalized;
            Step(to);
        }
        else
        {
            IsMoveing.Value = false;
        }
    }

    [Rpc(SendTo.Server)]
    public void MoveDirectRpc(Vector3 dest)
    {
        finalTarget.Value = dest;
        Vector3 dir = (finalTarget.Value - transform.position).normalized;
        Step(dir);
        IsMoveing.Value = true;
    }

    bool Reached(Vector3 target, float tol)
    {
        return (transform.position - target).sqrMagnitude <= tol * tol;
    }

    void Step(Vector3 dir)
    {
        Vector3 next = transform.position + dir * speed * Time.deltaTime;
        if (!Physics.CheckSphere(next, unitRadius * 0.9f, obstacleMask))
            transform.position = next;
    }
}
