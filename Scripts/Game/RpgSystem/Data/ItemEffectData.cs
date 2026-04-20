using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.RpgSystem.Data
{
    [CreateAssetMenu(fileName = "NewItemEffectData", menuName = "Game Data/ItemEffectData")]
    public class ItemEffectData : ScriptableObject
    {
        #region Serialized Fields and Properties
        [field: SerializeField] public EffectType EffectType {  get; private set; }
        [field: SerializeField] public LocalizedString DisplayName { get; private set; }
        [field: SerializeField] public LocalizedString Description { get; private set; }
        [field: SerializeField] public int MaxLevel { get; private set; }

        [SerializeField] private ItemType[] _compatibleItemTypes;
        [SerializeField] private int[] _levelValues;
        #endregion

        #region Public Methods
        public bool HasCompatibleType(ItemType itemType)
        {
            return _compatibleItemTypes.Contains(itemType);
        }

        public int GetEffectValue(int level)
        {
            return _levelValues[level - 1];
        }
        #endregion
    }
}
