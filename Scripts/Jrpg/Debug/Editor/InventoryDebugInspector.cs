using System.Collections.Generic;
using Framework.Identifiers;
using Game.RpgSystem.Models;
using UnityEditor;
using UnityEngine;

namespace Jrpg.Debug.Editor
{
    [CustomEditor(typeof(InventoryDebugMenu))]
    public class InventoryDebugInspector : AbstractDebugInspector
    {
        #region Statics
        private static List<InventoryItem> s_inventoryBuffer = new();
        #endregion

        #region Private Fields
        private InventoryDebugMenu _inventoryDebugMenu;
        #endregion

        #region AbstractDebugInspector Implementation
        public override void SetDebugMenu()
        {
            _inventoryDebugMenu = (InventoryDebugMenu) target;
        }

        public override void DisplayCheats()
        {
            GUI.enabled = Identifier.IsValid(_inventoryDebugMenu.SelectedItem) && _inventoryDebugMenu.ItemCount > 0;
            if (GUILayout.Button($"Add {_inventoryDebugMenu.SelectedItem} x{_inventoryDebugMenu.ItemCount} to inventory"))
                _inventoryDebugMenu.CheatAddItem();
            GUI.enabled = true;
        }

        public override void DisplayDebugFoldout()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Current Inventory");
            EditorGUI.indentLevel++;
            s_inventoryBuffer.Clear();
            s_inventoryBuffer.AddRange(_inventoryDebugMenu.Inventory);
            foreach (InventoryItem item in s_inventoryBuffer)
                DisplayItem(item);

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Private Methods   
        private void DisplayItem(InventoryItem item)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{item.Item.Name.GetLocalizedString()} x{item.Quantity}");

            if (GUILayout.Button($"-{Mathf.Clamp(_inventoryDebugMenu.ItemCount, 0, item.Quantity)}", GUILayout.Width(40)))
                _inventoryDebugMenu.CheatRemoveQuantity(item);

            if (GUILayout.Button("X", GUILayout.Width(25)))
                _inventoryDebugMenu.CheatRemoveItem(item);

            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }
}
