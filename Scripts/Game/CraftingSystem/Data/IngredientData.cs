using System;
using Framework.Utils;
using Game.RpgSystem.Data;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.CraftingSystem.Data
{
    [Serializable]
    public class IngredientData
    {
        #region  Serialized Fields
        [SerializeField] private bool _isCategory;
        [HideIf(nameof(_isCategory), true)]
        [SerializeField] private RpgItemData _item;
        [HideIf(nameof(_isCategory), false)]
        [SerializeField] private ItemCategory _category;
        [field: SerializeField] public int Quantity { get; private set; }
        #endregion

        #region Public Properties
        public RpgItemData Item => _item;
        public ItemCategory Category => _category;
        public bool IsCategory => _isCategory;
        public LocalizedString Name => IsCategory ? Category.ToLocalizedString() : Item.Name;
        #endregion
    }
}
