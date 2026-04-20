using System;
using System.Collections;
using System.Collections.Generic;
using Game.CraftingSystem.Data;
using Game.UI;
using UnityEngine;

namespace Jrpg.Menus.Crafting
{
    public class NewRecipesWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private RecipeListEntry _entryPrefab;
        [SerializeField] private InputtableWindow _window;
        #endregion

        #region Private Fields
        private List<RecipeListEntry> _listEntries = new();
        #endregion

        #region Public Properties
        public InputtableWindow Window => _window;
        #endregion

        #region Events
        public event Action OnCloseEvent = delegate { };
        #endregion

        #region Public Methods
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            StartCoroutine(CloseCoroutine());
        }

        public void PopulateList(IReadOnlyCollection<CraftingRecipeData> newRecipes)
        {
            Clear();
            foreach (CraftingRecipeData recipe in newRecipes)
            {
                RecipeListEntry entry = Instantiate(_entryPrefab, transform);
                entry.InitializeItem(recipe);
                entry.HideQuantity();
                _listEntries.Add(entry);
            }
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnConfirmPerformed()
        {
            Hide();
        }
        #endregion

        #region Private Methods
        private void Clear()
        {
            foreach (RecipeListEntry entry in _listEntries)
                Destroy(entry.gameObject);

            _listEntries.Clear();
        }

        private IEnumerator CloseCoroutine()
        {
            yield return null;
            OnCloseEvent();
            gameObject.SetActive(false);
        }
        #endregion
    }
}
