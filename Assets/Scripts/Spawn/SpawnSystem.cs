using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state) => state.RequireForUpdate<Config>();
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var instances = state.EntityManager.Instantiate(config.Prefab, config.SpawnCount, Allocator.Temp);
        var random = new Random(config.RandomSeed);

        foreach (var entity in instances)
        {
            var localTransform = SystemAPI.GetComponentRW<LocalTransform>(entity);
            var dancer = SystemAPI.GetComponentRW<Dancer>(entity);
            var walker = SystemAPI.GetComponentRW<Walker>(entity);

            localTransform.ValueRW = LocalTransform.FromPositionRotation(random.NextOnDisk() * config.SpawnRadius, random.NextYRotation());

            dancer.ValueRW = Dancer.Random(random.NextUInt());
            walker.ValueRW = Walker.Random(random.NextUInt());
        }

        state.Enabled = false;
    }
}

