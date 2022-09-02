using System.Collections.Generic;
using Reshad.Components.Data;
using Unity.Entities;
using UnityEngine;

namespace Reshad.MonoAuthoring
{
    [DisallowMultipleComponent]
    public class SetAoeData : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject aoePrefab;
        public float damageRadius;
        [Range(30, 600)]
        public float damagePower;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new AoeEffectData()
            {
                AoePrefab = conversionSystem.GetPrimaryEntity(aoePrefab),
                DamageRadius = damageRadius
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
