using Reshad.Components.Data;
using Reshad.Components.Tag;
using Unity.Entities;
using Unity.Physics.Stateful;
using UnityEngine;

namespace Reshad.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
    [UpdateAfter(typeof(ProjectileTriggerSystem))]
    public partial class AoeTriggerSystem : SystemBase
    {
        private EndFixedStepSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private EntityQueryMask _nonTriggerMask;

        private float _aoeDamagePower;

        private bool _hitEnemy;

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

            RequireSingletonForUpdate<AoeTag>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            _aoeDamagePower = GetSingleton<AoePowerData>().DamagePower;

            // Need this extra variable here so that it can
            // be captured by Entities.ForEach loop below
            var nonTriggerMask = _nonTriggerMask;

            Entities
                .WithName("AOE_TriggerSystem")
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
                            if (!HasComponent<EnemyTag>(otherEntity) || _aoeDamagePower <= 0) return;

                            if (!_hitEnemy)
                            {
                                var enemyHealthData = GetComponent<EnemyHealth>(otherEntity);

                                var enemyHealth = enemyHealthData.Value;

                                if (_aoeDamagePower >= enemyHealthData.Value)
                                {
                                    commandBuffer.AddComponent(otherEntity, new DestroyEntityTag());
                                }
                                else
                                {
                                    enemyHealthData.Value -= _aoeDamagePower;
                                    SetComponent(otherEntity, enemyHealthData);
                                }

                                SetSingleton(new AoePowerData {DamagePower = _aoeDamagePower - enemyHealth});

                                _hitEnemy = true;
                            }
                        }
                    }
                    _hitEnemy = false;
                }).Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
