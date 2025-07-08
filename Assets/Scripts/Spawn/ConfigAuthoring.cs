using Unity.Entities;
using UnityEngine;

public struct Config : IComponentData
{
    public Entity Prefab;
    public float SpawnRadius;
    public int SpawnCount;
    public uint RandomSeed;
}


public class ConfigAuthoring : MonoBehaviour
{
    [field: SerializeField]
    public GameObject Prefab { get; private set; } = null;

    [field: SerializeField]
    public float SpawnRadius { get; private set; } = 1f;

    [field: SerializeField] 
    public int SpawnCont { get; private set; } = 10;

    [field: SerializeField] 
    public uint RandomSeed { get; private set; } = 100;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var data = new Config()
            {
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                SpawnRadius = authoring.SpawnRadius,
                SpawnCount = authoring.SpawnCont,
                RandomSeed = authoring.RandomSeed
            };

            AddComponent(GetEntity(TransformUsageFlags.None), data);            
        }
    }
}
