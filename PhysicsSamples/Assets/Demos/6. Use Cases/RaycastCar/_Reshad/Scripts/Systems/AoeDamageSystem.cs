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

        private float _damageTarget;

        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            _damageRadius = GetSingleton<AoeEffectData>().DamageRadius;
            _damageTarget = GetSingleton<AoeEffectData>().Target;
        }

        protected override void OnUpdate()
        {
            var targetRadius = _damageRadius;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            var target = GetSingleton<AoeEffectData>().Target;

            Entities.WithBurst().ForEach((Entity entity, ref PhysicsCollider collider, ref Scale scale, in AoeTag aoeTag) =>
            {
                // make sure we are dealing with spheres
                if (collider.Value.Value.Type != ColliderType.Sphere) return;

                float oldRadius = 1.0f;
                float newRadius = 1.0f;

                unsafe
                {
                    SphereCollider* scPtr = (SphereCollider*)collider.ColliderPtr;

                    oldRadius = scPtr->Radius;
                    newRadius = math.lerp(oldRadius, target, 0.05f);

                    // update the collider geometry
                    var sphereGeometry = scPtr->Geometry;
                    sphereGeometry.Radius = newRadius;
                    scPtr->Geometry = sphereGeometry;
                }
                //
                // scale.Value = math.lerp(scale.Value, targetRadius, 0.05f);
                //
                // if (math.abs(scale.Value - targetRadius) < 0.01f)
                // {
                //     //commandBuffer.AddComponent<DestroyEntityTag>(entity);
                // }

                // now tweak the graphical representation of the sphere
                float oldScale = scale.Value;
                float newScale = oldScale;
                if (oldRadius == 0.0f)
                {
                    // avoid the divide by zero errors.
                    newScale = newRadius;
                }
                else
                {
                    newScale *= newRadius / oldRadius;
                }
                scale.Value = newScale;

                if (math.abs(scale.Value - targetRadius) < 0.01f)
                {
                    commandBuffer.AddComponent<DestroyEntityTag>(entity);
                }
            }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

            // Entities
            //     .WithName("AoeDamageSystem")
            //     .WithBurst()
            //     .ForEach((ref PhysicsCollider collider, ref Scale scale) =>
            //     {
            //         // make sure we are dealing with spheres
            //         if (collider.Value.Value.Type != ColliderType.Sphere) return;
            //
            //         // tweak the physical representation of the sphere
            //
            //         // NOTE: this approach affects all instances using the same BlobAsset
            //         // so you cannot simply use this approach for instantiated prefabs
            //         // if you want to modify prefab instances independently, you need to create
            //         // unique BlobAssets at run-time and dispose them when you are done
            //
            //         float oldRadius = 1.0f;
            //         float newRadius = 1.0f;
            //         unsafe
            //         {
            //             // grab the sphere pointer
            //             SphereCollider* scPtr = (SphereCollider*)collider.ColliderPtr;
            //             oldRadius = scPtr->Radius;
            //             newRadius = math.lerp(oldRadius, _damageTarget, 0.05f);
            //             // if we have reached the target radius get a new target
            //             if (math.abs(newRadius - _damageTarget) < 0.01f)
            //             {
            //                 _damageTarget = _damageTarget == 0 ? _damageRadius : 0;
            //             }
            //
            //             // update the collider geometry
            //             var sphereGeometry = scPtr->Geometry;
            //             sphereGeometry.Radius = newRadius;
            //             scPtr->Geometry = sphereGeometry;
            //         }
            //
            //         // now tweak the graphical representation of the sphere
            //         float oldScale = scale.Value;
            //         float newScale = oldScale;
            //         if (oldRadius == 0.0f)
            //         {
            //             // avoid the divide by zero errors.
            //             newScale = newRadius;
            //         }
            //         else
            //         {
            //             newScale *= newRadius / oldRadius;
            //         }
            //         scale.Value = newScale;
            //     }).Schedule();
        }
    }
}
