using Reshad.Components_Tags;
using Reshad.Components.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using Unity.Transforms;

//We did not need the ReferenceEntity so we deleted the IConvertGameObjectToEntity interface
//and the TriggerVolumeChangeMaterial component

namespace Reshad.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
    public partial class ProjectileTriggerSystem : SystemBase
    {
        private EndFixedStepSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private EntityQueryMask _nonTriggerMask;

        private Entity _aoeEntity;

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
            _aoeEntity = GetSingleton<AoeEffectData>().AoePrefab;
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            // Need this extra variable here so that it can
            // be captured by Entities.ForEach loop below
            var nonTriggerMask = _nonTriggerMask;

            Entities
                .WithName("ProjectileTriggerSystem")
                .WithoutBurst()
                .ForEach((Entity e, ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer) =>
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
                            var entityPositionComponent = GetComponent<Translation>(e);

                            var newTranslation = new Translation()
                            {
                                Value = new float3(entityPositionComponent.Value.x, 0, entityPositionComponent.Value.z)
                            };

                            var newEntity = commandBuffer.Instantiate(_aoeEntity);

                            var newScale = new Scale {Value = 1};
                            commandBuffer.AddComponent<Scale>(newEntity);


                            commandBuffer.SetComponent(newEntity, newTranslation);
                            commandBuffer.SetComponent(newEntity, newScale);

                            commandBuffer.AddComponent(e, new DestroyTag());
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
