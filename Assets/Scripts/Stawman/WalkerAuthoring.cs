using Unity.Entities;
using UnityEngine;

public struct Walker : IComponentData
{
    public float forwardSpeed;
    public float angularSpeed;

    public static Walker Random(uint seed)
    {
        var random = new Unity.Mathematics.Random(seed);
        return new Walker()
        {
            forwardSpeed = random.NextFloat(0.1f, 0.8f),
            angularSpeed = random.NextFloat(0.5f, 4f)};
    }
}

public class WalkerAuthoring : MonoBehaviour
{
    public float forwardSpeed = 1;
    public float angularSpeed = 1;

    class Baker : Baker<WalkerAuthoring>
    {
        public override void Bake(WalkerAuthoring src)
        {
            var data = new Walker()
            {
                forwardSpeed = src.forwardSpeed,
                angularSpeed = src.angularSpeed
            };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
        }
    }
}
