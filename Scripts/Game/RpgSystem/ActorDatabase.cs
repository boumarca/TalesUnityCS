using Framework.Data;
using Game.RpgSystem.Data;
using UnityEngine;

namespace Game.RpgSystem
{
    [CreateAssetMenu(fileName = "NewActorDatabase", menuName = "Databases/ActorDatabase")]
    public class ActorDatabase : Database<RpgActorData>
    {
    }
}
