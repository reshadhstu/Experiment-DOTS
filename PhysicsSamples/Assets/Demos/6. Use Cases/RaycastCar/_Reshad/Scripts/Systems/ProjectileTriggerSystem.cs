using Reshad.Components.Data;
using Reshad.Components.Tag;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using Unity.Transforms;

namespace Reshad.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
    public partial class ProjectileTriggerSystem : SystemBase
    {
        private EndFixedStepSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private EntityQueryMask _nonTriggerMask;

        private Entity _aoeEntity;

        private bool _isAoeCreated;

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

            RequireSingletonForUpdate<MissileTag>();
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
                .ForEach((Entity entity, ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer) =>
                {
                    for (int i = 0; i < triggerEventBuffer.Length; i++)
                    {
                        var triggerEvent = triggerEventBuffer[i];
                        var otherEntity = triggerEvent.GetOtherEntity(entity);

                        // exclude other triggers and processed events
                        if (triggerEvent.State == StatefulEventState.Stay || !nonTriggerMask.Matches(otherEntity))
                        {
                            continue;
                        }

                        if (triggerEvent.State == StatefulEventState.Enter)
                        {
                            if (!_isAoeCreated)
                            {
                                var entityPositionComponent = GetComponent<Translation>(entity);

                                var newTranslation = new Translation()
                                {
                                    Value = new float3(entityPositionComponent.Value.x, entityPositionComponent.Value.y, entityPositionComponent.Value.z)
                                };

                                var newEntity = commandBuffer.Instantiate(_aoeEntity);

                                var newScale = new Scale {Value = 1.0f};
                                commandBuffer.AddComponent<Scale>(newEntity);


                                commandBuffer.SetComponent(newEntity, newTranslation);
                                commandBuffer.SetComponent(newEntity, newScale);

                                commandBuffer.AddComponent(entity, new DestroyEntityTag());

                                _isAoeCreated = true;
                            }
                        }
                    }
                    _isAoeCreated = false;
                }).Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
