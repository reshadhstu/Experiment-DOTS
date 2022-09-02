using System;
using Unity.Entities;

namespace Reshad.Components.Data
{
    [Serializable]
    public struct ProjectileLauncherData : IComponentData
    {
        public Entity Missile;
        public float Strength;
        public float Rate;
        public float Duration;

        public int WasFiring;
        public int IsFiring;
    }
}
