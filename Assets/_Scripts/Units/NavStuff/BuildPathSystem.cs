using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.AI;

[UpdateAfter(typeof(ClickToMoveSystem))]
public partial struct BuildPathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (xf, dest, corners) in
                 SystemAPI.Query<RefRO<LocalTransform>,
                                 RefRO<NavDestination>,
                                 DynamicBuffer<NavCorner>>())
        {


        }
    }
}