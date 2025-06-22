using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

public partial struct SpawnUnitSys : ISystem
{
    Entity prefab;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCollection>();
        state.RequireForUpdate<ReceiveRpcCommandRequest>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (prefab == Entity.Null)
        {
            prefab = SystemAPI.GetSingleton<UnitCollection>().unit;
        }

        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                                   .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (rpc, recv, entity) in
                 SystemAPI.Query<SpawnUnitRequest, ReceiveRpcCommandRequest>()
                           .WithEntityAccess())
        {
            var unit = ecb.Instantiate(prefab);
            ecb.SetComponent(unit, LocalTransform.FromPosition(rpc.pos));
            ecb.AppendToBuffer(recv.SourceConnection, new LinkedEntityGroup { Value = unit });
            ecb.DestroyEntity(entity);
        }
    }
}

public struct UnitCollection : IComponentData
{
    public Entity unit;
}

public struct SpawnUnitRequest : IRpcCommand
{
    public float3 pos;
}