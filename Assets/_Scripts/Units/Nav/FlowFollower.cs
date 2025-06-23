using Unity.Netcode;
using UnityEngine;
using Unity.Mathematics;

public class FlowFollower : NetworkBehaviour
{
    public NetworkVariable<ushort> flowId = new();
    public float speed = 4f;

    void Update()
    {
        if (!IsSpawned) 
            return;

        if (FlowFieldManager.Instance.TrySample(flowId.Value,
                                                transform.position,
                                                out float2 dir))
        {
            transform.position += new Vector3(dir.x, 0f, dir.y) *
                                   speed * Time.deltaTime;
        }
    }
}
