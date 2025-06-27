using Unity.Netcode;
using UnityEngine;
using Unity.Mathematics;

public class FlowFollower : NetworkBehaviour
{
    public NetworkVariable<ushort> flowId = new();
    public float speed = 4f;
    [SerializeField] float unitRadius = 0.4f;
    [SerializeField] LayerMask obstacleMask = ~7;

    void Update()
    {
        if (!IsSpawned) return;

        if (FlowFieldManager.Instance.TrySample(flowId.Value, transform.position,out float2 dir))
        {
            transform.position += new Vector3(dir.x, 0, dir.y) *
                                  speed * Time.deltaTime;
        }
        if (transform.position != transform.position + new Vector3(dir.x, 0, dir.y) *
                                  speed * Time.deltaTime)
            Debug.Log(dir);
        Vector3 step = new Vector3(dir.x, 0, dir.y) * speed * Time.deltaTime;
        Vector3 next = transform.position + step;
        Debug.Log(next);

        float probe = unitRadius * 0.9f;
        if (!Physics.CheckSphere(next, probe, obstacleMask))
            transform.position = next;
    }
}
