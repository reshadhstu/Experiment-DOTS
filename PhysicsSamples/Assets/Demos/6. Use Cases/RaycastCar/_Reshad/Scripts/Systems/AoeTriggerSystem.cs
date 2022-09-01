using Reshad.Components_Tags;
using Reshad.Components.Data;
using Reshad.DataComponents;
using Unity.Entities;
using Unity.Physics.Stateful;
using Unity.Rendering;

namespace Reshad.Scripts.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
    public partial class AoeTriggerSystem : SystemBase
    {
        private EndFixedStepSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private EntityQueryMask _nonTriggerMask;

        //private float _aoeDamagePower;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

            _nonTriggerMask = EntityManager.GetEntityQueryMask(
                GetEntityQuery(new EntityQueryDesc
                {
                    None = new ComponentType[]
                    {
                        typeof(StatefulTriggerEvent)
                    }
                })
            );
        }

        protected override void OnStartRunning()
        {
            //_aoeDamagePower = GetSingleton<AoeEffectData>().DamagePower;
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            // Need this extra variable here so that it can
            // be captured by Entities.ForEach loop below
            var nonTriggerMask = _nonTriggerMask;

            Entities
                .WithName("AOE_TriggerSystem")
                .WithoutBurst()
                .ForEach((Entity e, ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer, in EnemyHealth enemyHealth) =>
                {
                    for (int i = 0; i < triggerEventBuffer.Length; i++)
                    {
                        var triggerEvent = triggerEventBuffer[i];
                        var otherEntity = triggerEvent.GetOtherEntity(e);

                        // exclude other triggers and processed events
                        if (triggerEvent.State == StatefulEventState.Stay || !nonTriggerMask.Matches(otherEntity))
                        {
                            continue;
                        }

                        if (triggerEvent.State == StatefulEventState.Enter)
                        {
                            //if (_aoeDamagePower > enemyHealth.Value)
                            //{
                            //commandBuffer.AddComponent(otherEntity, new DestroyTag());
                            //_aoeDamagePower -= enemyHealth;
                            //}

                            // var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(e);
                            // var overlappingRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(otherEntity);
                            // overlappingRenderMesh.material = volumeRenderMesh.material;
                            //
                            // commandBuffer.SetSharedComponent(otherEntity, overlappingRenderMesh);

                            //commandBuffer.AddComponent(otherEntity, new DestroyTag());
                        }
                        //The following is what happens on exit
                        // else
                        // {
                        //     commandBuffer.AddComponent(otherEntity, new DestroyTag() {});
                        // }
                    }
                }).Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
