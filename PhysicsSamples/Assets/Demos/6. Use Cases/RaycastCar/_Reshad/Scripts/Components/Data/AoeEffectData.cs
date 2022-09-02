using Unity.Entities;

namespace Reshad.Components.Data
{
    public struct AoeEffectData : IComponentData
    {
        public Entity AoePrefab;
        public float DamageRadius;
        public float Target;
    }
}
