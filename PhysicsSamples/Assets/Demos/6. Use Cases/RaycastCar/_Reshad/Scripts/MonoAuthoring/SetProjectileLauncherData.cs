using System.Collections.Generic;
using Reshad.Components.Data;
using Unity.Entities;
using UnityEngine;

namespace Reshad.MonoAuthoring
{
    [DisallowMultipleComponent]
    public class SetProjectileLauncherData : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject missile;

        public float strength;
        public float rate;

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(missile);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(
                entity,
                new ProjectileLauncherData()
                {
                    Missile = conversionSystem.GetPrimaryEntity(missile),
                    Strength = strength,
                    Rate = rate,
                    WasFiring = 0,
                    IsFiring = 0
                });
        }
    }
}
