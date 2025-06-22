using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public class ClientClickSpawner : MonoBehaviour
{
    EntityManager em;
    EntityQuery mouseQ;
    EntityQuery connQ;

    void Awake()
    {
        var clientWorld = World.All.First(w => (w.Flags & WorldFlags.GameClient) != 0); 
        em = clientWorld.EntityManager;

        mouseQ = em.CreateEntityQuery(new EntityQueryDesc{ All = new[] { ComponentType.ReadOnly<MouseInput>() },
        Options = EntityQueryOptions.IncludeSystems});

        connQ = em.CreateEntityQuery(ComponentType.ReadOnly<NetworkId>());
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.U))
            return;
        int cnt = mouseQ.CalculateEntityCount(); 
        Debug.Log($"{em.World.Name}  {cnt} MouseInput singleton(s)");
        if (cnt != 1) return;


        if (!mouseQ.TryGetSingleton(out MouseInput mi) || connQ.IsEmptyIgnoreFilter)
            return;


        var connEntity = connQ.GetSingletonEntity();

        var rpc = em.CreateEntity();
        em.AddComponentData(rpc, new SpawnUnitRequest { pos = mi.MouseWorldPos });
        em.AddComponentData(rpc, new SendRpcCommandRequest
        {
            TargetConnection = Entity.Null
        });
    }
}
