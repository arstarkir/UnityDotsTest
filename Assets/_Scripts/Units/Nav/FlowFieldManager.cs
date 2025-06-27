using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FlowFieldManager : MonoBehaviour
{
    public static FlowFieldManager Instance { get; private set; }

    [Header("Grid set-up")]
    public Vector2 worldOrigin = Vector2.zero;
    public float cellSize = 1f;
    public int2 gridSize = new int2(128, 128);

    NativeArray<byte> walkable;
    Dictionary<ushort, NativeArray<float2>> fields = new();
    ushort nextId = 1;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        walkable = new NativeArray<byte>(gridSize.x * gridSize.y,
                                         Allocator.Persistent);

        for (int i = 0; i < walkable.Length; i++)
            walkable[i] = 1;
    }

    void OnDestroy()
    {
        foreach (var f in fields.Values) if (f.IsCreated) f.Dispose();
        if (walkable.IsCreated) walkable.Dispose();
        Instance = null;
    }

    public ushort BuildField(Vector3 goalWorld, ushort forcedId = 0)
    {
        int2 goal = WorldToCellXZ(goalWorld);
        ushort id = forcedId != 0 ? forcedId : nextId++;
        if (fields.ContainsKey(id)) 
            return id;
        var cost = new NativeArray<int>(walkable.Length, Allocator.TempJob);
        var dir = new NativeArray<float2>(walkable.Length, Allocator.Persistent);

        var j0 = new BuildCostFieldJob
        {
            size = gridSize,
            walkable = walkable,
            cost = cost
        }.Schedule(cost.Length, 64);

        var j1 = new IntegrateFieldJob
        {
            size = gridSize,
            goal = goal,
            cost = cost
        }.Schedule(j0);

        var j2 = new BuildFlowFieldJob
        {
            size = gridSize,
            cost = cost,
            dir = dir
        }.Schedule(dir.Length, 64, j1);

        j2.Complete();
        cost.Dispose();

        fields.Add(id, dir);
        return id;
    }

    public bool TrySample(ushort id, Vector3 worldPos, out float2 dirOut)
    {
        dirOut = float2.zero;
        if (id == 0 || !fields.TryGetValue(id, out var field) || !field.IsCreated)
            return false;

        int2 cell = WorldToCellXZ(worldPos);
        if (!FlowFieldUtils.InBounds(cell, gridSize)) 
            return false;

        dirOut = field[FlowFieldUtils.Index(cell, gridSize)];
        return true;
    }

    public void MarkAreaUnwalkable(Bounds aabb)
    {
        float pad = 0.3f + cellSize * 0.01f;
        aabb.Expand(pad * 2f);

        int2 min = WorldToCellXZ(aabb.min);
        int2 max = WorldToCellXZ(aabb.max);

        for (int y = min.y; y <= max.y; y++)
            for (int x = min.x; x <= max.x; x++)
                walkable[FlowFieldUtils.Index(new int2(x, y), gridSize)] = 0;
    }

    public bool IsWalkable(int2 cell) =>
    FlowFieldUtils.InBounds(cell, gridSize) &&
    walkable[FlowFieldUtils.Index(cell, gridSize)] != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int2 WorldToCellXZ(Vector3 w) =>
        new int2(math.clamp((int)math.floor((w.x - worldOrigin.x) / cellSize), 0, gridSize.x - 1),
                 math.clamp((int)math.floor((w.z - worldOrigin.y) / cellSize), 0, gridSize.y - 1));
}
