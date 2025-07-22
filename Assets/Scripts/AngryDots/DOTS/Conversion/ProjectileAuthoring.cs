/* PROJECTILE AUTHORING
* This script handles converting the Bullet prefab into an entity. The
* data components given to this entity will allow it to be used by
* the various systems that provide the functionality
*/
using Unity.Entities;
using UnityEngine;

// This script will go on the Bullet prefab
public class ProjectileAuthoring : MonoBehaviour
{
    public class ProjectileBaker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            var projectileBehavior = authoring.GetComponent<ProjectileBehaviour>();

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TimeToLive { Value = projectileBehavior.lifeTime });
            AddComponent(entity, new MoveSpeed { Value = projectileBehavior.speed });
            AddComponent(entity, new MoveForward { });
        }
    }


}
