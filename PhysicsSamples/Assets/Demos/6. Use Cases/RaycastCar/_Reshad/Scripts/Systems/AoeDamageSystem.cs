using Reshad.Components.Data;
using Reshad.Components.Tag;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using SphereCollider = Unity.Physics.SphereCollider;

namespace Reshad.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public partial class AoeDamageSystem : SystemBase
    {
        private float _damageRadius;

        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            _damageRadius = GetSingleton<AoeEffectData>().DamageRadius;
        }

        protected override void OnUpdate()
        {
            var targetRadius = _damageRadius;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            Entities
                .WithName("AoeDamageSystem")
                .WithBurst()
                .ForEach((Entity entity, ref PhysicsCollider collider, ref Scale scale, in AoeTag aoeTag) =>
                {
                    // make sure we are dealing with spheres
                    if (collider.Value.Value.Type != ColliderType.Sphere) return;

                    unsafe
                    {
                        var scPtr = (SphereCollider*)collider.ColliderPtr;

                        var oldRadius = scPtr->Radius;
                        var newRadius = math.lerp(oldRadius, targetRadius, 0.05f);

                        // update the collider geometry
                        var sphereGeometry = scPtr->Geometry;
                        sphereGeometry.Radius = newRadius;
                        scPtr->Geometry = sphereGeometry;
                    }

                    var newScale = targetRadius * 2f;

                    scale.Value = math.lerp(scale.Value, newScale, 0.05f);

                    if (math.abs(scale.Value - newScale) < 0.1f)
                    {
                        commandBuffer.AddComponent<DestroyEntityTag>(entity);
                    }
                }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
