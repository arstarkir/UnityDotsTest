using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;

[UpdateAfter(typeof(Spawner))]
public partial struct MoverSys : ISystem
{
    public partial struct MovementJob : IJobEntity
    {
        public float3 to;

        void Execute( )
        {

        }
    }
}