using System.Collections.Generic;
using Reshad.Components.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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

            // Physics and graphics representations of bodies can be largely independent.
            // Positions and Rotations of each representation are associated through the BuildPhysicsWorld & ExportPhysicsWorld systems.
            // As scale is generally baked for runtime performance, we specifically need to add a scale component here
            // and will update both the graphical and physical scales in our own demo update system.
            // dstManager.AddComponentData(entity, new Scale
            // {
            //     Value = 1.0f,
            // });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(aoePrefab);
        }
    }
}
