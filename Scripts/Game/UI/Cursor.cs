using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class Cursor : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private GameObject _emptySelection;
        #endregion

        #region Private Fields
        private Vector3 _startPosition;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            _startPosition = transform.position;
        }

        private void OnEnable()
        {
            UIManager.Instance.OnSelectionChangedEvent += HandleOnSelectionChanged;
        }

        private void OnDisable()
        {
            UIManager.Instance.OnSelectionChangedEvent -= HandleOnSelectionChanged;
        }
        #endregion

        #region Public Methods
        public void ResetPosition()
        {
            transform.position = _startPosition;
        }
        #endregion

        #region Private Methods
        private void HandleOnSelectionChanged(object sender, EventArgs e)
        {
            StartCoroutine(MoveCursorToSelection());
        }

        private IEnumerator MoveCursorToSelection()
        {
            //TODO: Make cursor movement animation
            yield return new WaitForSeconds(0.05f);
            GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
            if (currentSelection == null)
            {
                if(_emptySelection == null)
                    yield break;
                currentSelection = _emptySelection;
            }
            transform.position = currentSelection.transform.position;
        }
        #endregion
    }
}
