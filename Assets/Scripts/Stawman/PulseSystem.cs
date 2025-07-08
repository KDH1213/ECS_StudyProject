using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PulseSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new PulseUpdateJob() { ElapsedTime = (float)SystemAPI.Time.ElapsedTime };
        job.ScheduleParallel();

        // var elapsed = (float)SystemAPI.Time.ElapsedTime;
        //foreach (var (dancer, walker, xform) in
        //         SystemAPI.Query<RefRO<Dancer>,
        //                         RefRO<Walker>,
        //                         RefRW<LocalTransform>>())
        //{
        //    var t = dancer.ValueRO.speed * elapsed;
        //    xform.ValueRW.Scale = 1.1f - 0.3f * math.abs(math.cos(t));
        //}
    }
}

public partial struct PulseUpdateJob : IJobEntity
{
    public float ElapsedTime;

    public void Execute(in Dancer dancer, in Walker walker, ref LocalTransform localTransform)
    {
        var t = dancer.speed * ElapsedTime;
        localTransform.Scale = 1.1f - 0.3f * math.abs(math.cos(t));
    }
}
