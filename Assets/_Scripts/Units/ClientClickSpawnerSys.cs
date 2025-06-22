using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientClickSpawnerSys : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MouseInput>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!Input.GetKeyDown(KeyCode.U))
            return;
        var mi = SystemAPI.GetSingleton<MouseInput>();
        var e = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(e,
            new SpawnUnitRequest { pos = mi.MouseWorldPos });
        state.EntityManager.AddComponentData(e,
            new SendRpcCommandRequest { });
        Debug.Log("Z");
    }
}
