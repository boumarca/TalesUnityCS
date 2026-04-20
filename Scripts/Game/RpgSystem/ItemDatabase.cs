using Framework.Data;
using Game.RpgSystem.Data;
using UnityEngine;

namespace Game.RpgSystem
{
    [CreateAssetMenu(fileName = "NewItemDatabase", menuName = "Databases/ItemDatabase")]
    public class ItemDatabase : Database<RpgItemData>
    {
    }
}
