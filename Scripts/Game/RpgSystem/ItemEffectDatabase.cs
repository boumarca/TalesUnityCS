using Framework.Data;
using Game.RpgSystem.Data;
using UnityEngine;

namespace Game.RpgSystem
{
    [CreateAssetMenu(fileName = "NewItemEffectDatabase", menuName = "Databases/ItemEffectDatabase")]
    public class ItemEffectDatabase : Database<ItemEffectData>
    {
    }
}
