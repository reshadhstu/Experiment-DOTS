using System.Collections.Generic;
using Reshad.Components.Data;
using Unity.Entities;
using UnityEngine;

namespace Reshad.MonoAuthoring
{
    [DisallowMultipleComponent]
    public class SetEnemySpawnData : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject enemyPrefab;
        public int xGridCount;
        public int zGridCount;
        public float baseOffset;
        public float xPadding;
        public float zPadding;

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(enemyPrefab);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var spawnData = new EnemySpawnData()
            {
                PrefabToSpawn = conversionSystem.GetPrimaryEntity(enemyPrefab),
                XGridCount = xGridCount,
                ZGridCount = zGridCount,
                BaseOffset = baseOffset,
                XPadding = xPadding,
                ZPadding = zPadding
            };

            dstManager.AddComponentData(entity, spawnData);
        }
    }
}
