using NUnit.Framework.Internal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public struct NavAgentTag : IComponentData { }
public struct MoveSpeed : IComponentData { public float Value; }
public struct NavDestination : IComponentData { public float3 Value; }
public struct NavCorner : IBufferElementData { public float3 Value; }


public partial struct ClickToMoveSystem : ISystem
{
    EntityQuery mouseQ;

    void Awake()
    {
        mouseQ = World.DefaultGameObjectInjectionWorld.EntityManager.
            CreateEntityQuery(ComponentType.ReadOnly<MouseInput>(), ComponentType.ReadOnly<GhostOwnerIsLocal>());
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!Input.GetMouseButtonDown(0) || Camera.main == null)
            return;

        Vector3 mPos = mouseQ.GetSingleton<MouseInput>().MouseWorldPos;
        if (!NavMesh.SamplePosition(mPos, out var navHit, 2f, NavMesh.AllAreas)) return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (tag, corners, lt, entity) in
                 SystemAPI.Query<NavAgentTag, DynamicBuffer<NavCorner>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            if (!state.EntityManager.HasComponent<NavDestination>(entity))
                ecb.AddComponent(entity, new NavDestination { Value = navHit.position });
            else
                ecb.SetComponent(entity, new NavDestination { Value = navHit.position });

            corners.Clear();
            var path = new NavMeshPath();
            if (!NavMesh.CalculatePath(lt.ValueRO.Position, navHit.position, NavMesh.AllAreas, path))
                continue;

            foreach (float3 c in path.corners)
                corners.Add(new NavCorner { Value = c });
        }
        ecb.Playback(state.EntityManager);
    }
}