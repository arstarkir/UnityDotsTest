using UnityEngine;
using Unity.Mathematics;

/// <summary>
///  Scene-view visualiser for FlowFieldManager.walkable.
///  Draws one translucent cube per grid cell.
/// </summary>
[ExecuteAlways]
public class FlowFieldGizmo : MonoBehaviour
{
    [Header("Colours")]
    public Color walkableColour = new Color(0f, 1f, 0f, 0.18f);
    public Color unwalkableColour = new Color(1f, 0f, 0f, 0.25f);

    [Header("Draw-options")]
    public bool drawWalkable = true;
    public bool drawUnwalkable = true;

    void OnDrawGizmosSelected()
    {
        // Need live data -> only works in Play mode or when manager exists
        FlowFieldManager mgr = FlowFieldManager.Instance;
        if (mgr == null || mgr.gridSize.x == 0) return;

        Vector3 origin = new Vector3(mgr.worldOrigin.x, 0f, mgr.worldOrigin.y);
        float step = mgr.cellSize;
        float3 half = new float3(step, 0.01f, step) * 0.5f;

        for (int y = 0; y < mgr.gridSize.y; y++)
        {
            for (int x = 0; x < mgr.gridSize.x; x++)
            {
                int2 cell = new int2(x, y);
                bool walk = mgr.IsWalkable(cell);   // small helper added below
                if (walk && !drawWalkable) continue;
                if (!walk && !drawUnwalkable) continue;

                Vector3 centre = origin + new Vector3((x + 0.5f) * step,
                                                      0.01f,             // lift a hair so it’s visible
                                                      (y + 0.5f) * step);

                Gizmos.color = walk ? walkableColour : unwalkableColour;
                Gizmos.DrawCube(centre, (Vector3)half * 2f);
            }
        }
    }
}
