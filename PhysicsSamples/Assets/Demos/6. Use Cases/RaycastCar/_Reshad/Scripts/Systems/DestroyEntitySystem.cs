using Reshad.Components.Tag;
using Unity.Entities;

namespace Reshad.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class DestroyEntitySystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithName("DestroyEntitySystem")
                .WithAll<DestroyEntityTag>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
