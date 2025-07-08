using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct Dancer : IComponentData
{
    public float speed;

    public static Dancer Random(uint seed)
    {
        var random = new Unity.Mathematics.Random(seed);
        return new Dancer() { speed = random.NextFloat(1f, 8f) };
    }
}

public class DancerAuthoring : MonoBehaviour
{
    public float speed = 1f;

    class Baker : Baker<DancerAuthoring>
    {
        public override void Bake(DancerAuthoring src)
        {
            var data = new Dancer() { speed = src.speed };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
        }
    }
}
