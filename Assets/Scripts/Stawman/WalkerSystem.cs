using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct WalkerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new WalkerUpdateJob() { deltaTime = SystemAPI.Time.DeltaTime };
        job.ScheduleParallel();
        //var dt = SystemAPI.Time.DeltaTime;

        //foreach (var (walker, xform) in
        //         SystemAPI.Query<RefRO<Walker>,
        //                         RefRW<LocalTransform>>())
        //{
        //    var rot = quaternion.RotateY(walker.ValueRO.angularSpeed * dt);
        //    var fwd = math.mul(rot, xform.ValueRO.Forward());
        //    xform.ValueRW.Position += fwd * walker.ValueRO.forwardSpeed * dt;
        //    xform.ValueRW.Rotation = quaternion.LookRotation(fwd, xform.ValueRO.Up());
        //}
    }
}

public partial struct WalkerUpdateJob : IJobEntity
{
    public float deltaTime;

    public void Execute(in Walker walker, ref LocalTransform localTransform)
    {
        var rot = quaternion.RotateY(walker.angularSpeed * deltaTime);
        var fwd = math.mul(rot, localTransform.Forward());
        localTransform.Position += fwd * walker.forwardSpeed * deltaTime;
        localTransform.Rotation = quaternion.LookRotation(fwd, localTransform.Up());
    }
}
