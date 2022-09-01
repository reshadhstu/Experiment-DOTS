using System.Collections.Generic;
using Reshad.DataComponents;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Reshad.Authoring
{
    [DisallowMultipleComponent]
    public class SetSpawnData : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject enemyPrefab;
        public int enemyCount = 50;
        public Vector3 minSpawnPosition;
        public Vector3 maxSpawnPosition;

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(enemyPrefab);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var spawnData = new SpawnEnemyData
            {
                EnemyPrefab = conversionSystem.GetPrimaryEntity(enemyPrefab),
                Count = enemyCount,
                Random = Random.CreateFromIndex(0),
                MinSpawnPosition = minSpawnPosition,
                MaxSpawnPosition = maxSpawnPosition
            };

            dstManager.AddComponentData(entity, spawnData);
        }
    }
}
