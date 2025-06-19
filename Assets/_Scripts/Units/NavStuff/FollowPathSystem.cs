using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct FollowPathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (xf, spd, corners) in
                 SystemAPI.Query<RefRW<LocalTransform>,
                                 RefRO<MoveSpeed>,
                                 DynamicBuffer<NavCorner>>())
        {
            if (corners.Length == 0) continue;

            float3 pos = xf.ValueRO.Position;
            float3 tgt = corners[0].Value;
            float3 dir = tgt - pos;
            float step = spd.ValueRO.Value * dt;
            if (math.lengthsq(dir) <= step * step)
            {
                xf.ValueRW.Position = tgt;
                corners.RemoveAt(0);
            }
            else
            {
                xf.ValueRW.Position += math.normalize(dir) * step;
            }
        }
    }
}