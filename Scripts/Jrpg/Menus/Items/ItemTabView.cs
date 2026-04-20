using System;
using Game.RpgSystem.Data;
using Game.UI;

namespace Jrpg.Menus.Items
{
    public class ItemTabView : TabList
    {
        #region Events
        public event Action OnNewTabSelectedEvent = delegate { };
        public event Action<ItemType> OnFilteredTabSelectedEvent = delegate { };
        #endregion

        #region Unity Event UI Methods
        public void UIOnNewItemsTabSelected()
        {
            OnNewTabSelectedEvent();
        }

        public void UIOnAllItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.None);
        }

        public void UIOnConsummablesItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.Consummable);
        }

        public void UIOnWeaponsItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.Weapon);
        }

        public void UIOnArmorBodyItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.ArmorBody);
        }

        public void UIOnArmorHeadItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.ArmorHead);
        }

        public void UIOnArmorArmItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.ArmorArm);
        }

        public void UIOnAccessoryItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.Accessory);
        }

        public void UIOnMaterialsItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.Material);
        }

        public void UIOnKeyItemsTabSelected()
        {
            OnFilteredTabSelectedEvent(ItemType.KeyItem);
        }
        #endregion
    }
}
