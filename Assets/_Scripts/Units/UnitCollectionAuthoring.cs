using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class UnitCollectionAuthoring : MonoBehaviour
{
    public GameObject unitPrefab;

    public class Baker : Baker<UnitCollectionAuthoring>
    {
        public override void Bake(UnitCollectionAuthoring authoring)
        {
            var prefabEntity = GetEntity(authoring.unitPrefab, TransformUsageFlags.Dynamic);

            var e = CreateAdditionalEntity(TransformUsageFlags.None);
            AddComponent(e, new UnitCollection { unit = prefabEntity });
        }
    }
}

