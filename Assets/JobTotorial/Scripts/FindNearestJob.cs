using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct FindNearestJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float3> TargetPositions;
    [ReadOnly]
    public NativeArray<float3> SeekerPositions;

    public NativeArray<float3> NearestTargetPositions;

    //public void Execute()
    //{
    //    int lenght = SeekerPositions.Length;

    //    for (int i = 0; i < lenght; ++i)
    //    {
    //        float3 seekerPosition = SeekerPositions[i];
    //        float nearestDistanceSq = float.MaxValue;
    //        for (int j = 0; j < TargetPositions.Length; j++)
    //        {
    //            float3 targetPos = TargetPositions[j];
    //            float distanceSq = math.distancesq(seekerPosition, targetPos);
    //            if (distanceSq < nearestDistanceSq)
    //            {
    //                nearestDistanceSq = distanceSq;
    //                NearestTargetPositions[i] = targetPos;
    //            }
    //        }
    //    }
    //}

    //public void Execute(int index)
    //{
    //    int lenght = TargetPositions.Length;

    //    float3 seekerPosition = SeekerPositions[index];
    //    float nearestDistanceSq = float.MaxValue;
    //    for (int j = 0; j < lenght; j++)
    //    {
    //        float3 targetPos = TargetPositions[j];
    //        float distanceSq = math.distancesq(seekerPosition, targetPos);
    //        if (distanceSq < nearestDistanceSq)
    //        {
    //            nearestDistanceSq = distanceSq;
    //            NearestTargetPositions[index] = targetPos;
    //        }
    //    }
    //}

    public void Execute(int index)
    {
        float3 seekerPos = SeekerPositions[index];

        int startIndex = TargetPositions.BinarySearch(seekerPos, new AxisXComparer { });

        if (startIndex < 0)
        {
            startIndex = ~startIndex;
        }

        if (startIndex >= TargetPositions.Length)
        {
            startIndex = TargetPositions.Length - 1;
        }

        float3 nearestTargetPos = TargetPositions[startIndex];
        float nearestDistSq = math.distancesq(seekerPos, nearestTargetPos);

        Search(seekerPos, startIndex + 1, TargetPositions.Length, +1, ref nearestTargetPos, ref nearestDistSq);
        Search(seekerPos, startIndex - 1, -1, -1, ref nearestTargetPos, ref nearestDistSq);

        NearestTargetPositions[index] = nearestTargetPos;
    }

    void Search(float3 seekerPosition, int startIndexdx, int endIndex, int step, ref float3 nearestTargetPosition, ref float nearestDistanceSq)
    {
        for (int i = startIndexdx; i != endIndex; i += step)
        {
            float3 targetPos = TargetPositions[i];
            float xdiff = seekerPosition.x - targetPos.x;

            if ((xdiff * xdiff) > nearestDistanceSq)
            {
                break;
            }

            float distSq = math.distancesq(targetPos, seekerPosition);

            if (distSq < nearestDistanceSq)
            {
                nearestDistanceSq = distSq;
                nearestTargetPosition = targetPos;
            }
        }
    }

    public struct AxisXComparer : IComparer<float3>
    {
        public int Compare(float3 lhs, float3 rhs)
        {
            return lhs.x.CompareTo(rhs.x);
        }
    }
}