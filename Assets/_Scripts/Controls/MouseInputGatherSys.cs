using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class MouseInputGatherSys : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<MouseInput>();
    }
    protected override void OnUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float3 mouseWorld = float3.zero;
        if (groundPlane.Raycast(ray, out float enter))
        {
            mouseWorld = ray.GetPoint(enter);
        }
        Entities.WithAll<GhostOwnerIsLocal>().ForEach((ref MouseInput input) => {
            input.MouseWorldPos = mouseWorld;
        }).Run();
    }
}

public struct MouseInput : IInputComponentData
{
    public float3 MouseWorldPos;
}