using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
partial struct GoInGameClientSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkId>();
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach ((RefRO<NetworkId> networkId,Entity entity)
            in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(entity);
            Debug.Log("Setting Client as InGame");

            Entity rpcEntity = ecb.CreateEntity();
            ecb.AddComponent(rpcEntity, new GoInGameRequestRpc());
            ecb.AddComponent(rpcEntity, new SendRpcCommandRequest());
        }

        ecb.Playback(state.EntityManager);
    }
}

public struct GoInGameRequestRpc : IRpcCommand { }
