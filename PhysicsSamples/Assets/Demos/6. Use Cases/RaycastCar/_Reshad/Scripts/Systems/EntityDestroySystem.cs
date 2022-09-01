using Reshad.Components_Tags;
using Unity.Entities;

namespace Reshad.Systems
{
    //We are going to update LATE once all other systems are complete
//because we don't want to destroy the Entity before other systems have
//had a chance to interact with it if they need to
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class EntityDestroySystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            //We grab the EndSimulationEntityCommandBufferSystem to record our structural changes
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            //We add "AsParallelWriter" when we create our command buffer because we want
            //to run our jobs in parallel
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            //We now any entities with a DestroyTag and an AsteroidTag
            //We could just query for a DestroyTag, but we might want to run different processes
            //if different entities are destroyed, so we made this one specifically for Asteroids
            Entities
                .WithAll<DestroyTag>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                }).ScheduleParallel();

            //We then add the dependencies of these jobs to the EndSimulationEntityCOmmandBufferSystem
            //that will be playing back the structural changes recorded in this sytem
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
