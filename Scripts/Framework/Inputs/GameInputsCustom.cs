using System;
using UnityEngine.InputSystem;

namespace Framework.Inputs
{
    public partial class GameInputs
    {
        public GameInputs(InputActionAsset inputActions)
        {
            asset = inputActions;
            #region Copy-pasted action maps
            // LocalMap
            m_LocalMap = asset.FindActionMap("LocalMap", throwIfNotFound: true);
            m_LocalMap_Move = m_LocalMap.FindAction("Move", throwIfNotFound: true);
            m_LocalMap_Interact = m_LocalMap.FindAction("Interact", throwIfNotFound: true);
            m_LocalMap_MainMenu = m_LocalMap.FindAction("MainMenu", throwIfNotFound: true);
            // WorldMap
            m_WorldMap = asset.FindActionMap("WorldMap", throwIfNotFound: true);
            m_WorldMap_Move = m_WorldMap.FindAction("Move", throwIfNotFound: true);
            m_WorldMap_RideAirship = m_WorldMap.FindAction("RideAirship", throwIfNotFound: true);
            m_WorldMap_RideShip = m_WorldMap.FindAction("RideShip", throwIfNotFound: true);
            m_WorldMap_MainMenu = m_WorldMap.FindAction("MainMenu", throwIfNotFound: true);
            // Ship
            m_Ship = asset.FindActionMap("Ship", throwIfNotFound: true);
            m_Ship_Move = m_Ship.FindAction("Move", throwIfNotFound: true);
            m_Ship_RotateCamera = m_Ship.FindAction("RotateCamera", throwIfNotFound: true);
            m_Ship_RideAirship = m_Ship.FindAction("RideAirship", throwIfNotFound: true);
            m_Ship_GetOffShip = m_Ship.FindAction("GetOffShip", throwIfNotFound: true);
            m_Ship_MainMenu = m_Ship.FindAction("MainMenu", throwIfNotFound: true);
            // Airship
            m_Airship = asset.FindActionMap("Airship", throwIfNotFound: true);
            m_Airship_Move = m_Airship.FindAction("Move", throwIfNotFound: true);
            m_Airship_RotateCamera = m_Airship.FindAction("RotateCamera", throwIfNotFound: true);
            m_Airship_GetOffAirship = m_Airship.FindAction("GetOffAirship", throwIfNotFound: true);
            m_Airship_MainMenu = m_Airship.FindAction("MainMenu", throwIfNotFound: true);
            // Interactions
            m_Interactions = asset.FindActionMap("Interactions", throwIfNotFound: true);
            m_Interactions_AdvanceText = m_Interactions.FindAction("AdvanceText", throwIfNotFound: true);
            // Menus
            m_Menus = asset.FindActionMap("Menus", throwIfNotFound: true);
            m_Menus_Move = m_Menus.FindAction("Move", throwIfNotFound: true);
            m_Menus_Confirm = m_Menus.FindAction("Confirm", throwIfNotFound: true);
            // MenuCommon
            m_MenuCommon = asset.FindActionMap("MenuCommon", throwIfNotFound: true);
            m_MenuCommon_Cancel = m_MenuCommon.FindAction("Cancel", throwIfNotFound: true);
            m_MenuCommon_Sort = m_MenuCommon.FindAction("Sort", throwIfNotFound: true);
            m_MenuCommon_Remove = m_MenuCommon.FindAction("Remove", throwIfNotFound: true);
            m_MenuCommon_ChangeTab = m_MenuCommon.FindAction("ChangeTab", throwIfNotFound: true);
            m_MenuCommon_ChangeInfo = m_MenuCommon.FindAction("ChangeInfo", throwIfNotFound: true);
            m_MenuCommon_ChangePage = m_MenuCommon.FindAction("ChangePage", throwIfNotFound: true);
            // MainMenu
            m_MainMenu = asset.FindActionMap("MainMenu", throwIfNotFound: true);
            m_MainMenu_ReserveMembers = m_MainMenu.FindAction("ReserveMembers", throwIfNotFound: true);
            m_MainMenu_ActiveMembers = m_MainMenu.FindAction("ActiveMembers", throwIfNotFound: true);
            // FileMenu
            m_FileMenu = asset.FindActionMap("FileMenu", throwIfNotFound: true);
            m_FileMenu_DeleteSlot = m_FileMenu.FindAction("DeleteSlot", throwIfNotFound: true);
            // QuestMenu
            m_QuestMenu = asset.FindActionMap("QuestMenu", throwIfNotFound: true);
            m_QuestMenu_ShowObjectives = m_QuestMenu.FindAction("ShowObjectives", throwIfNotFound: true);
            m_QuestMenu_ShowSummary = m_QuestMenu.FindAction("ShowSummary", throwIfNotFound: true);
            m_QuestMenu_ScrollSummary = m_QuestMenu.FindAction("ScrollSummary", throwIfNotFound: true);
            m_QuestMenu_FollowQuest = m_QuestMenu.FindAction("FollowQuest", throwIfNotFound: true);
            // ItemMenu
            m_ItemMenu = asset.FindActionMap("ItemMenu", throwIfNotFound: true);
            m_ItemMenu_DiscardItem = m_ItemMenu.FindAction("DiscardItem", throwIfNotFound: true);
            // EquipMenu
            m_EquipMenu = asset.FindActionMap("EquipMenu", throwIfNotFound: true);
            m_EquipMenu_BestGear = m_EquipMenu.FindAction("BestGear", throwIfNotFound: true);
            // CraftingMenu
            m_CraftingMenu = asset.FindActionMap("CraftingMenu", throwIfNotFound: true);
            m_CraftingMenu_SimpleCraft = m_CraftingMenu.FindAction("SimpleCraft", throwIfNotFound: true);
            m_CraftingMenu_EffectList = m_CraftingMenu.FindAction("EffectList", throwIfNotFound: true);
            #endregion
        }

        /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.FindAction(Guid)" />
        public InputAction FindAction(Guid id)
        {
            return asset.FindAction(id);
        }
    }
}
