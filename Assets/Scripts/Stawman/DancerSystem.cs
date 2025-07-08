using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct DancerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new DancerUpdateJob() { Elapsed = (float)SystemAPI.Time.ElapsedTime };
        job.ScheduleParallel();

        //var elapsed = (float)SystemAPI.Time.ElapsedTime;

        //foreach (var (dancer, localTransform) in SystemAPI.Query<RefRO<Dancer>, RefRW<LocalTransform>>())
        //{
        //    var time = dancer.ValueRO.speed * elapsed;
        //    var y = math.abs(math.sin(time)) * 0.1f;
        //    var bank = math.cos(time) * 0.5f;

        //    var forward = localTransform.ValueRO.Forward();
        //    var rotation = quaternion.AxisAngle(forward, bank);
        //    var up = math.mul(rotation, math.float3(0, 1, 0));

        //    localTransform.ValueRW.Position.y = y;
        //    localTransform.ValueRW.Rotation = quaternion.LookRotation(forward, up);
        //}
    }
}

public partial struct DancerUpdateJob : IJobEntity
{
    public float Elapsed;

    public void Execute(in Dancer dancer, ref LocalTransform localTransform)
    {
        var time = dancer.speed * Elapsed;
        var y = math.abs(math.sin(time)) * 0.1f;
        var bank = math.cos(time) * 0.5f;

        var forward = localTransform.Forward();
        var rotation = quaternion.AxisAngle(forward, bank);
        var up = math.mul(rotation, math.float3(0, 1, 0));

        localTransform.Position.y = y;
        localTransform.Rotation = quaternion.LookRotation(forward, up);
    }
}
