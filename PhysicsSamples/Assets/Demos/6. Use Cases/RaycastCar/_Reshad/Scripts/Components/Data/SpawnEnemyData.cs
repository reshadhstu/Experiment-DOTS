using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Reshad.DataComponents
{
    [GenerateAuthoringComponent]
    public struct SpawnEnemyData : IComponentData
    {
        public Entity EnemyPrefab;
        public int Count;

        public float3 MinSpawnPosition;
        public float3 MaxSpawnPosition;

        public Random Random;
        public float3 RandomSpawnPos => Random.NextFloat3(MinSpawnPosition, MaxSpawnPosition);
    }
}
