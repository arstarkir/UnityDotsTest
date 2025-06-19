using Unity.Entities;
using UnityEngine;

public class MouseInputAu : MonoBehaviour
{
    class Baker : Baker<MouseInputAu>
    {
        public override void Bake(MouseInputAu authoring)
        {
            AddComponent(new MouseInput());
        }
    }
}
