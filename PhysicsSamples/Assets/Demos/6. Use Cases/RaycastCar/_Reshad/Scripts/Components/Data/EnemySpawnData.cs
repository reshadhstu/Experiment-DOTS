using Unity.Entities;

namespace Reshad.Components.Data
{
    [GenerateAuthoringComponent]
    public struct EnemySpawnData : IComponentData
    {
        public int XGridCount;
        public int ZGridCount;
        public float BaseOffset;
        public float XPadding;
        public float ZPadding;
        public Entity PrefabToSpawn;
    }
}
