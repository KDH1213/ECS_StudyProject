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

        // ���� �� �ڵ��� Schedule()�� �����ϸ�
        // ã�� ���� ���� �⿡ ���ӵ˴ϴ�. �̴� ���� ���� �Ϸ�� �Ŀ��� 
        // ã�� �� ������ ���۵��� �ǹ��մϴ�.
        // ���� ����� Ÿ�� ã�� ���� ������ �Ϸ�Ǳ⸦ 
        // ��ٷ��� �ϹǷ� ���� �⿡ ���ӵ˴ϴ�. 
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