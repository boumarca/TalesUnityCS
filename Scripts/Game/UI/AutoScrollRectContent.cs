using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Scroll Rect - automatic scroll with elements navigation
    /// </summary>
    public class AutoScrollRectContent : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;
        #endregion

        #region MonoBehaviour Methods
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
        public void ScrollPage(int direction)
        {
            float contentY = _content.anchoredPosition.y;
            float viewportY = _viewport.anchoredPosition.y;
            float viewportHeight = _viewport.rect.height;

            float viewportBottom = viewportY - viewportHeight;
            float contentBottom = contentY - _content.rect.height;

            if (direction > 0)
                contentY += Mathf.Min(viewportHeight, Mathf.Abs(contentBottom - viewportBottom));
            else
                contentY -= Mathf.Min(viewportHeight, Mathf.Abs(contentY - viewportY));

            _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, contentY);
        }
        #endregion

        #region Private Methods
        private void HandleOnSelectionChanged(object sender, EventArgs e)
        {
            StartCoroutine(DelayedScrollContent());
        }

        private IEnumerator DelayedScrollContent()
        {
            yield return null;
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject == null)
                yield break;

            RectTransform selectedRect = selectedObject.GetComponent<RectTransform>();
            if (!selectedRect.IsChildOf(transform))
                yield break;

            if (_scrollRect.horizontal)
                ScrollHorizontal(selectedRect);

            if(_scrollRect.vertical)
                ScrollVerticalPivotTop(selectedRect);
        }

        private void ScrollHorizontal(RectTransform selectedRect)
        {
            //TODO: do the same for horizontal scroll
        }

        private void ScrollVerticalPivotTop(RectTransform selectedRect)
        {
            float contentY = _content.anchoredPosition.y;

            //TODO: Handle arbitrary pivot position
            //Assumes the pivot is on top and items goes from top to bottom.
            float viewportTop = -contentY;
            float viewportBottom = -(contentY + _viewport.rect.height);

            float selectedRectHeight = selectedRect.rect.height;
            float selectedRectTop = selectedRect.offsetMax.y;
            float selectedRectBottom = selectedRect.offsetMin.y;

            //To scroll down, move anchoredPosition up.
            //To scroll up, move anchoredPosition down.
            if (selectedRectBottom < viewportBottom)
                contentY += Mathf.Max(selectedRectHeight, viewportBottom - selectedRectBottom);
            else if (selectedRectTop > viewportTop)
                contentY -= Mathf.Max(selectedRectHeight, selectedRectTop - viewportTop);
            
            _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, contentY);
        }
        #endregion
    }
}
