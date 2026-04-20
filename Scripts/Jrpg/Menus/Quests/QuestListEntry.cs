using Game.QuestSystem.Models;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Jrpg.Menus.Views
{
    public class QuestListEntry : ListEntry
    {
        #region Serialized Fields
        [SerializeField] private TextMeshProUGUI _nameTextMesh;
        [SerializeField] private LocalizeStringEvent _nameText;
        [SerializeField] private GameObject _completedMarker;
        [SerializeField] private Color _completedColor;
        [SerializeField] private GameObject _followMarker;
        #endregion

        #region Public Properties
        public Quest Quest => Data as Quest;
        #endregion

        #region MenuListEntry Implementation
        protected override void OnInitialize()
        {
            _nameText.StringReference = Quest.Name;
            if (Quest.IsCompleted)
            {
                _completedMarker.SetActive(true);
                _nameTextMesh.color = _completedColor;
            }
        }
        #endregion

        #region Public Methods
        public void MarkAsFollowed()
        {
            _followMarker.SetActive(true);
        }

        public void UnmarkAsFollowed()
        {
            _followMarker.SetActive(false);
        }
        #endregion
    }
}
