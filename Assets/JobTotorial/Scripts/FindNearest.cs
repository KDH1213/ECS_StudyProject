using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static FindNearestJob;

public class FindNearest : MonoBehaviour
{
    NativeArray<float3> TargetPositions;
    NativeArray<float3> SeekerPositions;
    NativeArray<float3> NearestTargetPositions;

    public void Start()
    {
        Spawner spawner = Object.FindObjectOfType<Spawner>();
        
        // We use the Persistent allocator because these arrays must
        // exist for the run of the program.
        TargetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
        SeekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        NearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        TargetPositions.Dispose();
        SeekerPositions.Dispose();
        NearestTargetPositions.Dispose();
    }

    public void Update()
    {
        FindTarget_Job();
    }

    public void FindTarget_Job()
    {
        for (int i = 0; i < TargetPositions.Length; i++)
        {
            TargetPositions[i] = Spawner.TargetTransforms[i].localPosition;
        }

        for (int i = 0; i < SeekerPositions.Length; i++)
        {
            SeekerPositions[i] = Spawner.SeekerTransforms[i].localPosition;
        }

        SortJob<float3, AxisXComparer> sortJob = TargetPositions.SortJob(new AxisXComparer { });
        JobHandle sortHandle = sortJob.Schedule();

        FindNearestJob findJob = new FindNearestJob
        {
            TargetPositions = TargetPositions,
            SeekerPositions = SeekerPositions,
            NearestTargetPositions = NearestTargetPositions,
        };

        // 정렬 잡 핸들을 Schedule()에 전달하면
        // 찾기 잡이 정렬 잡에 종속됩니다. 이는 정렬 잡이 완료된 후에만 
        // 찾기 잡 실행이 시작됨을 의미합니다.
        // 가장 가까운 타겟 찾기 잡은 정렬이 완료되기를 
        // 기다려야 하므로 정렬 잡에 종속됩니다. 
        JobHandle findHandle = findJob.Schedule(SeekerPositions.Length, 100, sortHandle);
        findHandle.Complete();


        for (int i = 0; i < SeekerPositions.Length; i++)
        {
            Debug.DrawLine(SeekerPositions[i], NearestTargetPositions[i]);
        }
    }

    public void FindTarget_Basic()
    {
        foreach (var seekerTransform in Spawner.SeekerTransforms)
        {
            Vector3 seekerPos = seekerTransform.localPosition;
            Vector3 nearestTargetPos = default;
            float nearestDistSq = float.MaxValue;
            foreach (var targetTransform in Spawner.TargetTransforms)
            {
                Vector3 offset = targetTransform.localPosition - seekerPos;
                float distSq = offset.sqrMagnitude;

                if (distSq < nearestDistSq)
                {
                    nearestDistSq = distSq;
                    nearestTargetPos = targetTransform.localPosition;
                }
            }

            Debug.DrawLine(seekerPos, nearestTargetPos);
        }
    }
}