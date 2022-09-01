using System;
using Unity.Entities;

namespace Reshad.Components_Tags
{
    [Serializable]
    public struct ProjectileLauncherData : IComponentData
    {
        public Entity Bullet;
        public float Strength;
        public float Rate;
        public float Duration;

        public int WasFiring;
        public int IsFiring;
    }
}
