using Unity.Netcode;
using UnityEngine;

public class MouseWorldPosition : NetworkBehaviour
{
    public static MouseWorldPosition instance { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
            return;
        instance = this;
    }

    public Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        return plane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;
    }
}
