/* TURN TOWARDS PLAYER SYSTEM
 * This system finds any entity that has a LocalTransform and EnemyTag component 
 * and turns the entity to point its Z axis towards a target (in this case, the player). 
 * In this project, the one entity type it will affect are enemies
 */

using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(MoveForwardSystem))]
// Timing goes here 
partial struct TurnTowardsPlayerSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<EnemyTag>();
	}

	//[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		if(Settings.IsPlayerDead())
		{
			return;
		}

		var TurnTowardsTargetJob = new TurnTowardsTargetJob
		{
			targetPosition = Settings.PlayerPosition
		};

		TurnTowardsTargetJob.ScheduleParallel();
	}
}

[BurstCompile]
public partial struct TurnTowardsTargetJob : IJobEntity
{
	public float3 targetPosition;

	public void Execute(ref LocalTransform localTransform)
	{
		float3 heading = targetPosition - localTransform.Position;
		heading.y = 0f;
        localTransform.Rotation = quaternion.LookRotation(heading, math.up());
	}
}

//float3 heading = targetPosition - transform.Position;
//heading.y = 0f;
//transform.Rotation = quaternion.LookRotation(heading, math.up());