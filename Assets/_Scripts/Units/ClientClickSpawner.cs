using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.NetCode;

public class ClientClickSpawner : MonoBehaviour
{
    EntityManager em;
    EntityQuery mouseQ;

    void Awake()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        mouseQ = em.CreateEntityQuery(ComponentType.ReadOnly<MouseInput>(), ComponentType.ReadOnly<GhostOwnerIsLocal>());
    }

    void Update()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (!world.IsClient())
            return;

        if (!Input.GetKeyDown(KeyCode.U))
            return;

        var req = em.CreateEntity();
        em.AddComponentData(req, new SpawnUnitRequest { pos = (float3)mouseQ.GetSingleton<MouseInput>().MouseWorldPos });
    }
}
