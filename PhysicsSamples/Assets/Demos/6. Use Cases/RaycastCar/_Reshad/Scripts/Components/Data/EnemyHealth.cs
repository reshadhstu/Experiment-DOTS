using System;
using Unity.Entities;

namespace Reshad.DataComponents
{
    [GenerateAuthoringComponent]
    public struct EnemyHealth : IComponentData
    {
        public int Value;
    }
}
