using Reshad.Components_Tags;
using Reshad.Components.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

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

            Entities.ForEach((Entity entity, ref PhysicsCollider collider, ref Scale scale, in AoeTag aoeTag) =>
            {
                // make sure we are dealing with spheres
                if (collider.Value.Value.Type != ColliderType.Sphere) return;

                unsafe
                {
                    SphereCollider* scPtr = (SphereCollider*)collider.ColliderPtr;

                    // update the collider geometry
                    var sphereGeometry = scPtr->Geometry;
                    sphereGeometry.Radius = targetRadius;
                    scPtr->Geometry = sphereGeometry;
                }

                scale.Value = math.lerp(scale.Value, targetRadius, 0.1f);

                if (math.abs(scale.Value - targetRadius) < 0.01f)
                {
                    commandBuffer.AddComponent<DestroyTag>(entity);
                }
            }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
