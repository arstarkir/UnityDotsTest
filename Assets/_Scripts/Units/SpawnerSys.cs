using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct Spawner : ISystem
{
    int spawned;
    Unity.Mathematics.Random random;
    public void OnCreate(ref SystemState state)
    {
        random = new Unity.Mathematics.Random(2484);

        spawned = 0;
    }

    public void OnUpdate(ref SystemState state)
    {
        if (spawned == 1)
            return;

        foreach (var spwaner in SystemAPI.Query<RefRW<SpanwerComponent>>())
        {
            Entity newEntity = state.EntityManager.Instantiate(spwaner.ValueRW.prefab);
            state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(new float3(0f, 1f, 0f)));

            spawned++;
        }
    }
}