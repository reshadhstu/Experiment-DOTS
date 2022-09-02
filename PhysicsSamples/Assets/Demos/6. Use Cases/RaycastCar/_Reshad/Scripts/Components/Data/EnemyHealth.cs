using Unity.Entities;

namespace Reshad.Components.Data
{
    [GenerateAuthoringComponent]
    public struct EnemyHealth : IComponentData
    {
        public float Value;
    }
}
