using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public sealed class NavMeshBootstrap : MonoBehaviour
{
    void Awake()
    {
        var surface = GetComponent<NavMeshSurface>();
        surface.AddData();
    }
}
