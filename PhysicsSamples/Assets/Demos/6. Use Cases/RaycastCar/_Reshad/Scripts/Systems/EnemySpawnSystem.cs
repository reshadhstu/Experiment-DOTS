using Reshad.Components.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Reshad.Systems
{
    public partial class EnemySpawnSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithName("EnemySpawnSystem")
                .WithBurst(synchronousCompilation: true)
                .ForEach((Entity entity, int entityInQueryIndex, in EnemySpawnData enemySpawnData, in LocalToWorld ltw) =>
                {
                    for (int i = 0; i < enemySpawnData.XGridCount; i++)
                    {
                        for (int j = 0; j < enemySpawnData.ZGridCount; j++)
                        {
                            var newEntity = commandBuffer.Instantiate(entityInQueryIndex, enemySpawnData.PrefabToSpawn);

                            var position = new float3(i * enemySpawnData.XPadding, enemySpawnData.BaseOffset, j * enemySpawnData.ZPadding);

                            commandBuffer.SetComponent(entityInQueryIndex, newEntity, new Translation {Value = position});
                        }
                    }
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
