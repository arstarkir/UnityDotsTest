using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
partial struct NetcodeClientSys : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Entity entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, new SimpleRPC{ value = 3 });
            state.EntityManager.AddComponentData(entity, new SendRpcCommandRequest());
            Debug.Log("A");
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
