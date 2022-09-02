using Reshad.DataComponents;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Reshad.Systems
{
    //[UpdateBefore(typeof(TransformSystemGroup))]
    public partial class EnemySpawnSystem : SystemBase
    {
        //This will be our query for Asteroids
        private EntityQuery _enemyEntityQuery;

        //We will use the BeginSimulationEntityCommandBufferSystem for our structural changes
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        //This will be our query to find GameSettingsComponent data to know how many and where to spawn Asteroids
        private EntityQuery _spawnDataQuery;

        //This will save our Asteroid prefab to be used to spawn Asteroids
        private Entity _enemyPrefab;

        protected override void OnCreate()
        {
            //This is an EntityQuery for our Asteroids, they must have an AsteroidTag
            _enemyEntityQuery = GetEntityQuery(ComponentType.ReadWrite<EnemyTag>());

            //This is an EntityQuery for the GameSettingsComponent which will drive how many Asteroids we spawn
            _spawnDataQuery = GetEntityQuery(ComponentType.ReadWrite<SpawnEnemyData>());

            //This will grab the BeginSimulationEntityCommandBuffer system to be used in OnUpdate
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            //This says "do not go to the OnUpdate method until an entity exists that meets this query"
            //We are using GameObjectConversion to create our GameSettingsComponent so we need to make sure
            //The conversion process is complete before continuing
            RequireForUpdate(_spawnDataQuery);
        }

        protected override void OnUpdate()
        {
            //Here we set the prefab we will use
            if (_enemyPrefab == Entity.Null)
            {
                //We grab the converted PrefabCollection Entity's AsteroidAuthoringComponent
                //and set m_Prefab to its Prefab value
                _enemyPrefab = GetSingleton<SpawnEnemyData>().EnemyPrefab;

                //we must "return" after setting this prefab because if we were to continue into the Job
                //we would run into errors because the variable was JUST set (ECS funny business)
                //comment out return and see the error
                return;
            }

            //Because of how ECS works we must declare local variables that will be used within the job
            //You cannot "GetSingleton<GameSettingsComponent>()" from within the job, must be declared outside
            var settings = GetSingleton<SpawnEnemyData>();

            //Here we create our commandBuffer where we will "record" our structural changes (creating an Asteroid)
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            //This provides the current amount of Asteroids in the EntityQuery
            var count = _enemyEntityQuery.CalculateEntityCountWithoutFiltering();

            //We must declare our prefab as a local variable (ECS funny business)
            var asteroidPrefab = _enemyPrefab;


            Job
                .WithCode(() =>
            {
                for (int i = count; i < settings.Count; ++i)
                {
                    //we then create a new translation component with the randomly generated x, y, and z values
                    var newTranslation = new Translation()
                    {
                        Value = settings.RandomSpawnPos
                    };

                    //on our command buffer we record creating an entity from our Asteroid prefab
                    var newEntity = commandBuffer.Instantiate(asteroidPrefab);

                    //we then set the Translation component of the Asteroid prefab equal to our new translation component
                    commandBuffer.SetComponent(newEntity, newTranslation);
                }
            }).Schedule();

            //This will add our dependency to be played back on the BeginSimulationEntityCommandBuffer
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
