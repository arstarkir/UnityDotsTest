using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct MouseInputGatherSys : ISystem
{
    Entity mi;
    public void OnCreate(ref SystemState state)
    {
        mi = state.EntityManager.CreateSingleton<MouseInput>();
        state.EntityManager.AddComponent<GhostOwnerIsLocal>(mi);
        Debug.Log("MouseInput");
    }

    public void OnUpdate(ref SystemState state)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float enter))
        {
            state.EntityManager.SetComponentData(mi, new MouseInput
            {
                MouseWorldPos = (float3)ray.GetPoint(enter)
            });
        }
    }
}

public struct MouseInput : IInputComponentData
{
    public float3 MouseWorldPos;
}