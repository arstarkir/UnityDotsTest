using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct GoInGameServerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<NetworkId>();
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        EntitiesReferences er = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((RefRO<ReceiveRpcCommandRequest> receiveRpcCommandRequest, Entity entity)
            in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<GoInGameRequestRpc>().WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(receiveRpcCommandRequest.ValueRO.SourceConnection);

            Entity playerEntity = ecb.Instantiate(er.playerPrefabEntity);
            NetworkId netId = SystemAPI.GetComponent<NetworkId>(receiveRpcCommandRequest.ValueRO.SourceConnection);
            Debug.Log(netId);
            ecb.AddComponent(playerEntity, new GhostOwner { NetworkId = netId.Value });
            ecb.AppendToBuffer(receiveRpcCommandRequest.ValueRO.SourceConnection, new LinkedEntityGroup { Value = playerEntity});
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
    }
}

