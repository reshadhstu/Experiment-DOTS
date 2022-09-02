using System.Collections.Generic;
using Reshad.Components.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Reshad.MonoAuthoring
{
    [DisallowMultipleComponent]
    public class SetAoeData : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject aoePrefab;
        public float damageRadius;
        public float damagePower;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new AoeEffectData()
            {
                AoePrefab = conversionSystem.GetPrimaryEntity(aoePrefab),
                DamageRadius = damageRadius,
                Target = math.lerp(0, damageRadius, 0.5f)
            });

            dstManager.AddComponentData(entity, new AoePowerData()
            {
                DamagePower = damagePower
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(aoePrefab);
        }
    }
}
