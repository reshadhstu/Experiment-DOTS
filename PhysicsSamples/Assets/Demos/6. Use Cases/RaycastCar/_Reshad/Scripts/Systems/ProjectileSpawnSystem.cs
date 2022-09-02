using Reshad.Components.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Reshad.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ProjectileSpawnSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            var isGunFiring = 0;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isGunFiring = 1;
            }

            var deltaTime = Time.DeltaTime;

            Entities
                .WithName("ProjectileSystemJob")
                .WithBurst()
                .ForEach((Entity _, int entityInQueryIndex, ref Rotation gunRotation, ref ProjectileLauncherData gun, in LocalToWorld gunTransform) =>
                {
                    gun.IsFiring = isGunFiring;

                    if (gun.IsFiring == 0)
                    {
                        gun.Duration = 0;
                        gun.WasFiring = 0;
                        return;
                    }

                    gun.Duration += deltaTime;

                    if ((gun.Duration > gun.Rate) || (gun.WasFiring == 0))
                    {
                        if (gun.Missile != null)
                        {
                            var newEntity = commandBuffer.Instantiate(entityInQueryIndex, gun.Missile);

                            var position = new Translation { Value = gunTransform.Position + gunTransform.Forward };

                            var rotation = new Rotation { Value = gunRotation.Value };

                            var velocity = new PhysicsVelocity
                            {
                                Linear = gunTransform.Forward * gun.Strength,
                                Angular = float3.zero
                            };

                            commandBuffer.SetComponent(entityInQueryIndex, newEntity, position);
                            commandBuffer.SetComponent(entityInQueryIndex, newEntity, rotation);
                            commandBuffer.SetComponent(entityInQueryIndex, newEntity, velocity);
                        }

                        gun.Duration = 0;
                    }

                    gun.WasFiring = 1;
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
