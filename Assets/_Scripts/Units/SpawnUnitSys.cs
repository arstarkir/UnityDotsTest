using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawnUnitSys : ISystem
{
    EntityQuery m_Requests;
    Entity prefab;
    bool initialized;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCollection>();
        m_Requests = state.GetEntityQuery(ComponentType.ReadOnly<SpawnUnitRequest>());
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!initialized)
        {
            prefab = SystemAPI.GetSingleton<UnitCollection>().unit;
            initialized = true;
        }

        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                     .CreateCommandBuffer(state.WorldUnmanaged)
                     .AsParallelWriter();

        var job = new SpawnJob
        {
            prefab = prefab,
            ecb = ecb
        };
        state.Dependency = job.ScheduleParallel(m_Requests, state.Dependency);
    }

    [BurstCompile]
    partial struct SpawnJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public Entity prefab;

        void Execute([EntityIndexInQuery] int idx, Entity entity, in SpawnUnitRequest req)
        {
            var unit = ecb.Instantiate(idx, prefab);
            ecb.SetComponent(idx, unit,
            LocalTransform.FromPosition(req.pos));
            ecb.DestroyEntity(idx, entity);
        }
    }
}

public struct UnitCollection : IComponentData
{
    public Entity unit;
}

public struct SpawnUnitRequest : IComponentData
{
    public float3 pos;
}