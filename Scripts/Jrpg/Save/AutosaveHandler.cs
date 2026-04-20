using System;
using System.Collections;
using Game.SaveSystem;
using Jrpg.Maps;
using UnityEngine;

namespace Jrpg.Save
{
    public class AutosaveHandler : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private GameObject _autosaveLabel;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            if (MapStateManager.Instance != null)
                MapStateManager.Instance.OnMapStateChangedEvent += HandleOnAutosaveTriggered;
        }

        private void OnDisable()
        {
            if (MapStateManager.Instance != null)
                MapStateManager.Instance.OnMapStateChangedEvent -= HandleOnAutosaveTriggered;
        }
        #endregion

        #region Private Methods
        private void HandleOnAutosaveTriggered(object sender, EventArgs args)
        {
            Autosave();
        }

        private void Autosave()
        {
            SaveDataManager.Instance.SaveGameAtIndex(-1);
            StartCoroutine(DisplayAutosaveLabel());
        }

        private IEnumerator DisplayAutosaveLabel()
        {
            _autosaveLabel.SetActive(true);
            yield return new WaitForSeconds(1);
            _autosaveLabel.SetActive(false);
        }
        #endregion
    }
}
