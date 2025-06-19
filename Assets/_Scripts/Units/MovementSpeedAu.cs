using Unity.Entities;
using UnityEngine;

public class MovementSpeedAu : MonoBehaviour
{
    public float speed;

    public class Baker : Baker<MovementSpeedAu>
    {
        public override void Bake(MovementSpeedAu authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MovementSpeed { speed = authoring.speed });
            AddComponent<NavAgentTag>(entity);
            AddBuffer<NavCorner>(entity);
        }
    }
}

public struct MovementSpeed : IComponentData
{
    public float speed;
}