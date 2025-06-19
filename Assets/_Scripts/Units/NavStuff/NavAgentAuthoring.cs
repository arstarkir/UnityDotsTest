using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class NavAgentAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;

    class Baker : Baker<NavAgentAuthoring>
    {
        public override void Bake(NavAgentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<NavAgentTag>(entity);
            AddBuffer<NavCorner>(entity);
            AddComponent(entity,
    new MoveSpeed { Value = authoring.moveSpeed });
            AddComponent(entity, new NavDestination
            {
                Value = float3.zero
            });
        }
    }
}
