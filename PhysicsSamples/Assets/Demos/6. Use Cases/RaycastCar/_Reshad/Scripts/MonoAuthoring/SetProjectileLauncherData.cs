using System.Collections.Generic;
using Reshad.Components_Tags;
using Unity.Entities;
using UnityEngine;

namespace Reshad.Authoring_Mono
{
    [DisallowMultipleComponent]
    public class SetProjectileLauncherData : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject Bullet;

        public float Strength;
        public float Rate;

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(Bullet);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(
                entity,
                new ProjectileLauncherData()
                {
                    Bullet = conversionSystem.GetPrimaryEntity(Bullet),
                    Strength = Strength,
                    Rate = Rate,
                    WasFiring = 0,
                    IsFiring = 0
                });
        }
    }
}
