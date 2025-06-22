using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct NetcodeServerSys : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach ((RefRO<SimpleRPC> simpleRpc, RefRO<ReceiveRpcCommandRequest> receiveRpcCommandRequest, Entity entity)
        in SystemAPI.Query<RefRO<SimpleRPC>,RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
        {
            Debug.Log(simpleRpc.ValueRO.value);
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
