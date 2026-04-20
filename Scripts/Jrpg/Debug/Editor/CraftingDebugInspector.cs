using System.Collections.Generic;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.CraftingSystem.Models;
using Game.RpgSystem;
using Game.RpgSystem.Models;
using UnityEditor;
using UnityEngine;

namespace Jrpg.Debug.Editor
{
    [CustomEditor(typeof(CraftingDebugMenu))]
    public class CraftingDebugInspector : AbstractDebugInspector
    {
        #region Private Fields
        private CraftingDebugMenu _craftingDebugMenu;
        #endregion

        #region AbstractDebugInspector Implementation
        public override void SetDebugMenu()
        {
            _craftingDebugMenu = (CraftingDebugMenu)target;
        }

        public override void DisplayCheats()
        {
            GUI.enabled = _craftingDebugMenu.CheatValue > 0;
            if (GUILayout.Button($"Give {_craftingDebugMenu.CheatValue} Crafting Experience"))
                _craftingDebugMenu.CheatGiveCraftingExperience();
            GUI.enabled = true;
        }

        public override void DisplayDebugFoldout()
        {
            GUI.enabled = false;
            EditorGUI.indentLevel++;
            EditorGUILayout.IntField("Crafting Level", _craftingDebugMenu.CraftingLevel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Current XP: {_craftingDebugMenu.TotalExperience}");
            EditorGUILayout.LabelField($"Next: {_craftingDebugMenu.RemainingExperienceToNextLevel()}");
            EditorGUILayout.EndHorizontal();
            if (_craftingDebugMenu.IsCraftingStarted)
                DisplaySelectIngredientState();
            else
                DisplaySelectRecipeState();
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Private Methods
        private void DisplaySelectRecipeState()
        {
            GUI.enabled = true;
            DisplaySelectedRecipe();
            EditorGUILayout.LabelField("Learned Recipes");
            EditorGUI.indentLevel++;
            foreach (CraftingRecipeData recipe in _craftingDebugMenu.LearnedRecipes)
                DisplayRecipe(recipe);

            EditorGUI.indentLevel--;
        }

        private void DisplaySelectIngredientState()
        {
            CraftingItem itemModel = _craftingDebugMenu.CurrentItemModel;

            GUI.enabled = true;
            DisplayPartyMemberList();
            DisplayCraftingSkills();

            EditorGUILayout.LabelField("Selected Ingredients");
            EditorGUI.indentLevel++;
            for (int i = 0; i < itemModel.IngredientCount; i++)
                DisplayIngredientDropDown(i);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Inherited Effects");
            EditorGUI.indentLevel++;
            foreach (ItemEffect effect in itemModel.InheritedEffects)
                DisplayInheritedEffect(effect);
            EditorGUI.indentLevel--;

            GUI.enabled = _craftingDebugMenu.CanPerformCrafting();
            if (GUILayout.Button($"Perform Crafting"))
                _craftingDebugMenu.PerformCrafting();

            GUI.enabled = true;
            if (GUILayout.Button($"Cancel Crafting"))
                _craftingDebugMenu.CancelCrafting();
        }

        private void DisplayPartyMemberList()
        {
            IReadOnlyList<RpgActor> partyMembers = PartyManager.Instance.CurrentParty.Members;
            string[] options = new string[partyMembers.Count];
            int selectedIndex = 0;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                RpgActor actor = partyMembers[i];
                options[i] = actor.Name.GetLocalizedString();
                if (actor == _craftingDebugMenu.SelectedActor)
                    selectedIndex = i;
            }

            selectedIndex = EditorGUILayout.Popup("Crafter Actor", selectedIndex, options);
            _craftingDebugMenu.SetSelectedActor(partyMembers[selectedIndex]);
        }

        private void DisplayCraftingSkills()
        {
            EditorGUILayout.LabelField("Active Crafting Skills");
            EditorGUI.indentLevel++;
            IReadOnlyCollection<CraftingSkillData> craftingSkills = _craftingDebugMenu.GetActorCraftingSkills();
            foreach(CraftingSkillData skill in craftingSkills)
                EditorGUILayout.LabelField(skill.Name.GetLocalizedString());
            EditorGUI.indentLevel--;
        }

        private void DisplayIngredientDropDown(int slotIndex)
        {
            //TODO: Remove assigned items
            CraftingItem itemModel = _craftingDebugMenu.CurrentItemModel;
            IngredientData ingredientData = itemModel.GetIngredientDataForSlot(slotIndex);
            IReadOnlyList<InventoryItem> inventoryItems = _craftingDebugMenu.GetCompatibleItems(ingredientData);
            string[] options = new string[inventoryItems.Count+1];
            options[0] = "None";
            int selectedIndex = 0;
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                InventoryItem inventoryItem = inventoryItems[i];
                options[i+1] = $"{i}: " + inventoryItem.Item.Name.GetLocalizedString();
                if(inventoryItem.Item == itemModel.GetIngredientInSlot(slotIndex))
                    selectedIndex = i+1;
            }

            selectedIndex = EditorGUILayout.Popup(ingredientData.Name.GetLocalizedString(), selectedIndex, options);
            if(selectedIndex > 0)
                _craftingDebugMenu.AssignIngredientToSlot(slotIndex, inventoryItems[selectedIndex - 1].Item);
            else
                _craftingDebugMenu.EmptyIngredientSlot(slotIndex);
        }

        private void DisplayInheritedEffect(ItemEffect effect)
        {
            CraftingItem itemModel = _craftingDebugMenu.CurrentItemModel;
            bool isSelected = itemModel.IsEffectSelected(effect);
            isSelected = EditorGUILayout.ToggleLeft(effect.DisplayName, isSelected);
            if(isSelected)
                itemModel.SelectEffect(effect);
            else
                itemModel.DeselectEffect(effect);
        }

        private void DisplayRecipe(CraftingRecipeData recipe)
        {
            EditorGUILayout.BeginHorizontal();
            bool isSelected = _craftingDebugMenu.SelectedRecipe == recipe;
            EditorGUILayout.LabelField($"{(isSelected ? "[Selected]" : string.Empty)} {recipe.Item.Name.GetLocalizedString()}");
            GUI.enabled = !isSelected;
            if (GUILayout.Button($"Select", GUILayout.Width(60)))
                _craftingDebugMenu.SelectedRecipe = recipe;
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void DisplaySelectedRecipe()
        {
            EditorGUILayout.BeginHorizontal();
            CraftingRecipeData recipe = _craftingDebugMenu.SelectedRecipe;
            EditorGUILayout.LabelField($"Selected Recipe: {(recipe != null ? recipe.Item.Name.GetLocalizedString() : "None")}");
            if (recipe == null)
            {
                EditorGUILayout.EndHorizontal();
                return;
            }

            EditorGUILayout.LabelField($"Required Crafting Level: {recipe.RecipeLevel}");
            EditorGUILayout.EndHorizontal();
            DisplayRecipeIngredients();

            bool guiEnabled = GUI.enabled;
            GUI.enabled = _craftingDebugMenu.CanCraftRecipe(recipe);
            if (GUILayout.Button($"Start Crafting"))
                _craftingDebugMenu.StartCrafting();

            if (GUILayout.Button($"Auto-Craft"))
                _craftingDebugMenu.AutoCraft();
            GUI.enabled = guiEnabled;
        }

        private void DisplayRecipeIngredients()
        {
            EditorGUI.indentLevel++;
            CraftingRecipeData recipe = _craftingDebugMenu.SelectedRecipe;
            foreach (IngredientData ingredient in recipe.Ingredients)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(ingredient.Name.GetLocalizedString());
                EditorGUILayout.LabelField($"{ingredient.Quantity} / {_craftingDebugMenu.GetOwnedIngredientQuantity(ingredient)}");
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
        #endregion
    }
}
