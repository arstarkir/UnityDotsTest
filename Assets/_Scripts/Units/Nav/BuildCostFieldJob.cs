using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public static class FlowFieldUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Index(int2 p, int2 size) => p.x + p.y * size.x;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InBounds(int2 p, int2 size) =>
        math.all(p >= 0) & math.all(p < size);
}

[BurstCompile]
public struct BuildCostFieldJob : IJobParallelFor
{
    public int2 size;
    [ReadOnly] public NativeArray<byte> walkable;
    [WriteOnly] public NativeArray<int> cost;

    public void Execute(int i)
    {
        cost[i] = walkable[i] == 0 ? int.MaxValue : int.MaxValue - 1;
    }
}

[BurstCompile]
public struct IntegrateFieldJob : IJob
{
    public int2 size;
    public int2 goal;
    public NativeArray<int> cost;

    static readonly int2[] Neigh = { new int2( 1,0), new int2(-1,0),
                                     new int2( 0,1), new int2( 0,-1) };
    public void Execute()
    {
        var frontier = new NativeQueue<int2>(Allocator.Temp);

        int goalIdx = FlowFieldUtils.Index(goal, size);
        cost[goalIdx] = 0;
        frontier.Enqueue(goal);

        while (frontier.TryDequeue(out var cell))
        {
            int baseCost = cost[FlowFieldUtils.Index(cell, size)];

            for (int n = 0; n < 4; n++)
            {
                int2 nb = cell + Neigh[n];
                if (!FlowFieldUtils.InBounds(nb, size)) continue;

                int nbIdx = FlowFieldUtils.Index(nb, size);

                if (cost[nbIdx] == int.MaxValue) continue;

                if (cost[nbIdx] > baseCost + 1)
                {
                    cost[nbIdx] = baseCost + 1;
                    frontier.Enqueue(nb);
                }
            }
        }
        frontier.Dispose();
    }
}

[BurstCompile]
public struct BuildFlowFieldJob : IJobParallelFor
{
    public int2 size;
    [ReadOnly] public NativeArray<int> cost;
    [WriteOnly] public NativeArray<float2> dir;

    static readonly int2[] Neigh = {
        new int2( 1, 0), new int2(-1, 0), new int2( 0, 1), new int2( 0,-1),
        new int2( 1, 1), new int2(-1, 1), new int2( 1,-1), new int2(-1,-1) };

    public void Execute(int index)
    {
        int2 cell = new int2(index % size.x, index / size.x);

        int cCost = cost[index];
        if (cCost == 0 || cCost == int.MaxValue)
        {
            dir[index] = float2.zero;
            return;
        }

        int bestCost = cCost;
        int2 bestStep = int2.zero;

        for (int n = 0; n < Neigh.Length; n++)
        {
            int2 nb = cell + Neigh[n];
            if (!FlowFieldUtils.InBounds(nb, size)) continue;

            int nbCost = cost[FlowFieldUtils.Index(nb, size)];
            if (nbCost < bestCost)
            {
                bestCost = nbCost;
                bestStep = Neigh[n];
            }
        }

        dir[index] = math.normalizesafe(bestStep);
    }

}